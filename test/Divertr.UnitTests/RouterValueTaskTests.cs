using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RouterValueTaskTests
    {
        private readonly Router<IValueTaskFoo> _router = new();

        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new ValueTaskFoo("foo");
            var proxy = _router.Proxy(original);
            _router.Redirect(new ValueTaskFoo(async () => $"hello {await _router.Relay.Original.MessageAsync}"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new ValueTaskFoo("foo");
            var proxy = _router.Proxy(original);
            _router.Redirect(new ValueTaskFoo(async () => $"hello {await _router.Relay.Next.MessageAsync}"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new ValueTaskFoo("foo");
            var proxy = _router.Proxy(original);
            IValueTaskFoo originalReference = null;
            _router.Redirect(new ValueTaskFoo(async () =>
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
        public async Task GivenMultipleProxiesWithOriginalRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _router.Proxy(new ValueTaskFoo($"foo{i}")))
                .ToList();
            
            _router.Redirect(new ValueTaskFoo(async () => $"diverted {await _router.Relay.Original.MessageAsync}"));

            // ACT
            var messages = proxies.Select(async p => await p.MessageAsync).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                var message = await messages[i];
                message.ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public async Task GivenMultipleProxiesWithNextRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _router.Proxy(new ValueTaskFoo($"foo{i}")))
                .ToList();

            _router
                .Redirect(new ValueTaskFoo(async () => $"diverted {await _router.Relay.Next.MessageAsync}"));

            // ACT
            var messages = proxies.Select(async p => await p.MessageAsync).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                var message = await messages[i];
                message.ShouldBe($"diverted foo{i}");
            }
        }

        [Fact]
        public async Task GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _router.Proxy(new ValueTaskFoo("hello foo"));
            var next = _router.Relay.Next;
            _router
                .AddRedirect(new ValueTaskFoo(async () => $"{await next.MessageAsync} me"))
                .AddRedirect(new ValueTaskFoo(async () => $"{await next.MessageAsync} here"))
                .AddRedirect(new ValueTaskFoo(async () => $"{await next.MessageAsync} again"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("hello foo me here again");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _router.Proxy(new ValueTaskFoo("foo"));
            var next = _router.Relay.Next;
            var orig = _router.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _router.AddRedirect(new ValueTaskFoo(async () =>
                    $"{await orig.MessageAsync} {counter} {await next.MessageAsync}"));
            }

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Reverse().Select(i => $"{i}"));
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _router.Proxy(new ValueTaskFoo("foo"));
            var next = _router.Relay.Next;
            var orig = _router.Relay.Original;

            var recursive = new ValueTaskFoo(async () =>
            {
                var state = (int[]) _router.Relay.State;
                var decrement = Interlocked.Decrement(ref state[0]);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.MessageAsync} {await proxy.MessageAsync} {await orig.MessageAsync}{decrement}]";
                }

                return await next.MessageAsync;
            });

            _router
                .AddRedirect(recursive, new[] {4})
                .AddRedirect(new ValueTaskFoo(async () =>
                    (await next.MessageAsync).Replace(await orig.MessageAsync, "bar")));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar bar1] bar2] bar3]");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var proxy = _router.Proxy(new ValueTaskFoo("foo"));

            var mock = new Mock<IValueTaskFoo>();
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
    }
}