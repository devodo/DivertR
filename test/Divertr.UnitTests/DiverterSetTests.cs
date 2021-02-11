using Divertr.UnitTests.Model;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterSetTests
    {
        private readonly IDiverterSet _diverter = new DiverterSet();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var diverter = _diverter.Get<IFoo>();
            var subject = diverter.Proxy(original);
            
            diverter.AddRedirect(new FooSubstitute(" me", diverter.CallCtx.Replaced));
            diverter.AddRedirect(new FooSubstitute(" again", diverter.CallCtx.Replaced));

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void RedirectShortHand_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _diverter.Proxy<IFoo>(original);

            // ACT
            _diverter.Redirect<IFoo>(new FooSubstitute(" diverted", _diverter.CallCtx<IFoo>().Replaced));
            
            // ASSERT
            subject.Message.ShouldBe(original.Message + " diverted");
        }
    }
}
