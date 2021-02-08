using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterSetTests
    {
        private readonly IDiverterSet _diverterSet = new DiverterSet();

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var diverter = _diverterSet.Get<IFoo>();
            var subject = diverter.Proxy(original);
            
            diverter.AddRedirect(new SubstituteTest(" me", diverter.CallCtx.Replaced));
            diverter.AddRedirect(new SubstituteTest(" again", diverter.CallCtx.Replaced));

            // ACT
            _diverterSet.ResetAll();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
