using System;
using DivertR.DependencyInjection;
using DivertR.UnitTests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DivertR.UnitTests;

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

    [Fact]
    public void ServiceCollectionDemoTest()
    {
        // Instantiate a Microsoft.Extensions.DependencyInjection.IServiceCollection
        IServiceCollection services = new ServiceCollection();
    
        // Register some services
        services.AddTransient<IFoo, Foo>();
        services.AddSingleton<IBarFactory, BarFactory>();
        services.AddSingleton<IEtc, Etc>();
    
        // Instantiate a Diverter instance
        var diverter = new Diverter();
    
        // Register the services you want to be able to redirect
        diverter    
            .Register<IFoo>()
            .Register<IBarFactory>();
    
        // Install DivertR into the ServiceCollection
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

    [Fact]
    public void RedirectIdExamples1()
    {
        var fooRedirect = new Redirect<IFoo>();
        
        Assert.Equal(typeof(IFoo), fooRedirect.RedirectId.Type);
        Assert.Equal(string.Empty, fooRedirect.RedirectId.Name);
        Assert.Equal(fooRedirect.RedirectId, new RedirectId(typeof(IFoo)));
        
        var fooRedirect2 = new Redirect<IFoo>("GroupX");
        
        Assert.Equal(typeof(IFoo), fooRedirect2.RedirectId.Type);
        Assert.Equal("GroupX", fooRedirect2.RedirectId.Name);
        Assert.Equal(fooRedirect2.RedirectId, new RedirectId(typeof(IFoo), "GroupX"));
    }

    [Fact]
    public void RedirectSetExamples2()
    {
        // Instantiate a new RedirectSet
        IRedirectSet redirectSet = new RedirectSet();
        // Create and store a Redirect instance
        IRedirect<IFoo> fooRedirect = redirectSet.GetOrCreate<IFoo>();
        // The Redirect has already been created so the existing instance is returned
        IRedirect<IFoo> fooRedirect2 = redirectSet.GetOrCreate<IFoo>();

        Assert.Same(fooRedirect, fooRedirect2);

        // Create and store sets of Redirects for different target types
        var barRedirect = redirectSet.GetOrCreate<IBar>();
        // Or multiple with the same type using names
        var fooRedirect3 = redirectSet.GetOrCreate<IFoo>("GroupX");

        Assert.NotNull(barRedirect);
        Assert.NotEqual(fooRedirect, fooRedirect3);
        
        // Perform a Redirect action across all Redirects in the set
        redirectSet.ResetAll();
        // Or across a subset by name
        redirectSet.Reset("GroupX");
    }

    private interface IEtc
    {
    }
    
    private class Etc : IEtc
    {
    }
}