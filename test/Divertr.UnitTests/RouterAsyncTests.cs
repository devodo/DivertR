using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RouterAsyncTests
    {
        private readonly Via<IAsyncFoo> _via = new();
        
        [Fact]
        public async Task GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
        
        [Fact]
        public async Task GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("hello foo"));
            var foo = new AsyncFoo("hi DivertR");
            _via.Redirect(foo);

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe(await foo.MessageAsync);
        }
        
        [Fact]
        public async Task GivenRedirectBeforeCreateProxy_ShouldDivert()
        {
            // ARRANGE
            var foo = new AsyncFoo("hi DivertR");
            _via.Redirect(foo);
            var proxy = _via.Proxy(new AsyncFoo("hello foo"));

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
            var proxy = _via.Proxy(original);
            _via.Redirect(new AsyncFoo("diverted"));
            _via.Reset();

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
            var proxy = _via.Proxy(original);
            _via.Redirect(new AsyncFoo(async () => $"hello {await _via.Relay.Original.MessageAsync}"));

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
            var proxy = _via.Proxy(original);
            _via.Redirect(new AsyncFoo(async () => $"hello {await _via.Relay.Next.MessageAsync}"));

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
            var proxy = _via.Proxy(original);
            IAsyncFoo originalReference = null;
            _via.Redirect(new AsyncFoo(async () =>
            {
                originalReference = _via.Relay.OriginalInstance;
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
                .Select(i => _via.Proxy(new AsyncFoo($"foo{i}")))
                .ToList();
            
            _via.Redirect(new AsyncFoo(async () => $"diverted {await _via.Relay.Original.MessageAsync}"));

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
                .Select(i => _via.Proxy(new AsyncFoo($"foo{i}")))
                .ToList();

            _via
                .Redirect(new AsyncFoo(async () => $"diverted {await _via.Relay.Next.MessageAsync}"));

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
        public async Task GivenMockedRedirect_ShouldDivert()
        {
            // ARRANGE
            var original = new AsyncFoo("hello");
            var proxy = _via.Proxy(original);
            
            var mock = new Mock<IAsyncFoo>();
            mock
                .Setup(x => x.MessageAsync)
                .Returns(async () => $"{await _via.Relay.Original.MessageAsync} world");

            _via.Redirect(mock.Object);

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public async Task GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .AddRedirect(new AsyncFoo(async () => $"DivertR {await next.MessageAsync} 1"))
                .AddRedirect(new AsyncFoo(async () => $"here {await next.MessageAsync} 2"))
                .AddRedirect(new AsyncFoo(async () => $"again {await next.MessageAsync} 3"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public async Task GivenMultipleInsertRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .InsertRedirect(0, new AsyncFoo(async () => $"DivertR {await next.MessageAsync} 1"))
                .InsertRedirect(0, new AsyncFoo(async () => $"here {await next.MessageAsync} 2"))
                .InsertRedirect(2, new AsyncFoo(async () => $"again {await next.MessageAsync} 3"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new AsyncFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.AddRedirect(new AsyncFoo(async () =>
                    $"{await orig.MessageAsync} {counter} {await next.MessageAsync}"));
            }

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}"));
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            var recursive = new AsyncFoo(async () =>
            {
                var state = (int[]) _via.Relay.State;
                var decrement = Interlocked.Decrement(ref state[0]);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.MessageAsync} {await proxy.MessageAsync} {await orig.MessageAsync}{decrement}]";
                }

                return await next.MessageAsync;
            });

            _via
                .AddRedirect(recursive, new[] {4})
                .AddRedirect(new AsyncFoo(async () =>
                    (await next.MessageAsync).Replace(await orig.MessageAsync, "bar")));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("foo"));

            var mock = new Mock<IAsyncFoo>();
            mock
                .Setup(x => x.MessageAsync)
                .Returns(async () => 
                    $"{_via.Relay.State} {await _via.Relay.Next.MessageAsync} {_via.Relay.State}");
            
            _via
                .AddRedirect(mock.Object, "1")
                .AddRedirect(mock.Object, "2")
                .AddRedirect(mock.Object, "3");

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public async Task GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _via.Proxy(original);
            _via.AddRedirect(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} me"));
            _via.Reset();
            _via.AddRedirect(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} again"));

            // ACT
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("hello foo again");
        }
        
        [Fact]
        public async Task GivenMultipleRedirects_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new AsyncFoo("hello foo");
            var proxy = _via.Proxy(original);
            _via.AddRedirect(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} me"));
            _via.AddRedirect(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} again"));
            _via.Reset();

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
    }
}
