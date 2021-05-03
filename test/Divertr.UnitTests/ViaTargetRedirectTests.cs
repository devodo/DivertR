using System.Linq;
using System.Threading;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaTargetRedirectTests
    {
        private readonly Via<IFoo> _via = new();
        
        [Fact]
        public void GivenViaWithNoRedirects_WhenCreateProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _via.Proxy(original);

            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenProxy_WhenAddTargetRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            
            // ACT
            _via.Redirect().To(foo);

            // ASSERT
            proxy.Message.ShouldBe(foo.Message);
        }
        
        [Fact]
        public void GivenProxy_WhenRedirectTo_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            
            // ACT
            _via.RedirectTo(foo);

            // ASSERT
            proxy.Message.ShouldBe(foo.Message);
        }

        [Fact]
        public void GivenProxyWithRedirect_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new Foo("diverted"));

            // ACT
            _via.Reset();

            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToOriginal_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new FooAlt(() => $"hello {_via.Relay.Original.Message}"));

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToNext_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            _via.RedirectTo(new FooAlt(() => $"hello {_via.Relay.Next.Message}"));

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToOriginalInstance_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;

            // ACT
            _via.RedirectTo(new FooAlt(() =>
            {
                originalReference = _via.Relay.CallInfo.Original;
                return $"hello {originalReference!.Message}";
            }));

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMultipleProxies_WhenAddRedirectWithRelayToOriginal_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            // ACT
            _via.RedirectTo(new FooAlt(() => $"diverted {_via.Relay.Original.Message}"));

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                proxies[i].Message.ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMultipleProxies_WhenAddRedirectWithRelayToNext_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();

            // ACT
            _via.RedirectTo(new FooAlt(() => $"diverted {_via.Next.Message}"));

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                proxies[i].Message.ShouldBe($"diverted foo{i}");
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

            _via.RedirectTo(mock.Object);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void GivenProxy_WhenAddMultipleRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            
            // ACT
            _via
                .RedirectTo(new FooAlt(() => $"again {next.Message} 3"))
                .RedirectTo(new FooAlt(() => $"here {next.Message} 2"))
                .RedirectTo(new FooAlt(() => $"DivertR {next.Message} 1"));
            
            // ASSERT
            proxy.Message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public void GivenProxy_WhenMultipleInsert_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;

            // ACT
            _via
                .Redirect().To(new FooAlt(() => $"DivertR {next.Message} 1"))
                .Redirect().To(new FooAlt(() => $"here {next.Message} 2"))
                .Redirect().To(new FooAlt(() => $"again {next.Message} 3"), -10);

            // ASSERT
            proxy.Message.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenProxy_WhenMultipleInsertRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;

            // ACT
            _via
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"DivertR {next.Message} 1")))
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"here {next.Message} 2")))
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"again {next.Message} 3")), -10);
            
            // ASSERT
            proxy.Message.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenProxy_WhenAddMultipleRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 2;
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            
            // ACT
            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.RedirectTo(new FooAlt(() => $"{orig.Message} {counter} {next.Message}"));
            }
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}").Reverse());
            proxy.Message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public void GivenProxy_WhenAddMultipleRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            var count = 4;

            var recursive = new FooAlt(() =>
            {
                var decrement = Interlocked.Decrement(ref count);

                if (decrement > 0)
                {
                    return $"[{decrement}{next.Message} {proxy.Message} {orig.Message}{decrement}]";
                }

                return next.Message;
            });
            
            // ACT
            _via
                .RedirectTo(new FooAlt(() => next.Message.Replace(orig.Message, "bar")))
                .RedirectTo(recursive);
            
            // ASSERT
            proxy.Message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public void GivenProxy_WhenInsertMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            
            string WriteMessage(int num)
            {
                return $"{num} {_via.Relay.Next.Message} {num}";
            }

            // ACT
            _via
                .InsertRedirect(_via.Redirect(x => x.Message).Build(() => WriteMessage(1)), 30)
                .InsertRedirect(_via.Redirect(x => x.Message).Build(() => WriteMessage(2)), 20)
                .InsertRedirect(_via.Redirect(x => x.Message).Build(() => WriteMessage(3)), 10);

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
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Message} me"));
            _via.Reset();
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Message} again"));

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
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Message} me"));
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Message} again"));
            _via.Reset();


            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
    }
}
