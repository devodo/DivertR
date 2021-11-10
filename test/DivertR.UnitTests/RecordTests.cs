using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Record;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly ICallStream<IFoo> _callStream;

        public RecordTests()
        {
            _callStream = _via.Record();
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
            _callStream.Select(x => x.CallInfo.Arguments[0]).ShouldBe(inputs);
            _callStream.Select(x => x.Returned?.Value).ShouldBe(outputs);

            var echoCalls = _callStream.To(x => x.Echo(Is<string>.Any));
            echoCalls.Select(call => call.Args((string input) => input)).ShouldBe(inputs);
            echoCalls.Select(call => call.Returned!.Value).ShouldBe(outputs);
            
            _callStream
                .To(x => x.Echo(Is<string>.Any))
                .Select((call, i) =>
                {
                    call.Args((string input) =>
                    {
                        input.ShouldBe(inputs[i]);
                    });
                    
                    return call.Returned!.Value;
                }).ShouldBe(outputs);
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
            var calls = _callStream.To(x => x.Echo(inputs[0]));

            // ASSERT
            calls.Count.ShouldBe(1);
            calls.Single().CallInfo.Arguments.Count.ShouldBe(1);
            calls.Single().CallInfo.Proxy.ShouldBeSameAs(fooProxy);

            calls.ForEach(call =>
            {
                call.Args<string>().ShouldBe(inputs[0]);
                call.Returned!.Value.ShouldBe(outputs[0]);
            });
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
            _callStream
                .To(x => x.Echo("test"))
                .ForEach(call =>
                {
                    call.Args<string>().ShouldBe("test");
                    call.Returned?.Exception.ShouldBeSameAs(caughtException);
                })
                .Count.ShouldBe(1);
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
            var call = _callStream.To(x => x.EchoAsync("test")).Single();
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
                .Redirect<(string input, __)>(args => _via.Relay.Next.EchoAsync(args.input));

            // ACT
            Func<Task<string>> testAction = () => _via.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            await testAction.ShouldThrowAsync<StrictNotSatisfiedException>();
            
            _callStream
                .To(x => x.EchoAsync("test"))
                .ForEach(call =>
                {
                    call.CallInfo.Arguments[0].ShouldBe("test");
                    call.Returned!.Exception.ShouldBeOfType<StrictNotSatisfiedException>();
                })
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecord_WithStrict_WhenMatchingRedirect_Redirects()
        {
            // ARRANGE
            _via.Strict();
            _via
                .To(x => x.EchoAsync("test"))
                .Redirect<(string input, __)>(async args => await _via.Relay.Next.EchoAsync(args.input) + " diverted");

            // ACT
            var result = await _via.Proxy(new Foo()).EchoAsync("test");

            // ASSERT
            result.ShouldBe("original: test diverted");

            _callStream
                .To(x => x.EchoAsync("test"))
                .ForEach(call =>
                {
                    call.Args<string>().ShouldBe("test");
                    call.Returned!.Value.Result.ShouldBe(result);
                })
                .Count.ShouldBe(1);
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
            _callStream
                .To(x => x.SetName(Is<string>.Any))
                .Select((call, i) =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    return call.Args((string name) => name.ShouldBe(inputs[i]));
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
                .Select(x => fooProxy.Name).ToList();

            // ASSERT
            _callStream
                .To(x => x.Name)
                .ForEach(call =>
                {
                    outputs.ShouldContain(call.Returned!.Value);
                })
                .Select(call => call.Returned!.Value)
                .ShouldBe(outputs);
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
            _callStream
                .ToSet(x => x.Name, () => Is<string>.Any)
                .ForEach((call, i) =>
                {
                    call.Args((string name) => name.ShouldBe(inputs[i]));
                    call.Returned!.Value.ShouldBeNull();
                })
                .Select(call => call.Args<string>())
                .ShouldBe(inputs);
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldRecord2()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();

            var echosa = _via
                .To(x => x.Echo(Is<string>.Any))
                .Record(call => call.Args((string input) => new
                {
                    input,
                    call.Returned!.Value
                }));
            
            var d = _via
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            var fooProxy = _via.Proxy();

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            _callStream.Select(x => x.CallInfo.Arguments[0]).ShouldBe(inputs);
            _callStream.Select(x => x.Returned?.Value).ShouldBe(outputs);

            var echoCalls = _callStream.To(x => x.Echo(Is<string>.Any));
            echoCalls.Select(call => call.Args((string input) => input)).ShouldBe(inputs);
            echoCalls.Select(call => call.Returned!.Value).ShouldBe(outputs);
            
            _callStream
                .To(x => x.Echo(Is<string>.Any))
                .Select((call, i) =>
                {
                    call.Args((string input) =>
                    {
                        input.ShouldBe(inputs[i]);
                    });
                    
                    return call.Returned!.Value;
                }).ShouldBe(outputs);
        }
    }
}