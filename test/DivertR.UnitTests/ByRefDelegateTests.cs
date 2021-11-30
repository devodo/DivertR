using System.Linq;
using DivertR.DynamicProxy;
using DivertR.Record;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefDelegateTestsDynamicProxy : ByRefTests
    {
        private static readonly DiverterSettings DiverterSettings = new(new DynamicProxyFactory());

        public ByRefDelegateTestsDynamicProxy()
            : base(new ViaSet(DiverterSettings))
        {
        }
    }
    
    public class ByRefDelegateTests
    {
        private delegate int RefCall(ref int input);
        
        private delegate void RefArrayCall(ref int[] input);
        
        private delegate void OutCall(int input, out int output);

        private readonly IVia<INumber> _via;

        public ByRefDelegateTests() : this(Via.For<INumber>())
        {
        }

        protected ByRefDelegateTests(IVia<INumber> via)
        {
            _via = via;
        }
        
        [Fact]
        public void GivenRefParameterDelegateRedirect_ShouldUpdateRefInput()
        {
            // ARRANGE
            _via
                .To(x => x.RefNumber(ref IsRef<int>.Match(m => m == 3).Value))
                .Redirect(new RefCall((ref int i) =>
                {
                    var refIn = _via.Relay.Next.RefNumber(ref i);
                    i += 10;

                    return refIn;
                }));
            
            var viaProxy = _via.Proxy(new Number(i => i * 2));
            
            // ACT
            int input = 3;
            var result = viaProxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(3);
            input.ShouldBe(16);
        }
        
        [Fact]
        public void GivenRefParameterRedirect_ShouldUpdateRefInput()
        {
            // ARRANGE
            _via
                .To(x => x.RefNumber(ref IsRef<int>.Match(m => m == 3).Value))
                .Redirect<(Ref<int> i, __)>((call, args) =>
                {
                    var refIn = call.Next.RefNumber(ref args.i.Value);
                    args.i.Value += 10;

                    return refIn;
                });
            
            var viaProxy = _via.Proxy(new Number(i => i * 2));
            
            // ACT
            int input = 3;
            var result = viaProxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(3);
            input.ShouldBe(16);
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
                .Redirect<(int i, Ref<int> o)>((call, args) =>
                {
                    call.Next.OutNumber(args.i, out args.o.Value);
                    args.o.Value += 10;
                });
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
        }
        
        [Fact]
        public void GivenOutParameterDelegateRedirect_ShouldUpdateOutParameter()
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
        public void GivenOutParameterDelegateRedirect_ShouldRecord()
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
            recordStream.Scan(call =>
            {
                call.Args[0].ShouldBe(3);
                call.Args[1].ShouldBe(13);
            }).ShouldBe(1);
        }
        
        [Fact]
        public void GivenOutParameterRedirect_ShouldRecord()
        {
            // ARRANGE
            var recordStream = _via
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect<(int i, Ref<int> o)>((call, args) =>
                {
                    call.Next.OutNumber(args.i, out args.o.Value);
                    args.o.Value += 10;
                })
                .Record();
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
            recordStream.Count.ShouldBe(1);
            recordStream.Scan(call =>
            {
                call.Args.i.ShouldBe(3);
                call.Args.o.Value.ShouldBe(13);
            }).ShouldBe(1);
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
        public void GivenRefDelegate_ShouldRedirect()
        {
            // ARRANGE
            var via = Via.For<INumber>();
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
        public void GivenRefParameter_WhenParameterlessDelegateRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = Via.For<INumber>();
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