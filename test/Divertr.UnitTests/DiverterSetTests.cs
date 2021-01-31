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
            var original = new TestA("hello world");
            var diverter = _diverterSet.Get<ITestSubject>();
            var subject = diverter.Proxy(original);
            
            diverter.AddRedirect(new SubstituteTest(" me", diverter.CallCtx));
            diverter.AddRedirect(new SubstituteTest(" again", diverter.CallCtx));

            // ACT
            _diverterSet.ResetAll();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
