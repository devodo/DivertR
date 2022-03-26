using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DefaultProxyTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenProxyWithDefaultRoot_WhenStringPropertyGetterCalled_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBeNull();
        }
        
        [Fact]
        public void GivenProxyWithDefaultRoot_WhenValueTypeReturnMethodCalled_ShouldReturnDefault()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric(5);

            // ASSERT
            result.ShouldBe(default);
        }
    }
}