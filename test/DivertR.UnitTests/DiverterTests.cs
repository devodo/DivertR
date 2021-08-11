using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverter _diverter = new Diverter();

        public DiverterTests()
        {
            _diverter.Register<IFoo>();
        }
        
        [Fact]
        public void GivenRedirectWithRelay_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var via = _diverter.Via<IFoo>().Proxy(original);

            // ACT
            _diverter.Via<IFoo>().Redirect(new FooAlt(() => $"{_diverter.Via<IFoo>().Relay.Next.Name} diverted"));
            
            // ASSERT
            via.Name.ShouldBe(original.Name + " diverted");
        }

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);
            
            via.Redirect(new FooAlt(() => $"{via.Relay.Next} me"));
            via.Redirect(new FooAlt(() => $"{via.Relay.Next} again"));

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
    }
}
