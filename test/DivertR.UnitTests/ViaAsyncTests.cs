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
        private readonly Via<IFoo> _via = new();
        
        [Fact]
        public async Task GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = await proxy.GetMessageAsync();
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public async Task GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _via.RedirectTo(foo);

            // ACT
            var message = await proxy.GetMessageAsync();

            // ASSERT
            message.ShouldBe(foo.Message);
        }

        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () => $"hello {await _via.Relay.Original.GetMessageAsync()}");

            // ACT
            var message = await proxy.GetMessageAsync();

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () => $"hello {await _via.Relay.Next.GetMessageAsync()}");

            // ACT
            var message = await proxy.GetMessageAsync();

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () =>
                {
                    originalReference = _via.Relay.CallInfo.Original;
                    return $"hello {await originalReference!.GetMessageAsync()}";
                });

            // ACT
            var message = await proxy.GetMessageAsync();

            // ASSERT
            message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public async Task GivenMultipleProxiesWithOriginalRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();

            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () => $"diverted {await _via.Relay.Original.GetMessageAsync()}");

            // ACT
            var messages = proxies.Select(p => p.GetMessageAsync()).ToList();

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
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();

            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () => $"diverted {await _via.Relay.Next.GetMessageAsync()}");

            // ACT
            var messages = proxies.Select(p => p.GetMessageAsync()).ToList();

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
            var original = new Foo("hello");
            var proxy = _via.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.GetMessageAsync())
                .Returns(async () => $"{await _via.Relay.Original.GetMessageAsync()} world");

            _via.RedirectTo(mock.Object);

            // ACT
            var message = await proxy.GetMessageAsync();

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public async Task GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via.Redirect(x => x.GetMessageAsync()).To(async () => $"again {await next.GetMessageAsync()} 3")
                .Redirect(x => x.GetMessageAsync()).To(async () => $"here {await next.GetMessageAsync()} 2")
                .Redirect(x => x.GetMessageAsync()).To(async () => $"DivertR {await next.GetMessageAsync()} 1");

            // ACT
            var message = await proxy.GetMessageAsync();
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }

        [Fact]
        public async Task GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.Redirect(x => x.GetMessageAsync())
                    .To(async () => $"{await orig.GetMessageAsync()} {counter} {await next.GetMessageAsync()}");
            }

            // ACT
            var message = await proxy.GetMessageAsync();
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}").Reverse());
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            var count = 4;

            async Task<string> Recursive()
            {
                var decrement = Interlocked.Decrement(ref count);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.GetMessageAsync()} {await proxy.GetMessageAsync()} {await orig.GetMessageAsync()}{decrement}]";
                }

                return await next.GetMessageAsync();
            }

            _via
                .Redirect(x => x.GetMessageAsync())
                .To(async () =>
                    (await next.GetMessageAsync()).Replace(await orig.GetMessageAsync(), "bar"))
                .Redirect(x => x.GetMessageAsync()).To(Recursive);

            // ACT
            var message = await proxy.GetMessageAsync();
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public async Task GivenMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));

            async Task<string> WriteMessage(int num)
            {
                return $"{num} {await _via.Relay.Next.GetMessageAsync()} {num}";
            }

            _via
                .InsertRedirect(_via.Redirect(x => x.GetMessageAsync()).Build(() => WriteMessage(1)), 30)
                .InsertRedirect(_via.Redirect(x => x.GetMessageAsync()).Build(() => WriteMessage(2)), 20)
                .InsertRedirect(_via.Redirect(x => x.GetMessageAsync()).Build(() => WriteMessage(3)), 10);

            // ACT
            var message = await proxy.GetMessageAsync();
            
            // ASSERT
            message.ShouldBe("1 2 3 foo 3 2 1");
        }
    }
}
