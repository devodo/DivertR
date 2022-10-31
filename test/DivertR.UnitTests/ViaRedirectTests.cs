using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class ViaRedirectTests
{
    [Fact]
    public void GivenRedirect_WhenRedirectAddedToVia_ThenRedirectProxyCalls()
    {
        // ARRANGE
        var redirect = RedirectBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Via.Proxy<IFoo>(new Foo("MrFoo"));
        Via.From(foo).Redirect(redirect);
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenRedirect_WhenRedirectWithOptionsAddedToVia_ThenRedirectProxyCalls()
    {
        // ARRANGE
        var redirect = RedirectBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Via.Proxy<IFoo>(new Foo("MrFoo"));
        Via.From(foo).Redirect(redirect, opt => opt.Repeat(1));
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
        foo.Name.ShouldBe("MrFoo");
    }

    [Fact]
    public void GivenRedirect_WhenRedirectAddedToNonTypedVia_ThenRedirectProxyCalls()
    {
        // ARRANGE
        var redirect = RedirectBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Via.Proxy<IFoo>(new Foo("MrFoo"));
        IVia via = Via.From(foo);
        via.Redirect(redirect);
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenRedirect_WhenRedirectWithOptionsAddedToNonTypedVia_ThenRedirectProxyCalls()
    {
        // ARRANGE
        var redirect = RedirectBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        var foo = Via.Proxy<IFoo>(new Foo("MrFoo"));
        IVia via = Via.From(foo);
        via.Redirect(redirect, opt => opt.Repeat(1));
        
        // ASSERT
        foo.Name.ShouldBe("MrFoo diverted");
        foo.Name.ShouldBe("MrFoo");
    }
    
    [Fact]
    public void GivenRedirectWithChainedRecord_WhenProxyCalled_ThenRecordsCalls()
    {
        // ARRANGE
        var redirect = RedirectBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        var foo = Via.Proxy<IFoo>(new Foo("MrFoo"));
        var fooCalls = Via.From(foo)
            .Redirect(redirect, opt => opt.Repeat(1))
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