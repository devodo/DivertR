using DivertR.DynamicProxy;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefInTests
    {
        private delegate int InCall(in int input);

        private readonly Via<INumberIn> _via = new();
        private readonly ICallRecord<INumberIn> _callRecord;
        
        static ByRefInTests()
        {
            DiverterSettings.Default.ProxyFactory = new DynamicProxyFactory();
        }

        public ByRefInTests()
        {
            _callRecord = _via.RecordCalls();
        }
        
        [Fact]
        public void GivenInRedirect_ShouldDivert()
        {
            // ARRANGE
            _via
                .Redirect(x => x.GetNumber(in Is<int>.AnyRef))
                .To(new InCall((in int i) => _via.Next.GetNumber(i) + 10));
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int input = 3;
            var result = viaProxy.GetNumber(in input);

            // ASSERT
            result.ShouldBe(13);
        }
        
        [Fact]
        public void GivenInRedirect_WhenParamValueMatches_ShouldDivert()
        {
            // ARRANGE
            const int input = 3;
            int inParam = input;
            _via
                .Redirect(x => x.GetNumber(in inParam))
                .To(new InCall((in int i) => _via.Next.GetNumber(i) + 10));
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int callParam = input;
            var result = viaProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(input + 10);
        }
        
        [Fact]
        public void GivenInRedirect_WhenParamValueDoesNotMatches_ShouldDefaultToOriginal()
        {
            // ARRANGE
            int inParam = 4;
            _via
                .Redirect(x => x.GetNumber(in inParam))
                .To(new InCall((in int i) => _via.Next.GetNumber(i) + 10));
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int callParam = 3;
            var result = viaProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(callParam);
        }
    }
}