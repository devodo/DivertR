using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class ViaTests
{
    [Fact]
    public void GivenVia_WhenViaAddedToRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Redirect.Proxy<IFoo>(new Foo("MrFoo"));
        Redirect.From(foo).Via(via);
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenVia_WhenViaWithOptionsAddedToRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Redirect.Proxy<IFoo>(new Foo("MrFoo"));
        Redirect.From(foo).Via(via, opt => opt.Repeat(1));
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
        foo.Name.ShouldBe("MrFoo");
    }

    [Fact]
    public void GivenVia_WhenViaAddedToNonTypedRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Redirect.Proxy<IFoo>(new Foo("MrFoo"));
        IRedirect redirect = Redirect.From(foo);
        redirect.Via(via);
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenVia_WhenViaWithOptionsAddedToNonTypedRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Redirect.Proxy<IFoo>(new Foo("MrFoo"));
        IRedirect redirect = Redirect.From(foo);
        redirect.Via(via, opt => opt.Repeat(1));
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
        foo.Name.ShouldBe("MrFoo");
    }
    
    [Fact]
    public void GivenViaWithChainedRecord_WhenProxyCalled_ThenRecordsCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        var foo = Redirect.Proxy<IFoo>(new Foo("MrFoo"));
        var fooCalls = Redirect.From(foo)
            .Via(via, opt => opt.Repeat(1))
            .Record();
        
        // ACT
        var names = Enumerable.Range(0, 2).Select(_ => foo.Name).ToArray();
        
        // ASSERT
        fooCalls
            .To(x => x.Name)
            .Verify()
            .Select(x => x.Returned!.Value)
            .ShouldBe(names);
    }
}