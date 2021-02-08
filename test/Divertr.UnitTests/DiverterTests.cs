using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterTests
    {
        private readonly Diverter<IFoo> _diverter = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var proxy = _diverter.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenDivertedProxy_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var subject = _diverter.Proxy(original);
            _diverter.Redirect(new SubstituteTest(" world", _diverter.CallCtx.Original));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMockedDivert_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var subject = _diverter.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_diverter.CallCtx.Original.Message} world");

            _diverter.Redirect(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void MultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var subject = _diverter.Proxy(new Foo("hello world"));

            // ACT
            _diverter
                .AddRedirect(new SubstituteTest(" me", _diverter.CallCtx.Replaced))
                .AddRedirect(new SubstituteTest(" me", _diverter.CallCtx.Replaced))
                .AddRedirect(new SubstituteTest(" again", _diverter.CallCtx.Replaced));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _diverter.Proxy(original);

            // ACT
            _diverter.AddRedirect(new SubstituteTest(" me", _diverter.CallCtx.Replaced));
            _diverter.Reset();
            _diverter.AddRedirect(new SubstituteTest(" again", _diverter.CallCtx.Replaced));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _diverter.Proxy(original);

            // ACT
            _diverter.AddRedirect(new SubstituteTest(" me", _diverter.CallCtx.Replaced));
            _diverter.AddRedirect(new SubstituteTest(" again", _diverter.CallCtx.Replaced));
            _diverter.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
