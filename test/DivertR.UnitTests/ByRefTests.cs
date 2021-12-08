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

        private delegate int RefCall(ref int input);
        
        private delegate void RefArrayCall(ref int[] input);
        
        private delegate void OutCall(int input, out int output);

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
        public void GivenOutParameterRedirectDelegate_ShouldUpdateOutParameter()
        {
            // ARRANGE
            _via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect(new OutCall((int i, out int o) =>
                {
                    _via.Relay.Next.OutNumber(i, out o);

                    o += 10;
                }));
            
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

            recordStream.Replay(call =>
            {
                call.Args.input.ShouldBe(3);
                call.Args.output.Value.ShouldBe(13);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenOutParameterRedirectDelegate_ShouldRecordCallArguments()
        {
            // ARRANGE
            var recordStream = _via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect(new OutCall((int i, out int o) =>
                {
                    _via.Relay.Next.OutNumber(i, out o);

                    o += 10;
                }))
                .Record();
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
            recordStream.Count.ShouldBe(1);
            recordStream.First().CallInfo.Arguments[0].ShouldBe(3);
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
                .Redirect(new RefArrayCall((ref int[] inRef) =>
                {
                    _via.Relay.Next.RefArrayNumber(ref inRef);

                    for (var i = 0; i < inRef.Length; i++)
                    {
                        inRef[i] += 10;
                    }
                }));

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
        public void GivenRefDelegate_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            int input = 5;
            via
                .To(x => x.RefNumber(ref input))
                .Redirect(new RefCall((ref int i) =>
                {
                    var refIn = i;
                    i = 50;

                    return refIn + 1;
                }));

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
    }
}