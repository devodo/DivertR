using Divertr.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiversionTests
    {
        private readonly Diversion<IFoo> _diversion = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var proxy = _diversion.Proxy(original);
            
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
            var subject = _diversion.Proxy(original);
            _diversion.Redirect(new FooSubstitute(" world", _diversion.CallCtx.Root));

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
            var subject = _diversion.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_diversion.CallCtx.Root.Message} world");

            _diversion.Redirect(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void MultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var subject = _diversion.Proxy(new Foo("hello world"));

            // ACT
            _diversion
                .AddRedirect(new FooSubstitute(" me", _diversion.CallCtx.Next))
                .AddRedirect(new FooSubstitute(" me", _diversion.CallCtx.Next))
                .AddRedirect(new FooSubstitute(" again", _diversion.CallCtx.Next));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _diversion.Proxy(original);

            // ACT
            _diversion.AddRedirect(new FooSubstitute(" me", _diversion.CallCtx.Next));
            _diversion.Reset();
            _diversion.AddRedirect(new FooSubstitute(" again", _diversion.CallCtx.Next));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _diversion.Proxy(original);

            // ACT
            _diversion.AddRedirect(new FooSubstitute(" me", _diversion.CallCtx.Next));
            _diversion.AddRedirect(new FooSubstitute(" again", _diversion.CallCtx.Next));
            _diversion.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
