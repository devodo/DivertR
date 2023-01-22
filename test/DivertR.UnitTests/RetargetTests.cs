using System.Linq;
using System.Threading;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RetargetTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();

        [Fact]
        public void GivenViaRetarget_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _redirect.To().Retarget(foo);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(foo.Name);
        }
        
        [Fact]
        public void GivenRedirectRetarget_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _redirect.Retarget(foo);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(foo.Name);
        }

        [Fact]
        public void GivenRetarget_WhenReset_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.Retarget(new Foo("diverted"));

            // ACT
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenMultipleRetargets_WhenReset_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _redirect.Proxy(original);

            // ACT
            _redirect.Retarget(new FooAlt(() => $"{_redirect.Relay.Next.Name} me"));
            _redirect.Retarget(new FooAlt(() => $"{_redirect.Relay.Next.Name} again"));
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRetargetWithRelayToRoot_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.Retarget(new FooAlt(() => $"hello {_redirect.Relay.Root.Name}"));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRetargetWithRelayToNext_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.Retarget(new FooAlt(() => $"hello {_redirect.Relay.Next.Name}"));
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRetargetWithRelayToRootInstance_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            IFoo? originalReference = null;
            _redirect.Retarget(new FooAlt(() =>
            {
                originalReference = _redirect.Relay.GetCurrentCall().CallInfo.Root;
                return $"hello {originalReference!.Name}";
            }));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMultipleProxiesWithRelayToRootRetarget_ShouldRedirect()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _redirect.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _redirect.Retarget(new FooAlt(() => $"diverted {_redirect.Relay.Root.Name}"));
            
            // ACT
            var names = proxies.Select(x => x.Name).ToList();

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                names[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMultipleProxiesWithRelayToNextRetarget_ShouldRedirect()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _redirect.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _redirect.Retarget(new FooAlt(() => $"diverted {_redirect.Relay.Next.Name}"));

            // ACT
            var names = proxies.Select(x => x.Name).ToList();

            // ASSERT
            for (var i = 0; i < proxies.Count; i++)
            {
                names[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMockedRetarget_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var proxy = _redirect.Proxy(original);
            
            var mock = new Moq.Mock<IFoo>();
            mock
                .Setup(x => x.Name)
                .Returns(() => $"{_redirect.Relay.Root.Name} world");

            _redirect.Retarget(mock.Object);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello world");
        }

        [Fact]
        public void GivenMultipleRetargets_ShouldChainVias()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var next = _redirect.Relay.Next;
            
            _redirect
                .Retarget(new FooAlt(() => $"again {next.Name} 3"))
                .Retarget(new FooAlt(() => $"here {next.Name} 2"))
                .Retarget(new FooAlt(() => $"DivertR {next.Name} 1"));
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            name.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public void GivenMultipleWeightedRetargets_ShouldOrderChain()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var next = _redirect.Relay.Next;
            _redirect.To(x => x.Name)
                .Retarget(new FooAlt(() => $"DivertR {next.Name} 1"))
                .Retarget(new FooAlt(() => $"here {next.Name} 2"))
                .Retarget(new FooAlt(() => $"again {next.Name} 3"), options => options.OrderWeight(-10));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("here DivertR again hello foo 3 1 2");
        }

        [Fact]
        public void GivenMultipleRetargetsWithNextAndRootRelays_ShouldChain()
        {
            // ARRANGE
            const int NumVias = 2;
            var proxy = _redirect.Proxy(new Foo("foo"));
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;
            
            for (var i = 0; i < NumVias; i++)
            {
                var counter = i;
                _redirect.Retarget(new FooAlt(() => $"{orig.Name} {counter} {next.Name}"));
            }
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, NumVias).Select(i => $"{i}").Reverse());
            name.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public void GivenMultipleRetargetsWithRecursiveProxy_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            var next = _redirect.Relay.Next;
            var orig = _redirect.Relay.Root;
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
            
            _redirect
                .Retarget(new FooAlt(() => next.Name.Replace(orig.Name, "bar")))
                .Retarget(recursive);
            
            // ACT
            var name = proxy.Name;
            
            // ASSERT
            name.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }

        [Fact]
        public void GivenResetBetweenAddRetargets_ShouldOnlyViaAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _redirect.Proxy(original);

            // ACT
            _redirect.Retarget(new FooAlt(() => $"{_redirect.Relay.Next.Name} me"));
            _redirect.Reset();
            _redirect.Retarget(new FooAlt(() => $"{_redirect.Relay.Next.Name} again"));

            // ASSERT
            proxy.Name.ShouldBe("hello foo again");
        }
    }
}