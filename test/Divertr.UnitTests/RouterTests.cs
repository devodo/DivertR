using System.Linq;
using System.Threading;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RouterTests
    {
        private readonly Via<IFoo> _via = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _via.Redirect(foo);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(foo.Message);
        }
        
        [Fact]
        public void GivenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(new Foo("diverted"));
            _via.Reset();

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(new Foo(() => $"hello {_via.Relay.Original.Message}"));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(new Foo(() => $"hello {_via.Relay.Next.Message}"));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via.Redirect(new Foo(() =>
            {
                originalReference = _via.Relay.OriginalInstance;
                return $"hello {originalReference!.Message}";
            }));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMultipleProxiesWithOriginalRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _via.Redirect(new Foo( () => $"diverted {_via.Relay.Original.Message}"));

            // ACT
            var messages = proxies.Select(p => p.Message).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                messages[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMultipleProxiesWithNextRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();

            _via
                .Redirect(new Foo( () => $"diverted {_via.Relay.Next.Message}"));

            // ACT
            var messages = proxies.Select(p => p.Message).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                messages[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMockedRedirect_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("hello");
            var proxy = _via.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_via.Relay.Original.Message} world");

            _via.Redirect(mock.Object);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .AddRedirect(new Foo(() => $"DivertR {next.Message} 1"))
                .AddRedirect(new Foo(() => $"here {next.Message} 2"))
                .AddRedirect(new Foo(() => $"again {next.Message} 3"));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public void GivenMultipleInsertRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .InsertRedirect(0, new Foo(() => $"DivertR {next.Message} 1"))
                .InsertRedirect(0, new Foo(() => $"here {next.Message} 2"))
                .InsertRedirect(2, new Foo(() => $"again {next.Message} 3"));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.AddRedirect(new Foo (() => $"{orig.Message} {counter} {next.Message}"));
            }

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}"));
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            var recursive = new Foo( () =>
            {
                var state = (int[]) _via.Relay.State;
                var decrement = Interlocked.Decrement(ref state[0]);

                if (decrement > 0)
                {
                    return $"[{decrement}{next.Message} {proxy.Message} {orig.Message}{decrement}]";
                }

                return next.Message;
            });

            _via
                .AddRedirect(recursive, new[] {4})
                .AddRedirect(new Foo( () => next.Message.Replace(orig.Message, "bar")));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));

            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => 
                    $"{_via.Relay.State} {_via.Relay.Next.Message} {_via.Relay.State}");

            // ACT
            _via
                .AddRedirect(mock.Object, "1")
                .AddRedirect(mock.Object, "2")
                .AddRedirect(mock.Object, "3");
            
            // ASSERT
            proxy.Message.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.AddRedirect(new Foo(() => $"{_via.Relay.Next.Message} me"));
            _via.Reset();
            _via.AddRedirect(new Foo(() => $"{_via.Relay.Next.Message} again"));

            // ASSERT
            proxy.Message.ShouldBe("hello foo again");
        }
        
        [Fact]
        public void GivenMultipleRedirects_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.AddRedirect(new Foo(() => $"{_via.Relay.Next.Message} me"));
            _via.AddRedirect(new Foo(() => $"{_via.Relay.Next.Message} again"));
            _via.Reset();


            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
    }
}