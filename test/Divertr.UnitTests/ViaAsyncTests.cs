using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaAsyncTests
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
            _via.RedirectTo(foo);

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
            _via.RedirectTo(foo);
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
            _via.RedirectTo(new AsyncFoo("diverted"));
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
            _via.RedirectTo(new AsyncFoo(async () => $"hello {await _via.Relay.Original.MessageAsync}"));

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
            _via.RedirectTo(new AsyncFoo(async () => $"hello {await _via.Relay.Next.MessageAsync}"));

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
            _via.RedirectTo(new AsyncFoo(async () =>
            {
                originalReference = _via.Relay.CallInfo.Original;
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
            
            _via.RedirectTo(new AsyncFoo(async () => $"diverted {await _via.Relay.Original.MessageAsync}"));

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
                .RedirectTo(new AsyncFoo(async () => $"diverted {await _via.Relay.Next.MessageAsync}"));

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

            _via.RedirectTo(mock.Object);

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
                .RedirectTo(new AsyncFoo(async () => $"again {await next.MessageAsync} 3"))
                .RedirectTo(new AsyncFoo(async () => $"here {await next.MessageAsync} 2"))
                .RedirectTo(new AsyncFoo(async () => $"DivertR {await next.MessageAsync} 1"));

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
                .InsertRedirect(_via.Redirect().Build(new AsyncFoo(async () => $"DivertR {await next.MessageAsync} 1")))
                .InsertRedirect(_via.Redirect().Build(new AsyncFoo(async () => $"here {await next.MessageAsync} 2")))
                .InsertRedirect(_via.Redirect().Build(new AsyncFoo(async () => $"again {await next.MessageAsync} 3")), -10);

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
                _via.RedirectTo(new AsyncFoo(async () =>
                    $"{await orig.MessageAsync} {counter} {await next.MessageAsync}"));
            }

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}").Reverse());
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new AsyncFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            var count = 4;

            var recursive = new AsyncFoo(async () =>
            {
                var decrement = Interlocked.Decrement(ref count);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.MessageAsync} {await proxy.MessageAsync} {await orig.MessageAsync}{decrement}]";
                }

                return await next.MessageAsync;
            });

            _via
                .RedirectTo(new AsyncFoo(async () =>
                    (await next.MessageAsync).Replace(await orig.MessageAsync, "bar")))
                .RedirectTo(recursive);

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

            async Task<string> WriteMessage(int num)
            {
                return $"{num} {await _via.Relay.Next.MessageAsync} {num}";
            }

            _via
                .InsertRedirect(_via.Redirect(x => x.MessageAsync).Build(() => WriteMessage(1)), 30)
                .InsertRedirect(_via.Redirect(x => x.MessageAsync).Build(() => WriteMessage(2)), 20)
                .InsertRedirect(_via.Redirect(x => x.MessageAsync).Build(() => WriteMessage(3)), 10);

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
            _via.RedirectTo(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} me"));
            _via.Reset();
            _via.RedirectTo(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} again"));

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
            _via.RedirectTo(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} me"));
            _via.RedirectTo(new AsyncFoo(async () => $"{await _via.Relay.Next.MessageAsync} again"));
            _via.Reset();

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe(await original.MessageAsync);
        }
        
        [Fact]
        public async Task GivenWhenPropertyRedirect_ShouldDivert()
        {
            // ARRANGE
            _via
                .Redirect(x => x.MessageAsync)
                .To(async () => $"before {await _via.Relay.Original.MessageAsync} after");

            // ACT
            var proxy = _via.Proxy(new AsyncFoo("hello foo"));
            var message = await proxy.MessageAsync;

            // ASSERT
            message.ShouldBe("before hello foo after");
        }
    }
}
