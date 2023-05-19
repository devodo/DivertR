using DivertR.DependencyInjection;
using DivertR.UnitTests.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class DiverterRedirectBuilderTests
{
    private readonly IDiverterRedirectBuilder<IFoo> _builder;
    private readonly IDiverter _diverter;
    private readonly IFoo _fooProxy;
    
    public DiverterRedirectBuilderTests()
    {
        var diverterBuilder = new DiverterBuilder().Register<IFoo>();
        _builder = diverterBuilder.Redirect<IFoo>();
        _diverter = diverterBuilder.Create();

        var serviceProvider = new ServiceCollection()
            .AddTransient<IFoo, Foo>()
            .Divert(_diverter)
            .BuildServiceProvider();

        _fooProxy = serviceProvider.GetRequiredService<IFoo>();
    }
    
    [Fact]
    public void GivenToFuncViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.Name).Via(call => call.CallNext() + " redirected");
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("original redirected");
    }
    
    [Fact]
    public void GivenToFuncViaRegisteredWithOptions_ShouldApplyOptions()
    {
        // ARRANGE
        _builder
            .To(x => x.Name)
            .Via(call => call.CallNext() + " redirected", opt => opt.Persist(false));
        
        _diverter.ResetAll(); // builder changes should not be persistant now

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("original");
    }
    
    [Fact]
    public void GivenToFuncViaArgsRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .To(x => x.Echo(Is<string>.Any))
            .Via<(string echo, __)>(call => call.CallNext() + $" redirected {call.Args.echo}");
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Echo("test");
            
        // ASSERT
        result.ShouldBe("original: test redirected test");
    }
    
    [Fact]
    public void GivenToFuncViaInstanceRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.Name).Via("redirected");
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("redirected");
    }
    
    [Fact]
    public void GivenToFuncViaDelegateRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.Name).Via(() => "redirected");
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("redirected");
    }
    
    [Fact]
    public void GivenToFuncViaCallHandlerRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.Name)
            .Via(new DelegateCallHandler(call => call.CallNext() + " redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("original redirected");
    }
    
    [Fact]
    public void GivenToFuncViaRetargetRegistered_ShouldRedirect()
    {
        // ARRANGE
        var altFoo = new Foo("alt");
        
        _builder
            .To(x => x.Echo("test"))
            .Retarget(altFoo);
            
        _diverter.ResetAll(); // builder changes should be persistant
        
        // ACT
        var result1 = _fooProxy.Echo("test");
        var result2 = _fooProxy.Echo("test2");
        
        // ASSERT
        result1.ShouldBe("alt: test");
        result2.ShouldBe("original: test2");
    }
    
    [Fact]
    public void GivenToFuncViaWithFilterRegistered_ShouldFilter()
    {
        // ARRANGE
        _builder
            .To(x => x.Echo(Is<string>.Any))
            .Filter(new DelegateCallConstraint(call => (string) call.Arguments[0] == "test"))
            .Via(call => call.CallNext() + " redirected");
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result1 = _fooProxy.Echo("test");
        var result2 = _fooProxy.Echo("test2");
            
        // ASSERT
        result1.ShouldBe("original: test redirected");
        result2.ShouldBe("original: test2");
    }
    
    [Fact]
    public void GivenToActionViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .To(x => x.SetName(Is<string>.Any))
            .Via(call => call.Next.SetName($"{call.Args[0]} redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.SetName("test");
            
        // ASSERT
        _fooProxy.Name.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenToActionViaWithFilterRegistered_ShouldFilter()
    {
        // ARRANGE
        _builder
            .To(x => x.SetName(Is<string>.Any))
            .Filter(new DelegateCallConstraint(call => (string) call.Arguments[0] == "test"))
            .Via(call => call.Next.SetName($"{call.Args[0]} redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.SetName("test");
        var testName = _fooProxy.Name;
        _fooProxy.SetName("test2");
        var filteredName = _fooProxy.Name;
            
        // ASSERT
        testName.ShouldBe("test redirected");
        filteredName.ShouldBe("test2");
    }
    
    [Fact]
    public void GivenToActionViaArgsRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .To(x => x.SetName(Is<string>.Any))
            .Via<(string name, __)>(call => call.Next.SetName($"{call.Args.name} redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.SetName("test");
            
        // ASSERT
        _fooProxy.Name.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenToActionViaActionRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .To(x => x.SetName(Is<string>.Any))
            .Via(() =>
            {
                var call = _builder.Redirect.Relay.GetCurrentCall();
                _builder.Redirect.Relay.Next.SetName($"{call.Args[0]} redirected");
            });
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.SetName("test");
            
        // ASSERT
        _fooProxy.Name.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenToActionViaCallHandlerRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.SetName(Is<string>.Any))
            .Via(new DelegateCallHandler(call => call.CallNext(new[] { $"{call.Args[0]} redirected" })));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.SetName("test");
            
        // ASSERT
        _fooProxy.Name.ShouldBe("test redirected");
    }

    [Fact]
    public void GivenToActionViaRetargetRegistered_ShouldRedirect()
    {
        // ARRANGE
        var spyFoo = Spy.On<IFoo>();
        
        _builder
            .To(x => x.SetName(Is<string>.Any))
            .Retarget(spyFoo);
            
        _diverter.ResetAll(); // builder changes should be persistant
        
        // ACT
        _fooProxy.SetName("test");
        var name = _fooProxy.Name;
        
        // ASSERT
        name.ShouldBe("original");
        Spy.Of(spyFoo).Calls.Count.ShouldBe(1);
        Spy.Of(spyFoo).Calls.To(x => x.SetName("test")).Count.ShouldBe(1);
    }
    
    [Fact]
    public void GivenToRetarget_ShouldRetarget()
    {
        // ARRANGE
        var altFoo = new Foo("alt");
        _builder.To().Retarget(altFoo);
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("alt");
    }
    
    [Fact]
    public void GivenToVia_ShouldRedirect()
    {
        // ARRANGE
        _builder.To().Via(new DelegateCallHandler(call => call.CallNext() + " redirected"));
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("original redirected");
    }
    
    [Fact]
    public void GivenToViaWithConstraint_ShouldFilterRedirect()
    {
        // ARRANGE
        _builder
            .To(new DelegateCallConstraint(call => (string) call.Arguments[0] == "test"))
            .Via(new DelegateCallHandler(call => call.CallNext() + " redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result1 = _fooProxy.Echo("test");
        var result2 = _fooProxy.Echo("test2");
            
        // ASSERT
        result1.ShouldBe("original: test redirected");
        result2.ShouldBe("original: test2");
    }
    
    [Fact]
    public void GivenToViaWithFilter_ShouldFilterRedirect()
    {
        // ARRANGE
        _builder
            .To()
            .Filter(new DelegateCallConstraint(call => (string) call.Arguments[0] == "test"))
            .Via(new DelegateCallHandler(call => call.CallNext() + " redirected"));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result1 = _fooProxy.Echo("test");
        var result2 = _fooProxy.Echo("test2");
            
        // ASSERT
        result1.ShouldBe("original: test redirected");
        result2.ShouldBe("original: test2");
    }
    
    [Fact]
    public void GivenRetarget_ShouldRetarget()
    {
        // ARRANGE
        var altFoo = new Foo("alt");
        _builder.Retarget(altFoo);
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("alt");
    }
    
    [Fact]
    public void GivenViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.Via(new Via(new DelegateCallHandler(call => call.CallNext() + " redirected")));
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.Name;
            
        // ASSERT
        result.ShouldBe("original redirected");
    }
    
    [Fact]
    public void GivenToSetViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .ToSet(x => x.Name)
            .Via(call => call.CallNext(new[] { call.Args[0] + " redirected" }));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.Name = "test";
            
        // ASSERT
        _fooProxy.Name.ShouldBe("test redirected");
    }
    
    [Fact]
    public void GivenToSetWithConstraintViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder
            .ToSet(x => x.Name, () => "test")
            .Via(call => call.CallNext(new[] { call.Args[0] + " redirected" }));
        
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        _fooProxy.Name = "test";
        var result1 = _fooProxy.Name;
        _fooProxy.Name = "test2";
        var result2 = _fooProxy.Name;
            
        // ASSERT
        result1.ShouldBe("test redirected");
        result2.ShouldBe("test2");
    }
    
    [Fact]
    public void GivenToStructViaRegistered_ShouldRedirect()
    {
        // ARRANGE
        _builder.To(x => x.EchoGeneric(Is<int>.Any)).Via(call => call.CallNext() + 1);
        _diverter.ResetAll(); // builder changes should be persistant

        // ACT
        var result = _fooProxy.EchoGeneric(10);
            
        // ASSERT
        result.ShouldBe(11);
    }
}