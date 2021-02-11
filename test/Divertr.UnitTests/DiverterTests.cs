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
            var director = _diverter.For<IFoo>();
            var subject = director.Proxy(original);
            
            director.AddRedirect(new FooSubstitute(" me", director.CallCtx.Replaced));
            director.AddRedirect(new FooSubstitute(" again", director.CallCtx.Replaced));

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
