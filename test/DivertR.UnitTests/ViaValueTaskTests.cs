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
        private readonly IVia<IFoo> _via = Via.For<IFoo>();
        private readonly IFoo _original = new Foo("foo");
        private readonly IFoo _proxy;

        public ViaValueTaskTests()
        {
            _proxy = _via.Proxy(_original);
        }

        [Fact]
        public async Task GivenTargetRedirectWithOriginalRelay_ShouldDivert()
        {
            // ARRANGE
            _via.Retarget(new FooAlt(() => $"alt {_via.Relay.Root.Name}"));

            // ACT
            var nameAsync = await _proxy.GetNameValueAsync();
            var nameSync = await _proxy.GetNameValueAsync();

            // ASSERT
            nameAsync.ShouldBe("alt foo");
            nameSync.ShouldBe("alt foo");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextRelay_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.EchoValueAsync(Is<string>.Any))
                .Redirect<(string input, __)>(async call => await call.Relay.Next.EchoValueAsync(call.Args.input) + " redirect");

            // ACT
            var message = await _proxy.EchoValueAsync("test");

            // ASSERT
            message.ShouldBe("foo: test redirect");
        }
        
        [Fact]
        public async Task GivenRedirectWithNextRelay_ShouldDivertSync()
        {
            // ARRANGE
            _via
                .To(x => x.EchoValueSync(Is<string>.Any))
                .Redirect<(string input, __)>(call => call.Relay.Next.EchoValueSync($"{call.Args.input} redirect"));

            // ACT
            var message = await _proxy.EchoValueSync("test");

            // ASSERT
            message.ShouldBe("foo: test redirect");
        }
        
        [Fact]
        public async Task GivenRedirectWithOriginalInstanceRelay_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.EchoValueAsync(Is<string>.Any))
                .Redirect<(string input, __)>(call => call.CallInfo.Original!.EchoValueAsync($"{call.Args.input} redirect"));

            // ACT
            var message = await _proxy.EchoValueAsync("test");

            // ASSERT
            message.ShouldBe("foo: test redirect");
        }

        [Fact]
        public async Task GivenMultipleRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int NumRedirects = 100;
            var next = _via.Relay.Next;
            var orig = _via.Relay.Root;

            for (var i = 0; i < NumRedirects; i++)
            {
                var counter = i;
                _via
                    .To(x => x.GetNameValueAsync())
                    .Redirect(async () => $"{await orig.GetNameValueAsync()} {counter} {await next.GetNameValueAsync()}");
            }

            // ACT
            var message = await _proxy.GetNameValueAsync();
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, NumRedirects).Select(i => $"{i}").Reverse());
            message.ShouldBe($"foo {join} foo");
        }
       
        [Fact]
        public async Task GivenMultipleRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var next = _via.Relay.Next;
            var orig = _via.Relay.Root;
            int count = 4;

            async ValueTask<string> Recursive()
            {
                var decrement = Interlocked.Decrement(ref count);

                if (decrement > 0)
                {
                    return $"[{decrement}{await next.GetNameValueAsync()} {await _proxy.GetNameValueAsync()} {await orig.GetNameValueAsync()}{decrement}]";
                }

                return await next.GetNameValueAsync();
            }

            _via
                .To(x => x.GetNameValueAsync())
                .Redirect(async () => (await next.GetNameValueAsync()).Replace(await orig.GetNameValueAsync(), "bar"))
                .Redirect(Recursive);
            // ACT
            var message = await _proxy.GetNameValueAsync();
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
    }
}