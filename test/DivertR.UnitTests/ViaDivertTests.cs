using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaDivertTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public ViaDivertTests()
        {
            _proxy = _via.Proxy(new Foo());
        }
        
        [Fact]
        public void GivenDivert_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via.Divert(x => x.GetFoo());

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe(_proxy.Name);
        }

        [Fact]
        public void GivenDivertRedirect_ShouldDivertAndRedirect()
        {
            // ARRANGE
            var fooVia = _via.Divert(x => x.GetFoo());
            fooVia.To(x => x.Name).Redirect("Diverted");

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
    }
}