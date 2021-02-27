using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverter _diverter = new Diverter();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var router = _diverter.Router<IFoo>();
            var subject = router.Proxy(original);
            
            router.AddRedirect(new Foo(() => $"{router.Relay.Next} me"));
            router.AddRedirect(new Foo(() => $"{router.Relay.Next} again"));

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
            _diverter.Router<IFoo>().Redirect(new Foo(() => $"{_diverter.Router<IFoo>().Relay.Next.Message} diverted"));
            
            // ASSERT
            subject.Message.ShouldBe(original.Message + " diverted");
        }
    }
}
