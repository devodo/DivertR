using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaDivertTests
    {
        private readonly IVia<IFoo> _via = Via.For<IFoo>();
        private readonly IFoo _proxy;

        public ViaDivertTests()
        {
            _proxy = _via.Proxy(new Foo());
        }
        
        [Fact]
        public void GivenDivert_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via.To(x => x.GetFoo()).Divert();

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe(_proxy.Name);
        }

        [Fact]
        public void GivenDivertRedirect_ShouldDivertAndRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenParentViaReset_DivertShouldStillRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.Reset();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenResetDivertViaGroup_ShouldResetDivert()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.Reset("nested");

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenResetAll_ShouldResetDivert()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.ResetAll();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
    }
}