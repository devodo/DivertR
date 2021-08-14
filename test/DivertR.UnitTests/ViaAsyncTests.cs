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
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public async Task GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            var foo = new Foo("DivertR");
            _via.Retarget(foo);

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe(foo.Name);
        }

        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.GetNameAsync())
                .Redirect(async () => $"Diverted {await _via.Relay.Original.GetNameAsync()}");

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("Diverted foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.GetNameAsync())
                .Redirect(async () => $"Diverted {await _via.Relay.Next.GetNameAsync()}");

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("Diverted foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via
                .To(x => x.GetNameAsync())
                .Redirect(async () =>
                {
                    originalReference = _via.Relay.CallInfo.Original;
                    return $"hello {await originalReference!.GetNameAsync()}";
                });

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("hello foo");
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
                .To(x => x.GetNameAsync())
                .Redirect(async () => $"diverted {await _via.Relay.Original.GetNameAsync()}");

            // ACT
            var names = proxies.Select(p => p.GetNameAsync()).ToList();

            // ASSERT
            for (var i = 0; i < names.Count; i++)
            {
                var name = await names[i];
                name.ShouldBe($"diverted foo{i}");
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
                .To(x => x.GetNameAsync())
                .Redirect(async () => $"diverted {await _via.Relay.Next.GetNameAsync()}");

            // ACT
            var names = proxies.Select(p => p.GetNameAsync()).ToList();

            // ASSERT
            for (var i = 0; i < names.Count; i++)
            {
                var name = await names[i];
                name.ShouldBe($"diverted foo{i}");
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
                .Setup(x => x.GetNameAsync())
                .Returns(async () => $"{await _via.Relay.Original.GetNameAsync()} world");

            _via.Retarget(mock.Object);

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("hello world");
        }

        [Fact]
        public async Task GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via.To(x => x.GetNameAsync()).Redirect(async () => $"again {await next.GetNameAsync()} 3")
                .To(x => x.GetNameAsync()).Redirect(async () => $"here {await next.GetNameAsync()} 2")
                .To(x => x.GetNameAsync()).Redirect(async () => $"DivertR {await next.GetNameAsync()} 1");

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("DivertR here again hello foo 3 2 1");
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
                _via.To(x => x.GetNameAsync())
                    .Redirect(async () => $"{await orig.GetNameAsync()} {counter} {await next.GetNameAsync()}");
            }

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}").Reverse());
            name.ShouldBe($"foo {join} foo");
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
                    return $"[{decrement}{await next.GetNameAsync()} {await proxy.GetNameAsync()} {await orig.GetNameAsync()}{decrement}]";
                }

                return await next.GetNameAsync();
            }

            _via
                .To(x => x.GetNameAsync())
                .Redirect(async () =>
                    (await next.GetNameAsync()).Replace(await orig.GetNameAsync(), "bar"))
                .To(x => x.GetNameAsync()).Redirect(Recursive);

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public async Task GivenMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));

            async Task<string> WriteMessage(int num)
            {
                return $"{num} {await _via.Relay.Next.GetNameAsync()} {num}";
            }

            _via
                .To(x => x.GetNameAsync()).WithOrderWeight(30).Redirect(() => WriteMessage(1))
                .To(x => x.GetNameAsync()).WithOrderWeight(20).Redirect(() => WriteMessage(2))
                .To(x => x.GetNameAsync()).WithOrderWeight(10).Redirect(() => WriteMessage(3));

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
    }
}