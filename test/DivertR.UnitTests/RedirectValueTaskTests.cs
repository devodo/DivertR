using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectValueTaskTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly IFoo _original = new Foo("foo");
        private readonly IFoo _proxy;

        public RedirectValueTaskTests()
        {
            _proxy = _redirect.Proxy(_original);
        }

        [Fact]
        public async Task GivenRetargetWithRootRelay_ShouldRedirect()
        {
            // ARRANGE
            _redirect.Retarget(new FooAlt(() => $"alt {_redirect.Relay.Root.Name}"));

            // ACT
            var nameAsync = await _proxy.GetNameValueAsync();
            var nameSync = await _proxy.GetNameValueAsync();

            // ASSERT
            nameAsync.ShouldBe("alt foo");
            nameSync.ShouldBe("alt foo");
        }
        
        [Fact]
        public async Task GivenViaWithNextRelay_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoValueAsync(Is<string>.Any))
                .Via<(string input, __)>(async call => await call.Relay.Next.EchoValueAsync(call.Args.input) + " via");

            // ACT
            var message = await _proxy.EchoValueAsync("test");

            // ASSERT
            message.ShouldBe("foo: test via");
        }
        
        [Fact]
        public async Task GivenViaWithNextRelay_WhenSyncCompletedAsyncProxyMethodCalled_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoValueSync(Is<string>.Any))
                .Via<(string input, __)>(call => call.Relay.Next.EchoValueSync($"{call.Args.input} via"));

            // ACT
            var message = await _proxy.EchoValueSync("test");

            // ASSERT
            message.ShouldBe("foo: test via");
        }
        
        [Fact]
        public async Task GivenViaWithRootInstanceRelay_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoValueAsync(Is<string>.Any))
                .Via<(string input, __)>(call => call.CallInfo.Root!.EchoValueAsync($"{call.Args.input} via"));

            // ACT
            var message = await _proxy.EchoValueAsync("test");

            // ASSERT
            message.ShouldBe("foo: test via");
        }

        [Fact]
        public async Task GivenMultipleViasWithNextAndRootRelays_ShouldChain()
        {
            // ARRANGE
            const int NumVias = 100;
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;

            for (var i = 0; i < NumVias; i++)
            {
                var counter = i;
                _redirect
                    .To(x => x.GetNameValueAsync())
                    .Via(async () => $"{await orig.GetNameValueAsync()} {counter} {await next.GetNameValueAsync()}");
            }

            // ACT
            var message = await _proxy.GetNameValueAsync();
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, NumVias).Select(i => $"{i}").Reverse());
            message.ShouldBe($"foo {join} foo");
        }
       
        [Fact]
        public async Task GivenMultipleViasWithRecursiveProxy_ShouldRedirect()
        {
            // ARRANGE
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;
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

            _redirect
                .To(x => x.GetNameValueAsync())
                .Via(async () => (await next.GetNameValueAsync()).Replace(await orig.GetNameValueAsync(), "bar"))
                .Via(Recursive);
            // ACT
            var message = await _proxy.GetNameValueAsync();
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
    }
}