using DivertR.DynamicProxy;
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

        private readonly IVia<INumberIn> _via = new ViaSet(DiverterSettings).Via<INumberIn>();

        [Fact]
        public void GivenInRedirect_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.GetNumber(in IsRef<int>.Any))
                .Redirect<(Ref<int> input, __)>(call => _via.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
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
                .Redirect<(Ref<int> input, __)>(call => _via.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int callParam = input;
            var result = viaProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(input + 10);
        }
        
        [Fact]
        public void GivenInRedirect_WhenParamValueDoesNotMatches_ShouldDefaultToRoot()
        {
            // ARRANGE
            int inParam = 4;
            _via
                .To(x => x.GetNumber(in inParam))
                .Redirect<(Ref<int> input, __)>(call => _via.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
            var viaProxy = _via.Proxy(new NumberIn());

            // ACT
            int callParam = 3;
            var result = viaProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(callParam);
        }
    }
}