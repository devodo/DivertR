---
layout: default
title: Quickstart
nav_order: 2
has_children: true
---

# Quickstart

## Installing

Install DivertR as a [NuGet package](https://www.nuget.org/packages/DivertR):

```sh
Install-Package DivertR
```

Or via the .NET command line interface:

```sh
dotnet add package DivertR
```

## Creating Proxies

The `Redirect<TTarget>` class is used to create and manage DivertR proxies. Its basic usage is similar to other common mocking frameworks:


```csharp
using DivertR;

public class QuickstartExample
{
    [Fact]
    public void RedirectTestSample()
    {
        // Create a Foo instance named "MrFoo"
        IFoo foo = new Foo("MrFoo");
        Assert.Equal("MrFoo", foo.Name);
        
        // Create an IFoo Redirect
        var fooRedirect = new Redirect<IFoo>();
        
        // Use the Redirect to create an IFoo proxy that wraps the Foo instance above as its root target
        IFoo fooProxy = fooRedirect.Proxy(foo);
        
        // By default proxies transparently forward calls to their root targets
        Assert.Equal("MrFoo", fooProxy.Name);
        
        // Intercept proxy calls and change behaviour by adding one or more 'Via' delegates to the Redirect
        fooRedirect
            .To(x => x.Name)
            .Via(() => "redirected");
        
        // The Redirect diverts proxy calls to its Via delegates
        Assert.Equal("redirected", fooProxy.Name);
        
        // Reset the Redirect and revert the proxy to its default transparent behaviour
        fooRedirect.Reset();
        Assert.Equal("MrFoo", fooProxy.Name);
        
        // Via delegates can access call context and e.g. relay the call to the root target
        fooRedirect
            .To(x => x.Name)
            .Via(call => call.CallRoot() + " redirected");
        
        Assert.Equal("MrFoo redirected", fooProxy.Name);
        
        // A Redirect can create any number of proxies
        var fooTwo = new Foo("FooTwo");
        IFoo fooTwoProxy = fooRedirect.Proxy(fooTwo);
        
        // Vias added to the Redirect are applied to all its proxies
        Assert.Equal("FooTwo redirected", fooTwoProxy.Name);
        
        // Reset is applied to all proxies.
        fooRedirect.Reset();
        Assert.Equal("MrFoo", fooProxy.Name);
        Assert.Equal("FooTwo", fooTwoProxy.Name);
        
        // A proxy with no root target returns default values
        var fooMock = fooRedirect.Proxy();
        Assert.Null(fooMock.Name);
        
        // Record and verify proxy calls
        var fooCalls = fooRedirect.Record();

        Assert.Equal("MrFoo", fooProxy.Name);
        Assert.Equal("FooTwo", fooTwoProxy.Name);
        Assert.Null(fooMock.Name);
        
        // Take an immutable snapshot of the currently recorded calls to verify against
        var snapshotCalls = fooCalls.To(x => x.Name).Verify();
        Assert.Equal(3, snapshotCalls.Count);
        
        Assert.Equal("MrFoo", snapshotCalls[0].Return);
        Assert.Equal("FooTwo", snapshotCalls[1].Return);
        Assert.Null(snapshotCalls[2].Return);
        
        // Calls are recorded across all of the Redirect's proxies
        Assert.Same(fooProxy, snapshotCalls[0].CallInfo.Proxy);
        Assert.Same(fooTwoProxy, snapshotCalls[1].CallInfo.Proxy);
        Assert.Same(fooMock, snapshotCalls[2].CallInfo.Proxy);
    }
}
```

## Dependency Injection Integration

DivertR is designed to be embedded easily and transparently into dependency injection containers like `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

```csharp

[Fact]
public void ServiceCollectionDemoTest()
{
    // Instantiate a Microsoft.Extensions.DependencyInjection.IServiceCollection
    IServiceCollection services = new ServiceCollection();

    // Register some services
    services.AddTransient<IFoo, Foo>();
    services.AddSingleton<IBarFactory, BarFactory>();
    services.AddSingleton<IEtc, Etc>();

    // Build a Diverter instance by registering services you want to be able to redirect
    var diverter = new DiverterBuilder()
        .Register<IFoo>()
        .Register<IBarFactory>()
        .Create();

    // Install Diverter into the ServiceCollection
    services.Divert(diverter);

    // Build an IServiceProvider as usual
    IServiceProvider provider = services.BuildServiceProvider();

    // Resolve services from the ServiceProvider as usual
    var foo = provider.GetRequiredService<IFoo>();
    var fooTwo = provider.GetRequiredService<IFoo>();

    // In its initial state DivertR is transparent and the behaviour of resolved services is unchanged
    fooTwo.Name = "FooTwo";
    Assert.Equal("original", foo.Name);
    Assert.Equal("FooTwo", fooTwo.Name);

    // Get a Redirect from the Diverter instance and configure a Via
    diverter
        .Redirect<IFoo>()
        .To(x => x.Name)
        .Via(call => $"{call.Next.Name} redirected");

    // The behaviour of resolved service instances is now changed
    Assert.Equal("original redirected", foo.Name);
    Assert.Equal("FooTwo redirected", fooTwo.Name);

    // Reset the Diverter instance
    diverter.ResetAll();

    // The original service behaviour is restored
    Assert.Equal("original", foo.Name);
    Assert.Equal("FooTwo", fooTwo.Name);
}
```

## WebApplicationFactory Integration

DivertR is also designed to integrate with Microsoft's [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests) and facilitates writing tests on a wired-up system like this:

```csharp
[Fact]
public async Task GivenFooExistsInRepo_WhenGetFoo_ThenReturnsFoo_WithOk200()
{
    // ARRANGE
    var foo = new Foo
    {
        Id = Guid.NewGuid(),
        Name = "Foo123"
    };

    _diverter
        .Redirect<IFooRepository>() // Redirect IFooRepository calls 
        .To(x => x.GetFooAsync(foo.Id)) // matching this method and argument
        .Via(() => Task.FromResult(foo)); // via this delegate

    // ACT
    var response = await _fooClient.GetFooAsync(foo.Id);
    
    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    response.Content.Id.ShouldBe(foo.Id);
    response.Content.Name.ShouldBe(foo.Name);
}

[Fact]
public async Task GivenFooRepoException_WhenGetFoo_ThenReturns500InternalServerError()
{
    // ARRANGE
    _diverter
        .Redirect<IFooRepository>()
        .To(x => x.GetFooAsync(Is<Guid>.Any))
        .Via(() => throw new Exception());

    // ACT
    var response = await _fooClient.GetFooAsync(Guid.NewGuid());

    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
}
```

For more examples and a demonstration of setting up a test harness for a WebApp like this see a [WebApp Testing Sample here](https://github.com/devodo/DivertR/tree/main/test/DivertR.WebAppTests).

Continue with [Documentation](../documentation/) for more details.