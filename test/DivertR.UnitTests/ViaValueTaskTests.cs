using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaValueTaskTests
    {
        private readonly Via<IValueTaskFoo> _via = new();

        [Fact]
        public async Task GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new ValueTaskFoo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(new ValueTaskFoo(async () => $"hello {await _via.Relay.Original.MessageAsync}"));

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
            _via.Redirect(new ValueTaskFoo(async () => $"hello {await _via.Relay.Next.MessageAsync}"));

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
            _via.Redirect(new ValueTaskFoo(async () =>
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
                .Select(i => _via.Proxy(new ValueTaskFoo($"foo{i}")))
                .ToList();
            
            _via.Redirect(new ValueTaskFoo(async () => $"diverted {await _via.Relay.Original.MessageAsync}"));

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
                .Redirect(new ValueTaskFoo(async () => $"diverted {await _via.Relay.Next.MessageAsync}"));

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
        public async Task GivenMultipleRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new ValueTaskFoo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .Redirect(new ValueTaskFoo(async () => $"again {await next.MessageAsync} 3"))
                .Redirect(new ValueTaskFoo(async () => $"here {await next.MessageAsync} 2"))
                .Redirect(new ValueTaskFoo(async () => $"DivertR {await next.MessageAsync} 1"));

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public async Task GivenMultipleRedirectsWithOrderWeights_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));
            
            async ValueTask<string> WriteMessage(int num)
            {
                return $"{num} {await _via.Relay.Next.MessageAsync} {num}";
            }

            _via
                .InsertRedirect(_via.When(x => x.MessageAsync).Build(() => WriteMessage(1)), 30)
                .InsertRedirect(_via.When(x => x.MessageAsync).Build(() => WriteMessage(2)), 20)
                .InsertRedirect(_via.When(x => x.MessageAsync).Build(() => WriteMessage(3)), 10);
                
            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public async Task GivenMultipleRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.Redirect(new ValueTaskFoo(async () =>
                    $"{await orig.MessageAsync} {counter} {await next.MessageAsync}"));
            }

            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}").Reverse());
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public async Task GivenMultipleRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new ValueTaskFoo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            int count = 4;

            var recursive = new ValueTaskFoo(async () =>
            {
                var decrement = Interlocked.Decrement(ref count);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.MessageAsync} {await proxy.MessageAsync} {await orig.MessageAsync}{decrement}]";
                }

                return await next.MessageAsync;
            });

            _via
                .Redirect(new ValueTaskFoo(async () =>
                    (await next.MessageAsync).Replace(await orig.MessageAsync, "bar")))
                .Redirect(recursive);
            // ACT
            var message = await proxy.MessageAsync;
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
    }
}