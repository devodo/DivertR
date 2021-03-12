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
        private readonly Via<IValueTaskFoo> _via = new();

        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new ValueTaskFoo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new ValueTaskFoo(async () => $"hello {await _via.Relay.Original.MessageAsync}"));

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
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new ValueTaskFoo(async () => $"hello {await _via.Relay.Next.MessageAsync}"));

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
            var proxy = _via.Proxy(original);
            IValueTaskFoo originalReference = null;
            _via.RedirectTo(new ValueTaskFoo(async () =>
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
                .Select(i => _via.Proxy(new ValueTaskFoo($"foo{i}")))
                .ToList();
            
            _via.RedirectTo(new ValueTaskFoo(async () => $"diverted {await _via.Relay.Original.MessageAsync}"));

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
                .Select(i => _via.Proxy(new ValueTaskFoo($"foo{i}")))
                .ToList();

            _via
                .RedirectTo(new ValueTaskFoo(async () => $"diverted {await _via.Relay.Next.MessageAsync}"));

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
            var proxy = _via.Proxy(new ValueTaskFoo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .RedirectTo(new ValueTaskFoo(async () => $"DivertR {await next.MessageAsync} 1"))
                .RedirectTo(new ValueTaskFoo(async () => $"here {await next.MessageAsync} 2"))
                .RedirectTo(new ValueTaskFoo(async () => $"again {await next.MessageAsync} 3"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.RedirectTo(new ValueTaskFoo(async () =>
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
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            var recursive = new ValueTaskFoo(async () =>
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
                .RedirectTo(recursive, new[] {4})
                .RedirectTo(new ValueTaskFoo(async () =>
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
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));

            var mock = new Mock<IValueTaskFoo>();
            mock
                .Setup(x => x.MessageAsync)
                .Returns(async () => 
                    $"{_via.Relay.State} {await _via.Relay.Next.MessageAsync} {_via.Relay.State}");
            
            _via
                .RedirectTo(mock.Object, "1")
                .RedirectTo(mock.Object, "2")
                .RedirectTo(mock.Object, "3");

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("1 2 3 foo 3 2 1");
        }
    }
}