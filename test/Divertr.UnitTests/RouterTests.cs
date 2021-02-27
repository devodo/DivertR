using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RouterTests
    {
        private readonly Router<IFoo> _router = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _router.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _router.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _router.Redirect(foo);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(foo.Message);
        }
        
        [Fact]
        public void GivenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _router.Proxy(original);
            _router.Redirect(new Foo("diverted"));
            _router.Reset();

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _router.Proxy(original);
            _router.Redirect(new Foo(() => $"hello {_router.Relay.Original.Message}"));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _router.Proxy(original);
            _router.Redirect(new Foo(() => $"hello {_router.Relay.Next.Message}"));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var subject = _router.Proxy(original);
            IFoo originalReference = null;
            _router.Redirect(new Foo(() =>
            {
                originalReference = _router.Relay.OriginalInstance;
                return $"hello {originalReference!.Message}";
            }));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMockedRedirect_ShouldDivert()
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
        public void GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var subject = _router.Proxy(new Foo("hello foo"));

            // ACT
            var next = _router.Relay.Next;
            _router
                .AddRedirect(new Foo(() => $"{next.Message} me"))
                .AddRedirect(new Foo(() => $"{next.Message} here"))
                .AddRedirect(new Foo(() => $"{next.Message} again"));
            
            // ASSERT
            subject.Message.ShouldBe("hello foo me here again");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            var subject = _router.Proxy(new Foo("foo"));

            // ACT
            var next = _router.Relay.Next;
            var orig = _router.Relay.Original;
            _router
                .AddRedirect(new Foo(() => $"{orig.Message} 1 {next.Message}"))
                .AddRedirect(new Foo(() => $"{orig.Message} 2 {next.Message}"))
                .AddRedirect(new Foo(() => $"{orig.Message} 3 {next.Message}"));
            
            // ASSERT
            subject.Message.ShouldBe("foo 3 foo 2 foo 1 foo");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var subject = _router.Proxy(new Foo("foo"));

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
            subject.Message.ShouldBe("3 2 1 foo 1 2 3");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var subject = _router.Proxy(original);

            // ACT
            _router.AddRedirect(new Foo(() => $"{_router.Relay.Next.Message} me"));
            _router.Reset();
            _router.AddRedirect(new Foo(() => $"{_router.Relay.Next.Message} again"));

            // ASSERT
            subject.Message.ShouldBe("hello foo again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var subject = _router.Proxy(original);

            // ACT
            _router.AddRedirect(new Foo(() => $"{_router.Relay.Next.Message} me"));
            _router.AddRedirect(new Foo(() => $"{_router.Relay.Next.Message} again"));
            _router.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
