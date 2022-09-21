using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DivertR.DependencyInjection;
using DivertR.UnitTests.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IDiverter _diverter = new Diverter().Register<IFoo>();

        [Fact]
        public void ShouldReplaceTypeRegistration()
        {
            _services.AddSingleton<IFoo, Foo>();
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            _diverter.Via<IFoo>().To(x => x.Name).Redirect("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceInstanceRegistration()
        {
            _services.AddSingleton<IFoo>(new Foo());
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            _diverter.Via<IFoo>().To(x => x.Name).Redirect("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceFactoryRegistration()
        {
            _services.AddSingleton<IFoo>(_ => new Foo());
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            _diverter.Via<IFoo>().To(x => x.Name).Redirect("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenServiceTypeMissingShouldThrowException()
        {
            Action test = () => _services.Divert(_diverter);
            test.ShouldThrow<DiverterException>().Message.ShouldContain($"{typeof(IFoo).FullName}");
        }
        
        [Fact]
        public void ShouldReplaceMultipleRegistrations()
        {
            var fooRegistrations = Enumerable.Range(0, 10)
                .Select((_, i) => new Foo($"Foo{i}")).ToList();
            fooRegistrations.ForEach(foo => _services.AddSingleton<IFoo>(foo));
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();

            _diverter.Via<IFoo>()
                .To(x => x.Name)
                .Redirect(() => "Diverted: " + _diverter.Via<IFoo>().Relay.Next.Name);
            
            var fooInstances = provider.GetServices<IFoo>().ToList();
            
            fooInstances.Select(x => x.Name).ShouldBe(fooRegistrations.Select(foo => "Diverted: " + foo.Name));
        }
        
        [Fact]
        public void GivenResolvedInstancesBeforeAndAfterRegisteringRedirect_ShouldRedirect()
        {
            var via = _diverter.Via<IFoo>();
            
            _services.AddTransient<IFoo, Foo>();
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            
            var fooBefore = provider.GetRequiredService<IFoo>();
            via.To(x => x.Name).Redirect("Diverted");
            var fooAfter = provider.GetRequiredService<IFoo>();
            
            fooBefore.Name.ShouldBe("Diverted");
            fooAfter.Name.ShouldBe("Diverted");
            
            _diverter.ResetAll();
            
            fooBefore.Name.ShouldBe("original");
            fooAfter.Name.ShouldBe("original");
        }
        
        [Fact]
        public void ShouldNotProxyNullDependencies()
        {
            _services.AddSingleton<IFoo>(_ => null!);
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetService<IFoo>();
            
            foo.ShouldBeNull();
        }
        
        [Fact]
        public void ContainerShouldDisposeNonDisposableDecoratedInstances()
        {
            using var disposeContext = new DisposeContext();
            _services.AddSingleton(disposeContext);

            for (var i = 0; i < 2; i++)
            {
                _services.AddTransient<ITestNonDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
                _services.AddTransient<ITestNonDisposable, TestDisposable>();
                _services.AddScoped<ITestNonDisposable, TestDisposable>();
                _services.AddScoped<ITestNonDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
            }
            
            var diverter = new Diverter().Register<ITestNonDisposable>();
            _services.Divert(diverter);
            
            var provider = _services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.GetServices<ITestNonDisposable>();
                disposeContext.DisposeCount.ShouldBe(0);
            }
            
            disposeContext.DisposeCount.ShouldBe(8);
        }
        
        [Fact]
        public void ContainerShouldNotDisposeDisposableInstances()
        {
            using var disposeContext = new DisposeContext();
            _services.AddSingleton(disposeContext);

            for (var i = 0; i < 2; i++)
            {
                _services.AddTransient<ITestDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
                _services.AddTransient<ITestDisposable, TestDisposable>();
                _services.AddScoped<ITestDisposable, TestDisposable>();
                _services.AddScoped<ITestDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
            }
            
            var diverter = new Diverter().Register<ITestDisposable>();
            _services.Divert(diverter);

            var disposeCalls = diverter
                .Via<ITestDisposable>()
                .To(x => x.Dispose())
                .Redirect(() => { })
                .Record();
            
            var provider = _services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.GetServices<ITestDisposable>();
                disposeContext.DisposeCount.ShouldBe(0);
            }
            
            disposeContext.DisposeCount.ShouldBe(0);
            disposeCalls.Count.ShouldBe(8);
        }
        
        [Fact]
        public async Task ContainerShouldNotDisposeAsyncDisposableInstances()
        {
            using var disposeContext = new DisposeContext();
            _services.AddSingleton(disposeContext);

            for (var i = 0; i < 2; i++)
            {
                _services.AddTransient<ITestAsyncDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
                _services.AddTransient<ITestAsyncDisposable, TestDisposable>();
                _services.AddScoped<ITestAsyncDisposable, TestDisposable>();
                _services.AddScoped<ITestAsyncDisposable>(provider => new TestDisposable(provider.GetRequiredService<DisposeContext>()));
            }
            
            var diverter = new Diverter().Register<ITestAsyncDisposable>();
            _services.Divert(diverter);

            var disposeCalls = diverter
                .Via<ITestAsyncDisposable>()
                .To(x => x.DisposeAsync())
                .Redirect(() => new ValueTask())
                .Record();
            
            var provider = _services.BuildServiceProvider();
            await using (var scope = provider.CreateAsyncScope())
            {
                scope.ServiceProvider.GetServices<ITestAsyncDisposable>();
                disposeContext.DisposeCount.ShouldBe(0);
            }
            
            disposeContext.DisposeCount.ShouldBe(0);
            disposeCalls.Count.ShouldBe(8);
        }

        private interface ITestNonDisposable
        {
        }
        
        private interface ITestDisposable : IDisposable
        {
        }
        
        private interface ITestAsyncDisposable : IAsyncDisposable
        {
        }

        private class TestDisposable : ITestNonDisposable, ITestDisposable, ITestAsyncDisposable
        {
            private readonly DisposeContext _disposeContext;
            private bool _isDisposed;


            public TestDisposable(DisposeContext disposeContext)
            {
                _disposeContext = disposeContext;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    throw new Exception("Already disposed");
                }

                _isDisposed = true;
                _disposeContext.AddDispose();
            }

            public ValueTask DisposeAsync()
            {
                Dispose();
                
                return new ValueTask();
            }
        }

        private class DisposeContext : IDisposable
        {
            private static readonly object LockObject = new();

            public DisposeContext()
            {
                Monitor.Enter(LockObject);
            }
            
            public int DisposeCount { get; private set; }

            public void AddDispose()
            {
                DisposeCount++;
            }
            
            public void Dispose()
            {
                Monitor.Exit(LockObject);
            }
        }
    }
}