using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaProxyTests
    {
        [Fact]
        public void GivenStaticallyCreatedViaProxyWithTarget_ShouldProxy()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(new Foo());
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBe("original: hello");
        }
        
        [Fact]
        public void GivenStaticallyCreatedViaProxyWithNoTarget_ShouldProxy()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>();
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenStaticallyCreatedViaProxyWithDummy_ShouldProxy()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(true);
            
            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenStaticallyCreatedViaProxyWithoutDummy_WhenProxyCalled_ShouldThrowException()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(false);
            
            // ACT
            var testAction = () => proxy.Echo("hello");

            // ASSERT
            testAction.ShouldThrow<DiverterNullRootException>();
        }
        
        [Fact]
        public void GivenStaticallyCreatedViaProxy_WhenStaticViaFrom_ShouldReturnVia()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(new Foo());
            
            // ACT
            var via = Via.From(proxy);

            // ASSERT
            via.ShouldNotBeNull();
            via.ShouldBeOfType<Via<IFoo>>();
        }
        
        [Fact]
        public void GivenStaticViaProxyWithStaticallyCreatedRedirect_WhenProxyCalled_ShouldRedirect()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(new Foo());
            Via
                .From(proxy)
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(call => call.CallNext() + " redirected");

            // ACT
            var result = proxy.Echo("hello");

            // ASSERT
            result.ShouldBe("original: hello redirected");
        }
        
        [Fact]
        public void GivenViaProxy_WhenViaFromWithWrongTargetType_ShouldThrowException()
        {
            // ARRANGE
            var proxy = Via.Proxy<IFoo>(false);
            
            // ACT
            var testAction = () => Via.From(proxy as object);

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void WhenViaFromWithInvalidProxy_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => Via.From(new object());

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
    }
}