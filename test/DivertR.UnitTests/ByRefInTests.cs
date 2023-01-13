using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefInTests
    {
#if NET6_0_OR_GREATER
        private static readonly DiverterSettings DiverterSettings = new();
#else
        // Due to a known issue DispatchProxy does not support in byref parameters prior to .NET 6
        // https://github.com/dotnet/runtime/issues/47522
        private static readonly DiverterSettings DiverterSettings = new(proxyFactory: new DynamicProxy.DynamicProxyFactory());
#endif
        private readonly IRedirect<INumberIn> _redirect = new RedirectSet(DiverterSettings).GetOrCreate<INumberIn>();

        [Fact]
        public void GivenInParameterVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.GetNumber(in IsRef<int>.Any))
                .Via<(Ref<int> input, __)>(call => _redirect.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
            var redirectProxy = _redirect.Proxy(new NumberIn());

            // ACT
            int input = 3;
            var result = redirectProxy.GetNumber(in input);

            // ASSERT
            result.ShouldBe(13);
        }
        
        [Fact]
        public void GivenInParameterVia_WhenParamValueMatches_ShouldRedirect()
        {
            // ARRANGE
            const int input = 3;
            int inParam = input;
            _redirect
                .To(x => x.GetNumber(in inParam))
                .Via<(Ref<int> input, __)>(call => _redirect.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
            var redirectProxy = _redirect.Proxy(new NumberIn());

            // ACT
            int callParam = input;
            var result = redirectProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(input + 10);
        }
        
        [Fact]
        public void GivenInParameterVia_WhenParamValueDoesNotMatches_ShouldDefaultToRoot()
        {
            // ARRANGE
            int inParam = 4;
            _redirect
                .To(x => x.GetNumber(in inParam))
                .Via<(Ref<int> input, __)>(call => _redirect.Relay.Next.GetNumber(call.Args.input.Value) + 10);
            
            var redirectProxy = _redirect.Proxy(new NumberIn());

            // ACT
            int callParam = 3;
            var result = redirectProxy.GetNumber(in callParam);

            // ASSERT
            result.ShouldBe(callParam);
        }
    }
}