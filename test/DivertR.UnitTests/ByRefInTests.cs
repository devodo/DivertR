using DivertR.DynamicProxy;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefInTests
    {
        // Due to a known issue DispatchProxy does not currently support in byref parameters
        // https://github.com/dotnet/runtime/issues/47522
        private static readonly DiverterSettings DiverterSettings = new(new DynamicProxyFactory());
        
        private delegate int InCall(in int input);

        private readonly Via<INumberIn> _via = new(DiverterSettings);

        [Fact]
        public void GivenInRedirect_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.GetNumber(in Is<int>.AnyRef))
                .Redirect(new InCall((in int i) => _via.Relay.Next.GetNumber(i) + 10));
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int input = 3;
            var result = viaProxy.GetNumber(in input);

            // ASSERT
            result.ShouldBe(13);
        }
        
        [Fact]
        public void GivenInRedirect_WhenParamValueMatches_ShouldRedirect()
        {
            // ARRANGE
            const int input = 3;
            int inParam = input;
            _via
                .To(x => x.GetNumber(in inParam))
                .Redirect(new InCall((in int i) => _via.Relay.Next.GetNumber(i) + 10));
            
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
                .To(x => x.GetNumber(in inParam))
                .Redirect(new InCall((in int i) => _via.Relay.Next.GetNumber(i) + 10));
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int callParam = 3;
            var result = viaProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(callParam);
        }
    }
}