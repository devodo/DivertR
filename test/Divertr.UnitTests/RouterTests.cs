using Divertr.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class RouterTests
    {
        private readonly Router<IFoo> _router = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var proxy = _router.Proxy(original);
            
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
            var subject = _router.Proxy(original);
            _router.Redirect(new FooSubstitute(" world", _router.Relay.Original));

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
            var subject = _router.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_router.Relay.Original.Message} world");

            _router.Redirect(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void MultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var subject = _router.Proxy(new Foo("hello world"));

            // ACT
            _router
                .AddRedirect(new FooSubstitute(" me", _router.Relay.Next))
                .AddRedirect(new FooSubstitute(" me", _router.Relay.Next))
                .AddRedirect(new FooSubstitute(" again", _router.Relay.Next));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void MultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var subject = _router.Proxy(new Foo("original foo"));

            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => 
                    $"{_router.Relay.State} {_router.Relay.Next.Message} {_router.Relay.State}");

            // ACT
            _router
                .AddRedirect(mock.Object, "1")
                .AddRedirect(mock.Object, "2")
                .AddRedirect(mock.Object, "3");
            
            // ASSERT
            subject.Message.ShouldBe("3 2 1 original foo 1 2 3");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _router.Proxy(original);

            // ACT
            _router.AddRedirect(new FooSubstitute(" me", _router.Relay.Next));
            _router.Reset();
            _router.AddRedirect(new FooSubstitute(" again", _router.Relay.Next));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _router.Proxy(original);

            // ACT
            _router.AddRedirect(new FooSubstitute(" me", _router.Relay.Next));
            _router.AddRedirect(new FooSubstitute(" again", _router.Relay.Next));
            _router.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
