using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectProxyTests
    {
        [Fact]
        public void GivenStaticallyCreatedRedirectProxyWithTarget_ShouldProxy()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(new Foo());
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBe("original: hello");
        }
        
        [Fact]
        public void GivenStaticallyCreatedRedirectProxyWithNoTarget_ShouldProxy()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>();
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenStaticallyCreatedRedirectProxyWithDummy_ShouldProxy()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(true);
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenStaticallyCreatedRedirectProxyWithoutDummy_WhenProxyCalled_ShouldThrowException()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(false);
            
            // ACT
            var testAction = () => proxy.Echo("hello");

            // ASSERT
            testAction.ShouldThrow<DiverterNullRootException>();
        }
        
        [Fact]
        public void GivenStaticallyCreatedRedirectProxy_WhenStaticRedirectFrom_ShouldReturnRedirect()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(new Foo());
            
            // ACT
            var redirect = Redirect.From(proxy);

            // ASSERT
            redirect.ShouldNotBeNull();
            redirect.ShouldBeOfType<Redirect<IFoo>>();
        }
        
        [Fact]
        public void GivenNonStaticallyCreatedRedirectProxy_WhenStaticRedirectFrom_ShouldReturnRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            var proxy = redirect.Proxy(new Foo());
            
            // ACT
            var fromRedirect = Redirect.From(proxy);

            // ASSERT
            fromRedirect.ShouldBeSameAs(redirect);
        }
        
        [Fact]
        public void GivenStaticRedirectProxyWithStaticallyCreatedVia_WhenProxyCalled_ShouldRedirect()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(new Foo());
            Redirect
                .From(proxy)
                .To(x => x.Echo(Is<string>.Any))
                .Via(call => call.CallNext() + " viaed");

            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBe("original: hello viaed");
        }
        
        [Fact]
        public void GivenRedirectProxy_WhenRedirectFromWithWrongTargetType_ShouldThrowException()
        {
            // ARRANGE
            var proxy = Redirect.Proxy<IFoo>(false);
            
            // ACT
            var testAction = () => Redirect.From(proxy as object);

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void WhenRedirectFromWithInvalidProxy_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => Redirect.From(new object());

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
    }
}