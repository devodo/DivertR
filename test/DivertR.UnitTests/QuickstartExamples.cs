using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.DependencyInjection;
using DivertR.UnitTests.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DivertR.UnitTests;

public class QuickstartExamples
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

        // Proxy calls and be recorded for verifying
        var fooCalls = fooRedirect.Record();
        
        Assert.Equal("MrFoo", fooProxy.Name);
        Assert.Equal("FooTwo", fooTwoProxy.Name);
        Assert.Null(fooMock.Echo("test"));
        
        // The recording is a collection containing details of the calls
        Assert.Equal(3, fooCalls.Count);
        
        // This can be filtered with expressions for verifying
        Assert.Equal(1, fooCalls.To(x => x.Echo(Is<string>.Any)).Count);

        // Take an immutable snapshot of the currently recorded calls to verify against
        var snapshotCalls = fooCalls.To(x => x.Name).Verify();
        Assert.Equal(2, snapshotCalls.Count);

        Assert.Equal("MrFoo", snapshotCalls[0].Return);
        Assert.Equal("FooTwo", snapshotCalls[1].Return);
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
        
        // Perform a reset across all Redirects in the set
        redirectSet.ResetAll();
        // Or across a subset by name
        redirectSet.Reset("GroupX");
    }
    
    [Fact]
    public async Task SpyTestSample()
    {
        // Create an IFoo mock
        var fooMock = Spy.On<IFoo>();
        // By default mocks return dummy values (C# defaults)
        Assert.Null(fooMock.Name);
        // For async methods a Task is returned wrapping a dummy result
        Assert.Null(await fooMock.EchoAsync("test"));
        
        // Mock behaviour can be configured by adding Vias using the usual redirect fluent syntax
        Spy.Of(fooMock)
            .To(x => x.Name)
            .Via(() => "redirected");

        // Mock calls are redirected to the Via delegates
        Assert.Equal("redirected", fooMock.Name);
        
        // The spy records all mock calls
        Assert.Equal(3, Spy.Of(fooMock).Calls.Count);
        // These can be filtered with expressions for verifying
        Assert.Equal(1, Spy.Of(fooMock).Calls.To(x => x.EchoAsync(Is<string>.Any)).Count);
        
        // Spies can be reset
        Spy.Of(fooMock).Reset();
        // This resets recorded calls
        Assert.Equal(0, Spy.Of(fooMock).Calls.Count);
        // And removes all Via configurations
        Assert.Null(fooMock.Name);
    }

    [Fact]
    public void SpyReadmeExample()
    {
        IFoo fooMock = Spy.On<IFoo>();
        
        Spy.Of(fooMock)
            .To(x => x.Name)
            .Via(() => "redirected");
        
        Assert.Equal("redirected", fooMock.Name);
    }

    [Fact]
    public async Task SpyInstantiationAndUsage()
    {
        // Instantiate an IFoo spy
        ISpy<IFoo> fooSpy = new Spy<IFoo>();
        // The Mock property is the spy's proxy object
        IFoo fooMock = fooSpy.Mock;
        // Out the box spy proxies return dummy values (C# defaults)
        Assert.Null(fooMock.Name);
        // For async methods a Task is returned wrapping a dummy result
        Assert.Null(await fooMock.EchoAsync("test"));
        
        // Proxy behaviour can be configured using the usual redirect fluent syntax
        fooSpy.To(x => x.Name).Via(() => "Hello spy");
        // Now matching calls are redirected to the Via delegate
        Assert.Equal("Hello spy", fooMock.Name);
        
        // Proxy calls are recorded to the Calls property
        Assert.Equal(3, fooSpy.Calls.Count);
        // Recorded calls can be filtered and verified
        Assert.Equal(1, fooSpy.Calls.To(x => x.EchoAsync(Is<string>.Any)).Count);
    }
    
    [Fact]
    public void SpyProxyRoot()
    {
        IFoo fooRoot = new Foo("MrFoo");
        Assert.Equal("MrFoo", fooRoot.Name);

        // Specify the proxy root at creation
        var fooSpy = new Spy<IFoo>(fooRoot);
        // By default proxy calls are relayed to the root
        Assert.Equal("MrFoo", fooSpy.Mock.Name);
    }
    
    [Fact]
    public void SpyRetarget()
    {
        IFoo fooRoot = new Foo("MrFoo");
        Assert.Equal("MrFoo", fooRoot.Name);

        // Create spy without proxy root
        var fooSpy = new Spy<IFoo>();
        Assert.Null(fooSpy.Mock.Name);
        
        // Retarget to a new proxy root
        fooSpy.Retarget(fooRoot);
        
        // Proxy calls are now relayed to the set target
        Assert.Equal("MrFoo", fooSpy.Mock.Name);
    }
    
    [Fact]
    public void SpyReset()
    {
        var fooSpy = new Spy<IFoo>();

        fooSpy.To(x => x.Name).Via(() => "redirected");
        Assert.Equal("redirected", fooSpy.Mock.Name);
        Assert.Equal(1, fooSpy.Calls.Count);
        
        // Reset spy
        fooSpy.Reset();
        
        // Call counts are reset
        Assert.Equal(0, fooSpy.Calls.Count);
        // And configured Vias removed
        Assert.Null(fooSpy.Mock.Name);
    }
    
    [Fact]
    public void SpyStaticShorthand()
    {
        var fooRoot = new Foo("MrFoo");
        
        // Create a spy proxy with optional root using the Spy.On static method
        IFoo fooProxy = Spy.On<IFoo>(fooRoot);
        
        // The proxy instance relays calls to the root
        Assert.Equal("MrFoo", fooProxy.Name);
        
        // Use the Spy.Of static method to access and update spy configuration
        Spy.Of(fooProxy)
            .To(x => x.Name)
            .Via(call => call.CallNext() + " spied");
        
        Assert.Equal("MrFoo spied", fooProxy.Name);
        Assert.Equal(2, Spy.Of(fooProxy).Calls.Count);
    }
    
    private interface IEtc
    {
    }
    
    private class Etc : IEtc
    {
    }
}