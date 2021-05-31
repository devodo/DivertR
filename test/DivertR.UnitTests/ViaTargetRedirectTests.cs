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
        public void GivenViaWithNoRedirects_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithTargetRedirect_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _via.Redirect().To(foo);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(foo.Name);
        }
        
        [Fact]
        public void GivenProxyWithTargetRedirectTo_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _via.RedirectTo(foo);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(foo.Name);
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
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithRelayToOriginalRedirect_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new FooAlt(() => $"hello {_via.Relay.Original.Name}"));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithRelayToNextRedirect_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new FooAlt(() => $"hello {_via.Relay.Next.Name}"));
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithRelayToOriginalInstanceRedirect_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via.RedirectTo(new FooAlt(() =>
            {
                originalReference = _via.Relay.CallInfo.Original;
                return $"hello {originalReference!.Name}";
            }));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMultipleProxiesWithRelayToOriginalRedirects_ShouldRelay()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _via.RedirectTo(new FooAlt(() => $"diverted {_via.Relay.Original.Name}"));
            
            // ACT
            var names = proxies.Select(x => x.Name).ToList();

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                names[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMultipleProxiesWithRelayToNextRedirects_ShouldRelay()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _via.RedirectTo(new FooAlt(() => $"diverted {_via.Next.Name}"));

            // ACT
            var names = proxies.Select(x => x.Name).ToList();

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                names[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenProxyWithMockedRedirect_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var proxy = _via.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Name)
                .Returns(() => $"{_via.Relay.Original.Name} world");

            _via.RedirectTo(mock.Object);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello world");
        }

        [Fact]
        public void GivenProxyWithMultipleRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            
            _via
                .RedirectTo(new FooAlt(() => $"again {next.Name} 3"))
                .RedirectTo(new FooAlt(() => $"here {next.Name} 2"))
                .RedirectTo(new FooAlt(() => $"DivertR {next.Name} 1"));
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            name.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public void GivenProxyWithMultipleInsertRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .Redirect().To(new FooAlt(() => $"DivertR {next.Name} 1"))
                .Redirect().To(new FooAlt(() => $"here {next.Name} 2"))
                .Redirect().To(new FooAlt(() => $"again {next.Name} 3"), -10);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenProxyWithMultipleOrderedInsertRedirects_ShouldOrderChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            
            _via
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"DivertR {next.Name} 1")))
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"here {next.Name} 2")))
                .InsertRedirect(_via.Redirect().Build(new FooAlt(() => $"again {next.Name} 3")), -10);

            // ACT
            var name = proxy.Name;
            
            // ASSERT
            name.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenProxyWithMultipleRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int NumRedirects = 2;
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;
            
            for (var i = 0; i < NumRedirects; i++)
            {
                var counter = i;
                _via.RedirectTo(new FooAlt(() => $"{orig.Name} {counter} {next.Name}"));
            }
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, NumRedirects).Select(i => $"{i}").Reverse());
            name.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public void GivenProxyWithMultipleRedirectsWithRecursiveProxy_ShouldRedirect()
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
                    return $"[{decrement}{next.Name} {proxy.Name} {orig.Name}{decrement}]";
                }

                return next.Name;
            });
            
            _via
                .RedirectTo(new FooAlt(() => next.Name.Replace(orig.Name, "bar")))
                .RedirectTo(recursive);
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            name.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public void GivenProxyWithInsertMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            
            string WriteMessage(int num)
            {
                return $"{num} {_via.Relay.Next.Name} {num}";
            }
            
            _via
                .InsertRedirect(_via.Redirect(x => x.Name).Build(() => WriteMessage(1)), 30)
                .InsertRedirect(_via.Redirect(x => x.Name).Build(() => WriteMessage(2)), 20)
                .InsertRedirect(_via.Redirect(x => x.Name).Build(() => WriteMessage(3)), 10);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Name} me"));
            _via.Reset();
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Name} again"));

            // ASSERT
            proxy.Name.ShouldBe("hello foo again");
        }
        
        [Fact]
        public void GivenMultipleRedirects_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Name} me"));
            _via.RedirectTo(new FooAlt(() => $"{_via.Relay.Next.Name} again"));
            _via.Reset();

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
    }
}