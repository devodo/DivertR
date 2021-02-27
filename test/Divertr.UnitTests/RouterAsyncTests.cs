using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RouterAsyncTests
    {
        private readonly Router<IAsyncFoo> _router = new();
        
        [Fact]
        public async Task GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _router.Proxy(original);
            
            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
        
        [Fact]
        public async Task GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _router.Proxy(new AsyncFoo("hello foo"));
            var foo = new AsyncFoo("hi DivertR");
            _router.Redirect(foo);

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe(await foo.MessageAsync);
        }
        
        [Fact]
        public async Task GivenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new AsyncFoo("foo");
            var proxy = _router.Proxy(original);
            _router.Redirect(new AsyncFoo("diverted"));
            _router.Reset();

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new AsyncFoo("foo");
            var proxy = _router.Proxy(original);
            _router.Redirect(new AsyncFoo(async () => $"hello {await _router.Relay.Original.MessageAsync}"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new AsyncFoo("foo");
            var proxy = _router.Proxy(original);
            _router.Redirect(new AsyncFoo(async () => $"hello {await _router.Relay.Next.MessageAsync}"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new AsyncFoo("foo");
            var proxy = _router.Proxy(original);
            IAsyncFoo originalReference = null;
            _router.Redirect(new AsyncFoo(async () =>
            {
                originalReference = _router.Relay.OriginalInstance;
                return $"hello {await originalReference!.MessageAsync}";
            }));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public async Task GivenMockedRedirect_ShouldDivert()
        {
            // ARRANGE
            var original = new AsyncFoo("hello");
            var proxy = _router.Proxy(original);
            
            var mock = new Mock<IAsyncFoo>();
            mock
                .Setup(x => x.MessageAsync)
                .Returns(async () => $"{await _router.Relay.Original.MessageAsync} world");

            _router.Redirect(mock.Object);

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public async Task GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _router.Proxy(new AsyncFoo("hello foo"));
            var next = _router.Relay.Next;
            _router
                .AddRedirect(new AsyncFoo(async () => $"{await next.MessageAsync} me"))
                .AddRedirect(new AsyncFoo(async () => $"{await next.MessageAsync} here"))
                .AddRedirect(new AsyncFoo(async () => $"{await next.MessageAsync} again"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("hello foo me here again");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            var proxy = _router.Proxy(new AsyncFoo("foo"));
            var next = _router.Relay.Next;
            var orig = _router.Relay.Original;
            _router
                .AddRedirect(new AsyncFoo(async () => $"{await orig.MessageAsync} 1 {await next.MessageAsync}"))
                .AddRedirect(new AsyncFoo(async () => $"{await orig.MessageAsync} 2 {await next.MessageAsync}"))
                .AddRedirect(new AsyncFoo(async () => $"{await orig.MessageAsync} 3 {await next.MessageAsync}"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("foo 3 foo 2 foo 1 foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var proxy = _router.Proxy(new AsyncFoo("foo"));

            var mock = new Mock<IAsyncFoo>();
            mock
                .Setup(x => x.MessageAsync)
                .Returns(async () => 
                    $"{_router.Relay.State} {await _router.Relay.Next.MessageAsync} {_router.Relay.State}");
            
            _router
                .AddRedirect(mock.Object, "1")
                .AddRedirect(mock.Object, "2")
                .AddRedirect(mock.Object, "3");

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("3 2 1 foo 1 2 3");
        }
        
        [Fact]
        public async Task GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _router.Proxy(original);
            _router.AddRedirect(new AsyncFoo(async () => $"{await _router.Relay.Next.MessageAsync} me"));
            _router.Reset();
            _router.AddRedirect(new AsyncFoo(async () => $"{await _router.Relay.Next.MessageAsync} again"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo again");
        }
        
        [Fact]
        public async Task GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _router.Proxy(original);
            _router.AddRedirect(new AsyncFoo(async () => $"{await _router.Relay.Next.MessageAsync} me"));
            _router.AddRedirect(new AsyncFoo(async () => $"{await _router.Relay.Next.MessageAsync} again"));
            _router.Reset();

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
    }
}
