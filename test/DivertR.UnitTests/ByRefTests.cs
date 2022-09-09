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
            : base(new ViaSet(DiverterSettings))
        {
        }
        
        [Fact]
        public void GivenOutRedirect_WhenException_ShouldWriteBackRefs()
        {
            // ARRANGE
            var via = ViaSet.Via<INumber>();
            via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    throw new Exception();
                });

            var viaProxy = via.Proxy(new Number(x => x * 2));

            // ACT
            var output = 0;
            Action testAction = () => viaProxy.OutNumber(3, out output);

            // ASSERT
            testAction.ShouldThrow<Exception>();
            output.ShouldBe(6);
        }
    }
    
    public class ByRefTests
    {
        protected readonly IViaSet ViaSet;
        private readonly IVia<INumber> _via;
        private readonly INumber _proxy;

        public ByRefTests() : this(new ViaSet())
        {
        }

        protected ByRefTests(IViaSet viaSet)
        {
            ViaSet = viaSet;
            _via = viaSet.Via<INumber>();
            _proxy = _via.Proxy(new Number(i => i + 100));
        }

        [Fact]
        public void GivenRefParameterRedirect_ShouldUpdateRefInput()
        {
            // ARRANGE
            _via
                .To(x => x.RefNumber(ref IsRef<int>.Match(m => m == 3).Value))
                .Redirect<(Ref<int> input, __)>(call =>
                {
                    var relayResult = _via.Relay.Next.RefNumber(ref call.Args.input.Value);
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
        public void GivenRefParameterTargetRedirect_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _via.Retarget(test);

            // ACT
            int input = 5;
            var proxy = _via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(test.GetNumber(5));
        }
        
        [Fact]
        public void GivenOutParameterRedirect_ShouldUpdateOutParameter()
        {
            // ARRANGE
            _via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    call.Args.output.Value += 10;
                });
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
        }

        [Fact]
        public void GivenOutParameterTargetRedirect_ShouldUpdateOutParameter()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _via.Retarget(test);
            
            var viaProxy = _via.Proxy(new Number());
            
            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(6);
        }
        
        [Fact]
        public void GivenOutParameterRedirect_ShouldRecordCallArguments()
        {
            // ARRANGE
            var recordStream = _via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect<(int input, Ref<int> output)>(call =>
                {
                    call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value);

                    call.Args.output.Value += 10;
                })
                .Record();
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

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
        public void GivenRefArrayParameterTargetRedirect_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _via.Retarget(test);

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _via.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x * 2));
        }
        
        [Fact]
        public void GivenRefArrayParameterRedirect_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.RefArrayNumber(ref IsRef<int[]>.Any))
                .Redirect<(Ref<int[]> inRef, __)>(call =>
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
            var proxy = _via.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x + 10));
        }
        
        [Fact]
        public void GivenRefArrayParameterDelegateRedirect_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.RefArrayNumber(ref IsRef<int[]>.Any))
                .Redirect<(Ref<int[]> inRef, __)>(call =>
                {
                    _via.Relay.Next.RefArrayNumber(ref call.Args.inRef.Value);

                    for (var i = 0; i < call.Args.inRef.Value.Length; i++)
                    {
                        call.Args.inRef.Value[i] += 10;
                    }
                });

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _via.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x + 10));
        }
        
        [Fact]
        public void GivenRefRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            int input = 5;
            via
                .To(x => x.RefNumber(ref input))
                .Redirect<(Ref<int> i, __)>(call =>
                {
                    var refIn = call.Args.i.Value;
                    call.Args.i.Value = 50;

                    return refIn + 1;
                });

            // ACT
            var i2 = 5;
            var proxy = via.Proxy(new Number());
            var result = proxy.RefNumber(ref i2);

            // ASSERT
            result.ShouldBe(6);
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .To(x => x.RefNumber(ref IsRef<int>.Any))
                .Redirect(_ => 10);

            // ACT
            var input = 5;
            var proxy = via.Proxy(new Number());
            var result = proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(10);
            input.ShouldBe(5);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessDelegateRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .To(x => x.RefNumber(ref IsRef<int>.Any))
                .Redirect(() => 10);

            // ACT
            var input = 5;
            var proxy = via.Proxy(new Number());
            var result = proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(10);
            input.ShouldBe(5);
        }
        
        [Fact]
        public void GivenCallNextRedirect_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Redirect<(Ref<int> input, Ref<int> output)>(call =>
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
        public void GivenCallNextRedirectWithArgs_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Redirect<(Ref<int> input, Ref<int> output)>(call =>
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
        public void GivenRedirectValueTupleWithTypeAndDiscards_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.RefOutNumber(ref IsRef<int>.Any, out IsRef<int>.Any))
                .Redirect<(__, Ref<int> output, __)>(call =>
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