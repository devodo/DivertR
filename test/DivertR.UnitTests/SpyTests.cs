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
    public void GivenNonGenericSpy_WhenMockCalled_ThenRecordCall()
    {
        // ARRANGE
        ISpy spy = new Spy<IFoo>(new Foo("test"));
        var fooMock = (IFoo) spy.Mock;
        
        // ACT
        _ = fooMock.Name;
        
        // ASSERT
        spy.Calls
            .To(() => Is<string>.Return)
            .Verify(call => call.Return.ShouldBe("test")).Count.ShouldBe(1);
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
    public void GivenSpy_WhenReset_RemovesVia()
    {
        // ARRANGE
        ISpy<IFoo> fooSpy = new Spy<IFoo>(new Foo("MrFoo"));
        fooSpy.To(x => x.Name).Via("rename");
        
        // ACT
        var before = fooSpy.Mock.Name;
        fooSpy.Reset();
        var result = fooSpy.Mock.Name;
        
        // ASSERT
        before.ShouldBe("rename");
        result.ShouldBe("MrFoo");
        fooSpy.Calls.Verify().Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenGenericSpy_WhenReset_ResetsCalls()
    {
        // ARRANGE
        ISpy fooSpy = new Spy<IFoo>();
        var fooMock = (IFoo) fooSpy.Mock;
        _ = fooMock.Name;
        var callsBefore = fooSpy.Calls;
        
        // ACT
        fooSpy.Reset();

        // ASSERT
        callsBefore.Count.ShouldBe(1);
        fooSpy.Calls.Count.ShouldBe(0);
    }
    
    [Fact]
    public void GivenSpyReset_WhenMockCalled_RecordsCalls()
    {
        // ARRANGE
        ISpy<IFoo> fooSpy = new Spy<IFoo>(new Foo());
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
        ISpy<IFoo> fooSpy = new Spy<IFoo>();
        
        fooSpy
            .To(x => x.Echo(Is<string>.Any))
            .Via<(string input, __)>(call => $"{call.Args.input} redirected");
        
        // ACT
        var result = fooSpy.Mock.Echo("test");
        
        // ASSERT
        result.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenSpyWithGlobalVia_WhenMockCalled_ThenRedirects()
    {
        // ARRANGE
        ISpy<IFoo> fooSpy = new Spy<IFoo>();

        var via = ViaBuilder<IFoo>
            .To(x => x.Echo(Is<string>.Any))
            .Build<(string input, __)>(call => $"{call.Args.input} redirected");

        fooSpy.Via(via);
        
        // ACT
        var result = fooSpy.Mock.Echo("test");
        
        // ASSERT
        result.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenNonGenericSpyWithGlobalVia_WhenMockCalled_ThenRedirects()
    {
        // ARRANGE
        ISpy fooSpy = new Spy<IFoo>();
        var fooMock = (IFoo) fooSpy.Mock;

        var via = ViaBuilder<IFoo>
            .To(x => x.Echo(Is<string>.Any))
            .Build<(string input, __)>(call => $"{call.Args.input} redirected");

        fooSpy.Via(via);
        
        // ACT
        var result = fooMock.Echo("test");
        
        // ASSERT
        result.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenSpyWithRetarget_WhenMockCalled_ThenRedirects()
    {
        // ARRANGE
        ISpy<IFoo> fooSpy = new Spy<IFoo>();
        fooSpy.Retarget(new Foo("MrFoo"));
        
        // ACT
        var result = fooSpy.Mock.Name;
        
        // ASSERT
        result.ShouldBe("MrFoo");
    }
    
    [Fact]
    public void GivenSpyWithStrict_WhenMockCalled_ThenThrowsException()
    {
        // ARRANGE
        ISpy<IFoo> fooSpy = new Spy<IFoo>();
        fooSpy.Strict();
        
        // ACT
        var testAction = () => fooSpy.Mock.Name;
        
        // ASSERT
        testAction.ShouldThrow<StrictNotSatisfiedException>();
    }
    
    [Fact]
    public void GivenNonGenericSpyWithStrict_WhenMockCalled_ThenThrowsException()
    {
        // ARRANGE
        ISpy fooSpy = new Spy<IFoo>();
        fooSpy.Strict();
        var fooMock = (IFoo) fooSpy.Mock;
        
        // ACT
        var testAction = () => fooMock.Name;
        
        // ASSERT
        testAction.ShouldThrow<StrictNotSatisfiedException>();
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