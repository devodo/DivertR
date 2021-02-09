using Divertr.UnitTests.Model;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterSetTests
    {
        private readonly IDiverterSet _diverters = new DiverterSet();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var diverter = _diverters.Get<IFoo>();
            var subject = diverter.Proxy(original);
            
            diverter.AddRedirect(new FooSubstitute(" me", diverter.CallCtx.Replaced));
            diverter.AddRedirect(new FooSubstitute(" again", diverter.CallCtx.Replaced));

            // ACT
            _diverters.ResetAll();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
