using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests;

public class RedirectSetTests
{
    private readonly IRedirectSet _redirectSet;

    public RedirectSetTests()
    {
        _redirectSet = new RedirectSet();
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetOrAddRedirect_ThenReturnsExistingFooRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();
        
        // ACT
        var testRedirect = _redirectSet.GetOrCreate<IFoo>();
        
        // ASSERT
        testRedirect.ShouldNotBeNull();
        testRedirect.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithNamedFooRedirectAdded_WhenGetOrAddSameNamedRedirect_ThenReturnsExistingFooRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>("test");
        
        // ACT
        var testRedirect = _redirectSet.GetOrCreate<IFoo>("test");
        
        // ASSERT
        testRedirect.ShouldNotBeNull();
        testRedirect.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetOrAddNamedRedirect_ThenCreatesNewFooRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();
        
        // ACT
        var testRedirect = _redirectSet.GetOrCreate<IFoo>("test");
        
        // ASSERT
        testRedirect.ShouldNotBeNull();
        testRedirect.ShouldNotBe(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetOrAddRedirectByType_ThenReturnsExistingFooRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();
        
        // ACT
        var testRedirect = _redirectSet.GetOrCreate(typeof(IFoo));
        
        // ASSERT
        testRedirect.ShouldNotBeNull();
        testRedirect.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenEmptyRedirectSet_WhenGetRedirect_ThenReturnsNull()
    {
        // ARRANGE

        // ACT
        var result = _redirectSet.Get<IFoo>();
        
        // ASSERT
        result.ShouldBeNull();
    }
    
    [Fact]
    public void GivenEmptyRedirectSet_WhenGetRedirectByType_ThenReturnsNull()
    {
        // ARRANGE

        // ACT
        var result = _redirectSet.Get(typeof(IFoo));
        
        // ASSERT
        result.ShouldBeNull();
    }
    
    [Fact]
    public void GivenEmptyRedirectSet_WhenGetRedirectById_ThenReturnsNull()
    {
        // ARRANGE

        // ACT
        var result = _redirectSet.Get(RedirectId.From<IFoo>());
        
        // ASSERT
        result.ShouldBeNull();
    }
    
    [Fact]
    public void GivenRedirectSetWithNamedFooRedirectAdded_WhenGetRedirect_ThenReturnsNull()
    {
        // ARRANGE
        _redirectSet.GetOrCreate<IFoo>("test");

        // ACT
        var result = _redirectSet.Get<IFoo>();
        
        // ASSERT
        result.ShouldBeNull();
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetRedirect_ThenReturnsRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();

        // ACT
        var result = _redirectSet.Get<IFoo>();
        
        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithNamedFooRedirectAdded_WhenGetNamedRedirect_ThenReturnsRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>("test");

        // ACT
        var result = _redirectSet.Get<IFoo>("test");
        
        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetRedirectByType_ThenReturnsRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();

        // ACT
        var result = _redirectSet.Get(typeof(IFoo));
        
        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithFooRedirectAdded_WhenGetRedirectById_ThenReturnsRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>();

        // ACT
        var result = _redirectSet.Get(RedirectId.From<IFoo>());
        
        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(fooRedirect);
    }
    
    [Fact]
    public void GivenRedirectSetWithNamedFooRedirectAdded_WhenGetNamedRedirectByType_ThenReturnsRedirect()
    {
        // ARRANGE
        var fooRedirect = _redirectSet.GetOrCreate<IFoo>("test");

        // ACT
        var result = _redirectSet.Get(typeof(IFoo), "test");
        
        // ASSERT
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(fooRedirect);
    }
}