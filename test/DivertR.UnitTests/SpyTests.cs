using DivertR.DynamicProxy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class SpyTests
{
    [Fact]
    public void GivenSpy_WhenMockCalled_ThenRelayToRoot()
    {
        // ARRANGE
        var fooMock = Spy.On<IFoo>(new Foo("test"));
        
        // ACT
        var callResult = fooMock.Name;
        
        // ASSERT
        callResult.ShouldBe("test");
    }
    
    [Fact]
    public void GivenSpy_WhenMockCalled_ThenRecordCall()
    {
        // ARRANGE
        var fooMock = Spy.On<IFoo>(new Foo("test"));
        
        // ACT
        _ = fooMock.Name;
        
        // ASSERT
        Spy.Of(fooMock).Calls.Verify(call => call.Return.ShouldBe("test")).Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenSpyWithNoRoot_WhenMockCalled_ShouldReturnDefault()
    {
        // ARRANGE
        var fooMock = Spy.On<IFoo>();
        
        // ACT
        var callResult = fooMock.Name;
        
        // ASSERT
        callResult.ShouldBeNull();
    }
    
    [Fact]
    public void GivenSpyReset_WhenMockCalled_RecordsCalls()
    {
        // ARRANGE
        var fooSpy = new Spy<IFoo>(new Foo());
        var callsBeforeReset = fooSpy.Calls;
        fooSpy.Reset();
        var callsAfterReset = fooSpy.Calls;
        
        // ACT
        fooSpy.Mock.Echo("test");
        
        // ASSERT
        callsBeforeReset.Count.ShouldBe(0);
        callsAfterReset.Verify(call =>
        {
            call.Args[0].ShouldBe("test");
            call.Return.ShouldBe("original: test");
        }).Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenSpyWithVia_WhenMockCalled_ThenRedirects()
    {
        // ARRANGE
        var fooSpy = new Spy<IFoo>();
        
        fooSpy
            .To(x => x.Echo(Is<string>.Any))
            .Via<(string input, __)>(call => $"{call.Args.input} redirected");
        
        // ACT
        var result = fooSpy.Mock.Echo("test");
        
        // ASSERT
        result.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenSpyWithVia_WhenMockCalled_ThenRecords()
    {
        // ARRANGE
        var fooSpy = new Spy<IFoo>();
        var echoCalls = fooSpy.Calls
            .To(x => x.Echo(Is<string>.Any))
            .Args<(string Input, __)>()
            .Map(call => new
            {
                call.Args.Input,
                call.Return
            });
        
        fooSpy
            .To(x => x.Echo(Is<string>.Any))
            .Via<(string input, __)>(call => $"{call.Args.input} redirected");
        
        // ACT
        fooSpy.Mock.Echo("test");
        
        // ASSERT
        echoCalls.Verify(call =>
        {
            call.Input.ShouldBe("test");
            call.Return.ShouldBe("test redirected");
        }).Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenSpyMockAsObject_WhenSpyOf_ThenThrowsDiverterException()
    {
        // ARRANGE
        object fooMock = Spy.On<IFoo>();
        
        // ACT
        var testAction = () => Spy.Of(fooMock);
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
    
    [Fact]
    public void GivenNonSpyObject_WhenSpyOfObject_ThenThrowsDiverterException()
    {
        // ARRANGE
        IFoo foo = new Foo();
        
        // ACT
        var testAction = () => Spy.Of(foo);
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
    
    [Fact]
    public void GivenDispatchProxyFactory_WhenSpyOnClassType_ThenThrowsDiverterException()
    {
        // ARRANGE

        // ACT
        var testAction = () => Spy.On<Foo>();
        
        // ASSERT
        testAction.ShouldThrow<DiverterException>();
    }
    
    [Fact]
    public void GivenDynamicProxyFactoryClassSpy_WhenVirtualMethodCalled_DoesRecord()
    {
        // ARRANGE
        var fooSpy = new Spy<Foo>(new Foo("MrFoo"), new DiverterSettings(proxyFactory: new DynamicProxyFactory()));

        // ACT
        var result = fooSpy.Mock.NameVirtual;
        
        // ASSERT
        result.ShouldBe("MrFoo");
        fooSpy.Calls.To(x => x.NameVirtual).Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenDynamicProxyFactoryClassSpy_WhenNonVirtualMethodCalled_DoesNotRecord()
    {
        // ARRANGE
        var fooSpy = new Spy<Foo>(new Foo("MrFoo"), new DiverterSettings(proxyFactory: new DynamicProxyFactory()));

        // ACT
        var result = fooSpy.Mock.Name;
        
        // ASSERT
        result.ShouldBe("MrFoo");
        fooSpy.Calls.Count.ShouldBe(0);
    }
}