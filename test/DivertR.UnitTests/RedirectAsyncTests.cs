using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectAsyncTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        
        [Fact]
        public async Task GivenProxy_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            
            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public async Task GivenRetarget_WhenProxyAsyncMethodCalled_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            var foo = new Foo("DivertR");
            _redirect.Retarget(foo);

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe(foo.Name);
        }

        [Fact]
        public async Task GivenViaWithRootReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.GetNameAsync())
                .Via(async () => $"Diverted {await _redirect.Relay.Root.GetNameAsync()}");

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("Diverted foo");
        }
        
        [Fact]
        public async Task GivenViaWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.GetNameAsync())
                .Via(async () => $"Diverted {await _redirect.Relay.Next.GetNameAsync()}");

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("Diverted foo");
        }
        
        [Fact]
        public async Task GivenViaWithRootInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            IFoo? originalReference = null;
            _redirect
                .To(x => x.GetNameAsync())
                .Via(async call =>
                {
                    originalReference = call.CallInfo.Root;
                    return $"hello {await originalReference!.GetNameAsync()}";
                });

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public async Task GivenMultipleProxiesWithRootRelay_ShouldRedirect()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _redirect.Proxy(new Foo($"foo{i}")))
                .ToList();

            _redirect
                .To(x => x.GetNameAsync())
                .Via(async () => $"diverted {await _redirect.Relay.Root.GetNameAsync()}");

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
        public async Task GivenMultipleProxiesWithNextRelay_ShouldRedirect()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _redirect.Proxy(new Foo($"foo{i}")))
                .ToList();

            _redirect
                .To(x => x.GetNameAsync())
                .Via(async () => $"diverted {await _redirect.Relay.Next.GetNameAsync()}");

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
        public async Task GivenMockedVia_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var proxy = _redirect.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.GetNameAsync())
                .Returns(async () => $"{await _redirect.Relay.Root.GetNameAsync()} world");

            _redirect.Retarget(mock.Object);

            // ACT
            var name = await proxy.GetNameAsync();

            // ASSERT
            name.ShouldBe("hello world");
        }

        [Fact]
        public async Task GivenMultipleAddVias_ShouldChain()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var next = _redirect.Relay.Next;
            _redirect.To(x => x.GetNameAsync())
                .Via(async () => $"again {await next.GetNameAsync()} 3")
                .Via(async () => $"here {await next.GetNameAsync()} 2")
                .Via(async () => $"DivertR {await next.GetNameAsync()} 1");

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("DivertR here again hello foo 3 2 1");
        }

        [Fact]
        public async Task GivenMultipleAddViasWithNextAndRootRelays_ShouldChain()
        {
            // ARRANGE
            const int numVias = 100;
            var proxy = _redirect.Proxy(new Foo("foo"));
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;

            for (var i = 0; i < numVias; i++)
            {
                var counter = i;
                _redirect.To(x => x.GetNameAsync())
                    .Via(async () => $"{await orig.GetNameAsync()} {counter} {await next.GetNameAsync()}");
            }

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numVias).Select(i => $"{i}").Reverse());
            name.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleAddViasWithRecursiveProxy_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;
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

            _redirect
                .To(x => x.GetNameAsync())
                .Via(async () =>
                    (await next.GetNameAsync()).Replace(await orig.GetNameAsync(), "bar"))
                .Via(Recursive);

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public async Task GivenMultipleOrderedVias_ShouldChain()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));

            async Task<string> WriteMessage(int num)
            {
                return $"{num} {await _redirect.Relay.Next.GetNameAsync()} {num}";
            }

            _redirect
                .To(x => x.GetNameAsync())
                .Via(() => WriteMessage(1), options => options.OrderWeight(30))
                .Via(() => WriteMessage(2), options => options.OrderWeight(20))
                .Via(() => WriteMessage(3), options => options.OrderWeight(10));

            // ACT
            var name = await proxy.GetNameAsync();
            
            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
    }
}