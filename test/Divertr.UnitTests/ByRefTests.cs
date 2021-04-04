using System.Diagnostics;
using DivertR.Core;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class ByRefTests
    {
        private readonly ITestOutputHelper _output;

        private delegate void RefCall(ref int input);

        private delegate void OutCall(out int input);

        private delegate int InCall(in int input);
        
        private readonly Via<INumber> _via = new();
        private readonly ICallRecord<INumber> _callRecord;

        public ByRefTests(ITestOutputHelper output)
        {
            _output = output;
            _callRecord = _via.RecordCalls();
        }
        
        [Fact]
        public void GivenRefRedirect_ShouldUpdateRefInput()
        {
            // ARRANGE
            _via
                .Redirect(x => x.RefNumber(ref Is<int>.AnyRef))
                .To(new RefCall((ref int i) =>
                {
                    _via.Next.RefNumber(ref i);

                    i += 10;
                }));
            
            var viaProxy = _via.Proxy(new Number(i => i * 2));
            

            // ACT
            const int epochs = 1000000;
            int input = 0;
            var test = new Number(i => i * 2);
            
            var clock = Stopwatch.StartNew();
            for (var i = 0; i < epochs; i++)
            {
                input = 3;
                test.RefNumber(ref input);
            }
            
            _output.WriteLine($"Base: {clock.ElapsedMilliseconds}");
            clock.Restart();
            
            for (var i = 0; i < epochs; i++)
            {
                input = 3;
                viaProxy.RefNumber(ref input);
            }
            
            _output.WriteLine($"Elapsed: {clock.ElapsedMilliseconds}");

            // ASSERT
            input.ShouldBe(16);
        }
        
        [Fact]
        public void GivenRefInputRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            var test = new Number(x => x * 2);
            via
                .Redirect()
                .To(test);

            // ACT
            int input = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(test.GetNumber(5));
        }
        
        [Fact]
        public void GivenRefArrayInputRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            var test = new Number(x => x * 2);
            via
                .Redirect()
                .To(test);

            // ACT
            int[] inputOriginal = {5};
            var input = inputOriginal;
            var proxy = via.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input[0].ShouldBe(test.GetNumber(5));
        }

        [Fact]
        public void GivenRefDelegate_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            int input = 5;
            via
                .Redirect(x => x.RefNumber(ref input))
                .To(new RefCall((ref int i2) =>
                {
                    i2 = 50;
                }));

            // ACT
            var i2 = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref i2);

            // ASSERT
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefDelegateWithAnyParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .Redirect(x => x.RefNumber(ref Is<int>.AnyRef))
                .To(new RefCall((ref int i2) =>
                {
                    i2 = 50;
                }));

            // ACT
            var input = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(50);
        }
    }
}