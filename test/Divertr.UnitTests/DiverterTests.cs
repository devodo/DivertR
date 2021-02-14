using Divertr.UnitTests.Model;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverter _diverter = new Diverter();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var diversion = _diverter.Of<IFoo>();
            var subject = diversion.Proxy(original);
            
            diversion.AddRedirect(new FooSubstitute(" me", diversion.CallCtx.Next));
            diversion.AddRedirect(new FooSubstitute(" again", diversion.CallCtx.Next));

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
            var subject = _diverter.Of<IFoo>().Proxy(original);

            // ACT
            _diverter.Of<IFoo>().Redirect(new FooSubstitute(" diverted", _diverter.Of<IFoo>().CallCtx.Next));
            
            // ASSERT
            subject.Message.ShouldBe(original.Message + " diverted");
        }
    }
}
