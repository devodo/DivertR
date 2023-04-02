using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();

        [Fact]
        public void GivenNoVias_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _redirect.Proxy(original);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenValidRootObjectType_WhenCreateProxyObject_ShouldCreateProxy()
        {
            // ARRANGE
            var original = new Foo();

            // ACT
            var proxy = (IFoo) ((IRedirect) _redirect).Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithNullRoot_WhenProxyMemberCalled_ShouldThrowException()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(null);

            // ACT
            Func<object> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<DiverterNullRootException>();
        }
        
        [Fact]
        public void GivenInvalidRootObjectType_WhenCreateProxyObject_ShouldThrowArgumentException()
        {
            // ARRANGE
            var invalidOriginal = new object();

            // ACT
            Func<object> testAction = () => _redirect.Proxy(invalidOriginal);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenDefaultDiverterSettings_WhenCreateProxiesWithSameRootInstance_ShouldCache()
        {
            // ARRANGE
            var foo = new Foo();
            var proxy = _redirect.Proxy(foo);

            // ACT
            var testProxy = _redirect.Proxy(foo);

            // ASSERT
            testProxy.ShouldBeSameAs(proxy);
        }

        [Fact]
        public void GivenCacheRedirectProxiesDisabled_WhenCreateProxiesWithSameRootInstance_ShouldNotCache()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>(new DiverterSettings(cacheRedirectProxies: false));
            var foo = new Foo();
            var proxy = redirect.Proxy(foo);

            // ACT
            var testProxy = redirect.Proxy(foo);

            // ASSERT
            testProxy.ShouldNotBeSameAs(proxy);
        }

        [Fact]
        public void GivenStrictModeWithNoVia_ShouldThrowException()
        {
            // ARRANGE
            _redirect.Strict();
            var proxy = _redirect.Proxy(new Foo("hello foo"));

            // ACT
            Func<string> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenStrictModeWithVia_ShouldNotThrowException()
        {
            // ARRANGE
            _redirect.Strict();
            _redirect.To(x => x.Name).Via("divert");
            var proxy = _redirect.Proxy(new Foo("hello foo"));

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("divert");
        }
        
        [Fact]
        public void GivenPersistentVia_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " changed", opt => opt.Persist());
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("foo changed");
        }
        
        [Fact]
        public void GivenPersistentVia_WhenReset_ShouldNotRemoveVia()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " changed", opt => opt.Persist());
            
            // ACT
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe("foo changed");
        }

        [Fact]
        public void GivenPersistentViaFollowedByNormalVia_ShouldViaChain()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " changed", opt => opt.Persist());
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " again");
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("foo changed again");
        }
        
        [Fact]
        public void GivenPersistentViaFollowedByNormalVia_WhenRedirectReset_ShouldKeepPersistentVia()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " changed", opt => opt.Persist());
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " again");
            
            // ACT
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe("foo changed");
        }

        [Fact]
        public void GivenNormalViaFollowedByPersistentVia_WhenRedirectReset_ShouldKeepPersistentVia()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " changed");
            
            _redirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " persistent", opt => opt.Persist());
            
            // ACT
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe("foo persistent");
        }
    }
}