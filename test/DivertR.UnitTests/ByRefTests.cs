using System.Linq;
using DivertR.DynamicProxy;
using DivertR.Record;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefTestsDynamicProxy : ByRefTests
    {
        private static readonly DiverterSettings DiverterSettings = new()
        {
            ProxyFactory = new DynamicProxyFactory()
        };

        public ByRefTestsDynamicProxy()
            : base(new Via<INumber>(DiverterSettings))
        {
        }
    }
    
    public class ByRefTests
    {
        private delegate void RefCall(ref int input);
        
        private delegate void RefArrayCall(ref int[] input);
        
        private delegate void OutCall(int input, out int output);

        private readonly Via<INumber> _via;
        private readonly ICallStream<INumber> _callStream;

        public ByRefTests() : this(new Via<INumber>())
        {
        }

        protected ByRefTests(Via<INumber> via)
        {
            _via = via;
            _callStream = _via.Record();
        }
        
        [Fact]
        public void GivenRefParameterRedirect_ShouldUpdateRefInput()
        {
            // ARRANGE
            _via
                .To(x => x.RefNumber(ref Is<int>.AnyRef))
                .Redirect(new RefCall((ref int i) =>
                {
                    _via.Next.RefNumber(ref i);

                    i += 10;
                }));
            
            var viaProxy = _via.Proxy(new Number(i => i * 2));
            
            // ACT
            int input = 3;
            viaProxy.RefNumber(ref input);

            // ASSERT
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
                .To(x => x.OutNumber(Is<int>.Any, out Is<int>.AnyRef))
                .Redirect(new OutCall((int i, out int o) =>
                {
                    _via.Next.OutNumber(i, out o);

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
        public void GivenOutParameterRedirect_ShouldLogCallArguments()
        {
            // ARRANGE
            _via
                .To(x => x.OutNumber(Is<int>.Any, out Is<int>.AnyRef))
                .Redirect(new OutCall((int i, out int o) =>
                {
                    _via.Next.OutNumber(i, out o);

                    o += 10;
                }));
            
            var viaProxy = _via.Proxy(new Number());

            // ACT
            viaProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
            _callStream.Count.ShouldBe(1);
            _callStream.First().CallInfo.Arguments[0].ShouldBe(3);
        }

        [Fact]
        public void GivenRefArrayParameterTargetRedirect_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _via.Retarget(test);

            // ACT
            int[] inputOriginal = {5, 8};
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
                .To(x => x.RefArrayNumber(ref Is<int[]>.AnyRef))
                .Redirect(new RefArrayCall((ref int[] inRef) =>
                {
                    _via.Next.RefArrayNumber(ref inRef);

                    for (var i = 0; i < inRef.Length; i++)
                    {
                        inRef[i] += 10;
                    }
                }));

            // ACT
            int[] inputOriginal = {5, 8};
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
            var via = new Via<INumber>();
            int input = 5;
            via
                .To(x => x.RefNumber(ref input))
                .Redirect(new RefCall((ref int i) =>
                {
                    i = 50;
                }));

            // ACT
            var i2 = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref i2);

            // ASSERT
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessDelegateRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .To(x => x.RefNumber(ref Is<int>.AnyRef))
                .Redirect(() => { });

            // ACT
            var input = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(5);
        }
    }
}