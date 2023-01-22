using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class ViaTests
{
    private readonly IRedirect<IFoo> _fooRedirect = new Redirect<IFoo>();
    private readonly IFoo _fooProxy;

    public ViaTests()
    {
        _fooProxy = _fooRedirect.Proxy(new Foo("MrFoo"));
    }
    
    [Fact]
    public void GivenVia_WhenViaAddedToRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        _fooRedirect.Via(via);
        
        // ASSERT
        _fooProxy.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenVia_WhenViaWithOptionsAddedToRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        _fooRedirect.Via(via, opt => opt.Repeat(1));
        
        // ASSERT
        _fooProxy.Name.ShouldBe("MrFoo diverted");
        _fooProxy.Name.ShouldBe("MrFoo");
    }

    [Fact]
    public void GivenVia_WhenViaAddedToNonTypedRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        _fooRedirect.Via(via);
        
        // ASSERT
        _fooProxy.Name.ShouldBe("MrFoo diverted");
    }
    
    [Fact]
    public void GivenVia_WhenViaWithOptionsAddedToNonTypedRedirect_ThenViaProxyCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        // ACT
        _fooRedirect.Via(via, opt => opt.Repeat(1));
        
        // ASSERT
        _fooProxy.Name.ShouldBe("MrFoo diverted");
        _fooProxy.Name.ShouldBe("MrFoo");
    }
    
    [Fact]
    public void GivenViaWithChainedRecord_WhenProxyCalled_ThenRecordsCalls()
    {
        // ARRANGE
        var via = ViaBuilder<IFoo>
            .To(x => x.Name)
            .Build(call => call.CallNext() + " diverted");
        
        var fooCalls = _fooRedirect
            .Via(via, opt => opt.Repeat(1))
            .Record();
        
        // ACT
        var names = Enumerable.Range(0, 2).Select(_ => _fooProxy.Name).ToArray();
        
        // ASSERT
        fooCalls
            .To(x => x.Name)
            .Verify()
            .Select(x => x.Return)
            .ShouldBe(names);
    }
}