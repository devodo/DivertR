using Divertr.UnitTests.Model;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverterCollection _diverters = new DiverterCollection();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var diversion = _diverters.Of<IFoo>();
            var subject = diversion.Proxy(original);
            
            diversion.AddSendTo(new FooSubstitute(" me", diversion.CallCtx.Next));
            diversion.AddSendTo(new FooSubstitute(" again", diversion.CallCtx.Next));

            // ACT
            _diverters.ResetAll();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void RedirectShortHand_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _diverters.Of<IFoo>().Proxy(original);

            // ACT
            _diverters.Of<IFoo>().SendTo(new FooSubstitute(" diverted", _diverters.Of<IFoo>().CallCtx.Next));
            
            // ASSERT
            subject.Message.ShouldBe(original.Message + " diverted");
        }
    }
}
