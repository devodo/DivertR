using System;
using System.Linq;
using DivertR.DynamicProxy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefTestsDynamicProxy : ByRefTests
    {
        private static readonly DiverterSettings DiverterSettings = new(new DynamicProxyFactory());

        public ByRefTestsDynamicProxy()
            : base(new RedirectSet(DiverterSettings))
        {
        }
        
        [Fact]
        public void GivenOutParameterVia_WhenException_ShouldWriteBackRefs()
        {
            // ARRANGE
            var redirect = RedirectSet.GetOrCreate<INumber>();
            redirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    throw new Exception();
                });

            var redirectProxy = redirect.Proxy(new Number(x => x * 2));

            // ACT
            var output = 0;
            Action testAction = () => redirectProxy.OutNumber(3, out output);

            // ASSERT
            testAction.ShouldThrow<Exception>();
            output.ShouldBe(6);
        }
    }
    
    public class ByRefTests
    {
        protected readonly IRedirectSet RedirectSet;
        private readonly IRedirect<INumber> _redirect;
        private readonly INumber _proxy;

        public ByRefTests() : this(new RedirectSet())
        {
        }

        protected ByRefTests(IRedirectSet redirectSet)
        {
            RedirectSet = redirectSet;
            _redirect = redirectSet.GetOrCreate<INumber>();
            _proxy = _redirect.Proxy(new Number(i => i + 100));
        }

        [Fact]
        public void GivenRefParameterVia_ShouldUpdateRefInput()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefNumber(ref IsRef<int>.Match(m => m == 3).Value))
                .Via<(Ref<int> input, __)>(call =>
                {
                    var relayResult = _redirect.Relay.Next.RefNumber(ref call.Args.input.Value);
                    call.Args.input.Value += 10;

                    return relayResult;
                });

            // ACT
            int input = 3;
            var result = _proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(3);
            input.ShouldBe(113);
        }
        
        [Fact]
        public void GivenRefParameterRetarget_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);

            // ACT
            int input = 5;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(test.GetNumber(5));
        }
        
        [Fact]
        public void GivenOutParameterVia_ShouldUpdateOutParameter()
        {
            // ARRANGE
            _redirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    call.Args.output.Value += 10;
                });
            
            var redirectProxy = _redirect.Proxy(new Number());

            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
        }

        [Fact]
        public void GivenOutParameterRetarget_ShouldUpdateOutParameter()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);
            
            var redirectProxy = _redirect.Proxy(new Number());
            
            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(6);
        }
        
        [Fact]
        public void GivenOutParameterVia_ShouldRecordCallArguments()
        {
            // ARRANGE
            var recordStream = _redirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    call.Args.output.Value += 10;
                })
                .Record();
            
            var redirectProxy = _redirect.Proxy(new Number());

            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
            foreach (var call in recordStream)
            {
                call.Args.input.ShouldBe(3);
                call.Args.output.Value.ShouldBe(13);
            }

            recordStream.Verify(call =>
            {
                call.Args.input.ShouldBe(3);
                call.Args.output.Value.ShouldBe(13);
            }).Count.ShouldBe(1);
        }

        [Fact]
        public void GivenRefArrayParameterRetarget_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x * 2));
        }
        
        [Fact]
        public void GivenRefArrayParameterVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefArrayNumber(ref IsRef<int[]>.Any))
                .Via<(Ref<int[]> inRef, __)>(call =>
                {
                    call.Relay.Next.RefArrayNumber(ref call.Args.inRef.Value);

                    for (var i = 0; i < call.Args.inRef.Value.Length; i++)
                    {
                        call.Args.inRef.Value[i] += 10;
                    }
                });

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x + 10));
        }
        
        [Fact]
        public void GivenRefArrayParameterDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefArrayNumber(ref IsRef<int[]>.Any))
                .Via<(Ref<int[]> inRef, __)>(call =>
                {
                    _redirect.Relay.Next.RefArrayNumber(ref call.Args.inRef.Value);

                    for (var i = 0; i < call.Args.inRef.Value.Length; i++)
                    {
                        call.Args.inRef.Value[i] += 10;
                    }
                });

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x + 10));
        }
        
        [Fact]
        public void GivenRefParameterVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            int input = 5;
            redirect
                .To(x => x.RefNumber(ref input))
                .Via<(Ref<int> i, __)>(call =>
                {
                    var refIn = call.Args.i.Value;
                    call.Args.i.Value = 50;

                    return refIn + 1;
                });

            // ACT
            var i2 = 5;
            var proxy = redirect.Proxy(new Number());
            var result = proxy.RefNumber(ref i2);

            // ASSERT
            result.ShouldBe(6);
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            redirect
                .To(x => x.RefNumber(ref IsRef<int>.Any))
                .Via(_ => 10);

            // ACT
            var input = 5;
            var proxy = redirect.Proxy(new Number());
            var result = proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(10);
            input.ShouldBe(5);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            redirect
                .To(x => x.RefNumber(ref IsRef<int>.Any))
                .Via(() => 10);

            // ACT
            var input = 5;
            var proxy = redirect.Proxy(new Number());
            var result = proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(10);
            input.ShouldBe(5);
        }
        
        [Fact]
        public void GivenCallNextVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Via<(Ref<int> input, Ref<int> output)>(call =>
                {
                    var result = call.CallNext();

                    call.Args.input.Value = (int) call.CallInfo.Arguments[0] + 1;
                    call.Args.output.Value = (int) call.CallInfo.Arguments[1] + 2;
                    
                    return result + 10;
                });

            // ACT
            int input = 5;
            int output;
            var result = _proxy.RefOutNumber(ref input, out output);
            
            // ASSERT
            result.ShouldBe(15);
            input.ShouldBe(6);
            output.ShouldBe(107);
        }
        
        [Fact]
        public void GivenCallNextViaWithArgs_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Via<(Ref<int> input, Ref<int> output)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    var result = call.CallNext(args);

                    call.Args.input.Value = (int) args[0] + 1;
                    call.Args.output.Value = (int) args[1] + 2;
                    
                    return result + 10;
                });

            // ACT
            int input = 5;
            int output;
            var result = _proxy.RefOutNumber(ref input, out output);
            
            // ASSERT
            result.ShouldBe(15);
            input.ShouldBe(6);
            output.ShouldBe(107);
        }
        
        [Fact]
        public void GivenViaValueTupleWithTypeAndDiscards_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Via<(__, Ref<int> output, __)>(call =>
                {
                    var input = (int) call.CallInfo.Arguments[0];
                    var result = call.Next.RefOutNumber(ref input, out call.Args.output.Value);
                    call.Args.output.Value += 1;

                    return result + 10;
                });

            // ACT
            int input = 5;
            int output;
            var result = _proxy.RefOutNumber(ref input, out output);
            
            // ASSERT
            result.ShouldBe(input + 10);
            output.ShouldBe(input + 101);
        }
    }
}