using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class RedirectStaticTests
{
    [Fact]
    public void GivenProxy_WhenProxyMethodCalled_ThenRelayToRoot()
    {
        // ARRANGE
        var proxy = Redirect.Proxy<IFoo>(new Foo("test"));
        
        // ACT
        var callResult = proxy.Name;
        
        // ASSERT
        callResult.ShouldBe("test");
    }
    
    [Fact]
    public void GivenProxyWithNoRoot_WhenProxyCalled_ShouldReturnDefault()
    {
        // ARRANGE
        var proxy = Redirect.Proxy<IFoo>();
        
        // ACT
        var callResult = proxy.Name;
        
        // ASSERT
        callResult.ShouldBeNull();
    }
    
    [Fact]
    public void GivenProxyWithVia_WhenMockCalled_ThenRedirects()
    {
        // ARRANGE
        var proxy = Redirect.Proxy<IFoo>();
        var redirect = Redirect.Of(proxy);
        
        redirect
            .To(x => x.Echo(Is<string>.Any))
            .Via<(string input, __)>(call => $"{call.Args.input} redirected");
        
        // ACT
        var result = proxy.Echo("test");
        
        // ASSERT
        result.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenProxyAsObject_WhenRedirectOf_ThenThrowsDiverterException()
    {
        // ARRANGE
        object fooProxy = Redirect.Proxy<IFoo>();
        
        // ACT
        var testAction = () => Redirect.Of(fooProxy);
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
    
    [Fact]
    public void GivenNonProxyObject_WhenRedirectOfObject_ThenThrowsDiverterException()
    {
        // ARRANGE
        IFoo foo = new Foo();
        
        // ACT
        var testAction = () => Redirect.Of(foo);
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
    
    [Fact]
    public void GivenDispatchProxyFactory_WhenSpyOnClassType_ThenThrowsDiverterException()
    {
        // ARRANGE

        // ACT
        var testAction = () => Redirect.Proxy<Foo>();
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
}