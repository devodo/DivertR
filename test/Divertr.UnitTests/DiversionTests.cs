using Divertr.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiversionTests
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
            _diverter.SendTo(new FooSubstitute(" world", _diverter.CallCtx.Root));

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
                .Returns(() => $"{_diverter.CallCtx.Root.Message} world");

            _diverter.SendTo(mock.Object);

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
                .AddSendTo(new FooSubstitute(" me", _diverter.CallCtx.Next))
                .AddSendTo(new FooSubstitute(" me", _diverter.CallCtx.Next))
                .AddSendTo(new FooSubstitute(" again", _diverter.CallCtx.Next));


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
            _diverter.AddSendTo(new FooSubstitute(" me", _diverter.CallCtx.Next));
            _diverter.Reset();
            _diverter.AddSendTo(new FooSubstitute(" again", _diverter.CallCtx.Next));

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
            _diverter.AddSendTo(new FooSubstitute(" me", _diverter.CallCtx.Next));
            _diverter.AddSendTo(new FooSubstitute(" again", _diverter.CallCtx.Next));
            _diverter.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
