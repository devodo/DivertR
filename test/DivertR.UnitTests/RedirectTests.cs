using System;
using System.Threading.Tasks;
using DivertR.DispatchProxy;
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

        [Fact]
        public void GivenTypeViaRedirectConfigured_WhenTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());

            var barRedirect = _redirect.ViaRedirect<IBar>();
            barRedirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = proxy.EchoGeneric<IBar>(new Bar("test"));

            // ASSERT
            bar.Name.ShouldBe("test redirected");
        }
        
        [Fact]
        public async Task GivenTypeViaRedirectConfigured_WhenTaskTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());

            var barRedirect = _redirect.ViaRedirect<IBar>();
            barRedirect
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = await proxy.EchoGeneric(Task.FromResult<IBar>(new Bar("test")));

            // ASSERT
            bar.Name.ShouldBe("test redirected");
        }
        
        [Fact]
        public async Task GivenTypeViaRedirectConfigured_WhenTaskException_ShouldReturnException()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());

            _redirect.ViaRedirect<IBar>();
            
            _redirect
                .To(x => x.EchoGeneric(Is<Task<IBar>>.Any))
                .Via(async () =>
                {
                    await Task.Yield();

                    throw new Exception("test");
                });
            
            // ACT
            var barTask = proxy.EchoGeneric(Task.FromResult<IBar>(new Bar("test")));

            // ASSERT
            (await barTask.ShouldThrowAsync<Exception>()).Message.ShouldBe("test");
        }
        
        [Fact]
        public async Task GivenTypeViaRedirectConfigured_WhenValueTaskException_ShouldReturnException()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());

            _redirect.ViaRedirect<IBar>();
            
            _redirect
                .To(x => x.EchoGeneric(Is<ValueTask<IBar>>.Any))
                .Via(async () =>
                {
                    await Task.Yield();

                    throw new Exception("test");
                });
            
            // ACT
            var barTask = proxy.EchoGeneric(new ValueTask<IBar>(new Bar("test"))).AsTask();

            // ASSERT
            (await barTask.ShouldThrowAsync<Exception>()).Message.ShouldBe("test");
        }
        
        [Fact]
        public void GivenTypeViaRedirectConfigured_WhenNotTypeReturned_ShouldNotProxy()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());
            _redirect.ViaRedirect<IBar>();
            var number = new Number();
            
            // ACT
            var result = proxy.EchoGeneric<INumber>(number);

            // ASSERT
            result.ShouldBeSameAs(number);
        }
        
        [Fact]
        public void GivenTypeViaRedirectConfigured_WhenReset_ShouldNotPersist()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());
            _redirect.ViaRedirect<IBar>();
            var bar = new Bar("test");

            // ACT
            _redirect.Reset();
            var result = proxy.EchoGeneric<IBar>(bar);

            // ASSERT
            result.ShouldBeSameAs(bar);
        }
        
        [Fact]
        public void GivenTypeViaRedirectConfigured_WhenSameInstanceReturned_ShouldCacheProxy()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo());
            _redirect.ViaRedirect<IBar>();
            var bar = new Bar("test");

            // ACT
            var result1 = proxy.EchoGeneric<IBar>(bar);
            var result2 = proxy.EchoGeneric<IBar>(bar);

            // ASSERT
            result1.GetType().BaseType.ShouldBe(typeof(DiverterDispatchProxy));
            result1.ShouldBeSameAs(result2);
        }
        
        [Fact]
        public void GivenTypeViaRedirectConfigured_WhenEquivalentNamedRedirectConfigured_ShouldConfigureBoth()
        {
            // ARRANGE
            _redirect.ViaRedirect<IBar>().To(x => x.Name).Via(call => call.CallRoot() + " inner");

            // ACT
            var namedRedirect = _redirect.ViaRedirect<IBar>("test");
            namedRedirect.To(x => x.Name).Via(call => call.CallRoot() + " outer");
            var bar = _redirect.Proxy(new Foo()).EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            namedRedirect.RedirectId.Name.ShouldBe("test");
            bar.Name.ShouldBe("bar inner outer");
        }
    }
}
