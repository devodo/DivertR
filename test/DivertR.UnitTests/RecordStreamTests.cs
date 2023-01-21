using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Record;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordStreamTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly IRecordStream<IFoo> _recordStream;

        public RecordStreamTests()
        {
            _recordStream = _redirect.Record(opt => opt
                .OrderFirst());
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => Guid.NewGuid().ToString());

            var fooProxy = _redirect.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            _recordStream.Select(x => x.CallInfo.Arguments[0]).ShouldBe(inputs);
            _recordStream.Select(x => x.Return).ShouldBe(outputs);

            var echoCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any))
                .Args<(string input, __)>();
            
            echoCalls.Select(call => call.Args.input).ShouldBe(inputs);
            echoCalls.Select(call => call.Return).ShouldBe(outputs);

            var i = 0;
            echoCalls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[i++]);
            }).Count().ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenToCallConstraint_ShouldFilter()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, _) => $"{i}")
                .ToList();
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => Guid.NewGuid().ToString());

            var fooProxy = _redirect.Proxy();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ACT
            var calls = _recordStream
                .To(x => x.Echo(inputs[0]))
                .Args<(string input, __)>();

            // ASSERT
            calls.Count().ShouldBe(1);
            calls.Single().CallInfo.Arguments.Count.ShouldBe(1);
            calls.Single().CallInfo.Proxy.ShouldBeSameAs(fooProxy);

            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[0]);
                call.Return.ShouldBe(outputs[0]);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordCalls_WhenException_ShouldRecordException()
        {
            // ARRANGE
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => throw new Exception("test"));

            // ACT
            Exception? caughtException = null;
            try
            {
                _redirect.Proxy().Echo("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            caughtException.ShouldNotBeNull();
            _recordStream
                .To(x => x.Echo("test"))
                .Verify<(string input, __)>(call =>
                {
                    call.Args.input.ShouldBe("test");
                    call.Exception.ShouldBeSameAs(caughtException);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordCalls_WhenAsyncException_ShouldRecordReturnTask()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via(async () =>
                {
                    await Task.Yield();
                    throw new Exception("test");
                });

            // ACT
            Exception? caughtException = null;
            try
            {
                await _redirect.Proxy().EchoAsync("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            var call = _recordStream.To(x => x.EchoAsync("test")).Single();
            caughtException.ShouldNotBeNull();
            call.Exception.ShouldBeSameAs(caughtException);
            call.RawException.ShouldBeNull();
            
            Exception? returnedException = null;
            try
            {
                await call.Return!;
            }
            catch (Exception ex)
            {
                returnedException = ex;
            }
            
            returnedException.ShouldBeSameAs(caughtException);
        }
        
        [Fact]
        public async Task GivenRecordCalls_WhenNonAwaitedAsyncException_ShouldRecordReturnTask()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via(async () =>
                {
                    await Task.Yield();
                    throw new Exception("test");
                });

            // ACT
            Exception? caughtException = null;
            try
            {
                var _ = _redirect.Proxy().EchoAsync("test").Result;
            }
            catch (AggregateException ex)
            {
                caughtException = ex.InnerExceptions.First();
            }

            // ASSERT
            var call = _recordStream.To(x => x.EchoAsync("test")).Single();
            caughtException.ShouldNotBeNull();
            call.Exception.ShouldBeSameAs(caughtException);
            call.RawException.ShouldBeNull();
            
            Exception? returnedException = null;
            try
            {
                await call.Return!;
            }
            catch (Exception ex)
            {
                returnedException = ex;
            }
            
            returnedException.ShouldBeSameAs(caughtException);
        }
        
        [Fact]
        public async Task GivenRecord_WithStrict_WhenNoMatchingVias_ThrowsStrictNotSatisfiedException()
        {
            // ARRANGE
            _redirect.Strict()
                .To(x => x.EchoAsync(Is<string>.Match(m => m != "test")))
                .Via<(string input, __)>(call => call.Relay.Next.EchoAsync(call.Args.input));

            // ACT
            var testAction = () => _redirect.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            await testAction.ShouldThrowAsync<StrictNotSatisfiedException>();
            
            _recordStream
                .To(x => x.EchoAsync("test"))
                .Verify(call =>
                {
                    call.CallInfo.Arguments[0].ShouldBe("test");
                    call.Exception.ShouldBeOfType<StrictNotSatisfiedException>();
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordWithStrict_WhenMatchingVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect.Strict();
            _redirect
                .To(x => x.EchoAsync("test"))
                .Via<(string input, __)>(async call => await call.Relay.Next.EchoAsync(call.Args.input) + " diverted");

            // ACT
            var result = await _redirect.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            result.ShouldBe("original: test diverted");

            _recordStream
                .To(x => x.EchoAsync("test"))
                .Verify<(string input, __)>(call =>
                {
                    call.Args.input.ShouldBe("test");
                    call.Return!.Result.ShouldBe(result);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenProxyCallsToAction_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .To(x => x.SetName(Is<string>.Any))
                .Via(() => { });

            var fooProxy = _redirect.Proxy();

            // ACT
            inputs.ForEach(x => fooProxy.SetName(x));

            // ASSERT
            _recordStream
                .To(x => x.SetName(Is<string>.Any))
                .Args<(string name, __)>()
                .Select((call, i) =>
                {
                    call.Return.ShouldBeNull();
                    call.Args.name.ShouldBe(inputs[i]);

                    return call.Args.name;
                })
                .ShouldBe(inputs);
        }
        
        [Fact]
        public void GivenProxyCallsToPropertyGetter_ShouldRecord()
        {
            // ARRANGE
            _redirect
                .To(x => x.Name)
                .Via(() => Guid.NewGuid().ToString());

            var fooProxy = _redirect.Proxy();

            // ACT
            var outputs = Enumerable
                .Range(0, 20)
                .Select(_ => fooProxy.Name).ToList();

            // ASSERT
            var recordedCalls = _recordStream.To(x => x.Name);
            recordedCalls
                .Select(call => call.Return)
                .ShouldBe(outputs);

            recordedCalls.Verify(call =>
            {
                outputs.ShouldContain(call.Return);
            }).Count.ShouldBe(outputs.Count);
        }
        
        [Fact]
        public void GivenProxyCallsToPropertySetter_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .ToSet(x => x.Name, () => Is<string>.Any)
                .Via(() => { });

            var fooProxy = _redirect.Proxy();

            // ACT
            inputs.ForEach(x => fooProxy.Name = x);

            // ASSERT
            var recordedCalls = _recordStream
                .ToSet(x => x.Name, () => Is<string>.Any)
                .Args<(string name, __)>();
                
            recordedCalls
                .Select(call => call.Args.name)
                .ShouldBe(inputs);

            var i = 0;
            recordedCalls.Verify(call =>
            {
                call.Args.name.ShouldBe(inputs[i++]);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenRecordCallStream_ShouldDeferIterator()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, _) => $"{i}")
                .ToList();
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => Guid.NewGuid().ToString());

            var fooProxy = _redirect.Proxy();

            var recordedCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any));
            
            var recordedOutputs = recordedCalls.Select(call => call.Return);

            // ACT
            // ReSharper disable once PossibleMultipleEnumeration (Testing deferred enumeration)
            var beforeCount = recordedOutputs.Count();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            beforeCount.ShouldBe(0);
            recordedCalls.Verify<(string input, __)>().Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                return call;
            }).Count().ShouldBe(inputs.Count);
            // ReSharper disable once PossibleMultipleEnumeration (Testing deferred enumeration)
            recordedOutputs.Count().ShouldBe(inputs.Count);
            // ReSharper disable once PossibleMultipleEnumeration (Testing deferred enumeration)
            recordedOutputs.ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenRecordMap_ShouldRecordAndMap()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => Guid.NewGuid().ToString());

            var mappedCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any))
                .Args<(string input, __)>()
                .Map(call => new { Input = call.Args.input, Result = call.Return });

            var fooProxy = _redirect.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            mappedCalls.Verify().Select((call, i) =>
            {
                call.Input.ShouldBe(inputs[i]);
                call.Result.ShouldBe(outputs[i]);
                return call;
            }).Count().ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenRecordMapTask_ShouldRecordAndMap()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via(() => Task.FromResult(Guid.NewGuid().ToString()));

            var mappedCalls = _recordStream
                .To(x => x.EchoAsync(Is<string>.Any))
                .Args<(string input, __)>()
                .Map(call => new { Input = call.Args.input, Result = call.Return });

            var fooProxy = _redirect.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.EchoAsync(x)).ToList();

            // ASSERT
            var calls = mappedCalls.Select(async (call, i) =>
            {
                call.Input.ShouldBe(inputs[i]);
                (await call.Result!).ShouldBe((await outputs[i]));
                return call;
            }).ToList();
            
            await Task.WhenAll(calls);
            calls.Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenRecordedCalls_ShouldVerifyCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(() => Guid.NewGuid().ToString());

            var fooProxy = _redirect.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            var index = 0;
            _recordStream.Verify(call =>
            {
                call.Return.ShouldBe(outputs[index]);
                call.Args[0].ShouldBe(inputs[index++]);
            }).Count.ShouldBe(inputs.Count);
        }
    }
}