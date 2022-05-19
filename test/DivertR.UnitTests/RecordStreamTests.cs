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
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IRecordStream<IFoo> _recordStream;

        public RecordStreamTests()
        {
            _recordStream = _via.Record(opt => opt
                .OrderFirst()
                .DisableSatisfyStrict());
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            _recordStream.Select(x => x.CallInfo.Arguments[0]).ShouldBe(inputs);
            _recordStream.Select(x => x.Returned?.Value).ShouldBe(outputs);

            var echoCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any))
                .WithArgs<(string input, __)>();
            
            echoCalls.Select(call => call.Args.input).ShouldBe(inputs);
            echoCalls.Select(call => call.Returned!.Value).ShouldBe(outputs);
            echoCalls.Replay((_, args, i) => args.input.ShouldBe(inputs[i])).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenToCallConstraint_ShouldFilter()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, _) => $"{i}")
                .ToList();
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ACT
            var calls = _recordStream
                .To(x => x.Echo(inputs[0]))
                .WithArgs<(string input, __)>();

            // ASSERT
            calls.Count().ShouldBe(1);
            calls.Single().CallInfo.Arguments.Count.ShouldBe(1);
            calls.Single().CallInfo.Proxy.ShouldBeSameAs(fooProxy);

            calls.Replay(call =>
            {
                call.Args.input.ShouldBe(inputs[0]);
                call.Returned!.Value.ShouldBe(outputs[0]);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordCalls_WhenException_ShouldRecordException()
        {
            // ARRANGE
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => throw new Exception("test"));

            // ACT
            Exception caughtException = null;
            try
            {
                _via.Proxy().Echo("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            caughtException.ShouldNotBeNull();
            _recordStream
                .To(x => x.Echo("test"))
                .Replay<(string input, __)>((call, args) =>
                {
                    args.input.ShouldBe("test");
                    call.Returned?.Exception.ShouldBeSameAs(caughtException);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordCalls_WhenAsyncException_ShouldRecordReturnTask()
        {
            // ARRANGE
            _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect(async () =>
                {
                    await Task.Yield();
                    throw new Exception("test");
                });

            // ACT
            Exception caughtException = null;
            try
            {
                await _via.Proxy().EchoAsync("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            var call = _recordStream.To(x => x.EchoAsync("test")).Single();
            caughtException.ShouldNotBeNull();
            call.Returned!.Exception.ShouldBeSameAs(caughtException);
            
            Exception returnedException = null;
            try
            {
                await call.Returned!.Value;
            }
            catch (Exception ex)
            {
                returnedException = ex;
            }
            
            returnedException.ShouldBeSameAs(caughtException);
        }
        
        [Fact]
        public async Task GivenRecord_WithStrict_WhenNoMatchingRedirects_ThrowsStrictNotSatisfiedException()
        {
            // ARRANGE
            _via.Strict();
            _via
                .To(x => x.EchoAsync(Is<string>.Match(m => m != "test")))
                .Redirect<(string input, __)>(call => call.Relay.Next.EchoAsync(call.Args.input));

            // ACT
            Func<Task<string>> testAction = () => _via.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            await testAction.ShouldThrowAsync<StrictNotSatisfiedException>();
            
            _recordStream
                .To(x => x.EchoAsync("test"))
                .Replay(call =>
                {
                    call.CallInfo.Arguments[0].ShouldBe("test");
                    call.Returned!.Exception.ShouldBeOfType<StrictNotSatisfiedException>();
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecord_WithStrict_WhenMatchingRedirect_Redirects()
        {
            // ARRANGE
            _via.Strict();
            _via
                .To(x => x.EchoAsync("test"))
                .Redirect<(string input, __)>(async call => await call.Relay.Next.EchoAsync(call.Args.input) + " diverted");

            // ACT
            var result = await _via.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            result.ShouldBe("original: test diverted");

            _recordStream
                .To(x => x.EchoAsync("test"))
                .Replay<(string input, __)>(call =>
                {
                    call.Args.input.ShouldBe("test");
                    call.Returned!.Value.Result.ShouldBe(result);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenProxyCallsToAction_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .To(x => x.SetName(Is<string>.Any))
                .Redirect(() => { });

            var fooProxy = _via.Proxy();

            // ACT
            inputs.ForEach(x => fooProxy.SetName(x));

            // ASSERT
            _recordStream
                .To(x => x.SetName(Is<string>.Any))
                .WithArgs<(string name, __)>()
                .Select((call, i) =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.name.ShouldBe(inputs[i]);

                    return call.Args.name;
                })
                .ShouldBe(inputs);
        }
        
        [Fact]
        public void GivenProxyCallsToPropertyGetter_ShouldRecord()
        {
            // ARRANGE
            _via
                .To(x => x.Name)
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();

            // ACT
            var outputs = Enumerable
                .Range(0, 20)
                .Select(_ => fooProxy.Name).ToList();

            // ASSERT
            var recordedCalls = _recordStream.To(x => x.Name);
            recordedCalls
                .Select(call => call.Returned!.Value)
                .ShouldBe(outputs);

            recordedCalls.Replay(call =>
            {
                outputs.ShouldContain(call.Returned!.Value);
            }).Count.ShouldBe(outputs.Count);
        }
        
        [Fact]
        public void GivenProxyCallsToPropertySetter_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .ToSet(x => x.Name, () => Is<string>.Any)
                .Redirect(() => { });

            var fooProxy = _via.Proxy();

            // ACT
            inputs.ForEach(x => fooProxy.Name = x);

            // ASSERT
            var recordedCalls = _recordStream
                .ToSet(x => x.Name, () => Is<string>.Any)
                .WithArgs<(string name, __)>();
                
            recordedCalls
                .Select(call => call.Args.name)
                .ShouldBe(inputs);
                
            recordedCalls.Replay((call, args, i) =>
            {
                args.name.ShouldBe(inputs[i]);
                call.Returned!.Value.ShouldBeNull();
            }).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenRecordCallStream_ShouldDeferIterator()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, _) => $"{i}")
                .ToList();
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();

            var recordedCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any));
            
            var recordedOutputs = recordedCalls.Select(call => call.Returned!.Value);

            // ACT
            // ReSharper disable once PossibleMultipleEnumeration (Testing deferred enumeration)
            var beforeCount = recordedOutputs.Count();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            beforeCount.ShouldBe(0);
            recordedCalls.Replay<(string input, __)>((call, i) => call.Args.input.ShouldBe(inputs[i])).Count.ShouldBe(inputs.Count);
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
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var mappedCalls = _recordStream
                .To(x => x.Echo(Is<string>.Any))
                .WithArgs<(string input, __)>()
                .Map((call, args) => new { Input = args.input, Result = call.Returned });

            var fooProxy = _via.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            mappedCalls.Replay((call, i) =>
            {
                call.Input.ShouldBe(inputs[i]);
                call.Result?.Value.ShouldBe(outputs[i]);
            }).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenRecordMapTask_ShouldRecordAndMap()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect(() => Task.FromResult(Guid.NewGuid().ToString()));

            var mappedCalls = _recordStream
                .To(x => x.EchoAsync(Is<string>.Any))
                .WithArgs<(string input, __)>()
                .Map((call, args) => new { Input = args.input, Result = call.Returned });

            var fooProxy = _via.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.EchoAsync(x)).ToList();

            // ASSERT
            (await mappedCalls.Replay(async (call, i) =>
            {
                call.Input.ShouldBe(inputs[i]);
                (await call.Result!.Value).ShouldBe((await outputs[i]));
            })).Count.ShouldBe(inputs.Count);
        }
    }
}