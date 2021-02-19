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
            var router = _diverter.Router<IFoo>();
            var subject = router.Proxy(original);
            
            router.AddRedirect(new FooSubstitute(" me", router.Relay.Next));
            router.AddRedirect(new FooSubstitute(" again", router.Relay.Next));

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
            var subject = _diverter.Router<IFoo>().Proxy(original);

            // ACT
            _diverter.Router<IFoo>().Redirect(new FooSubstitute(" diverted", _diverter.Router<IFoo>().Relay.Next));
            
            // ASSERT
            subject.Message.ShouldBe(original.Message + " diverted");
        }
    }
}
