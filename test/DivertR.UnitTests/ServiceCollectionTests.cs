using System;
using System.Collections.Generic;
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

        [Fact]
        public void ShouldReplaceTypeRegistration()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            diverter.Redirect<IFoo>().To(x => x.Name).Via("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceInstanceRegistration()
        {
            _services.AddSingleton<IFoo>(new Foo());
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            diverter.Redirect<IFoo>().To(x => x.Name).Via("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceFactoryRegistration()
        {
            _services.AddSingleton<IFoo>(_ => new Foo());
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            diverter.Redirect<IFoo>().To(x => x.Name).Via("Diverted");
            
            foo.Name.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceTypeDecorator()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder().Decorate<IFoo>(foo => new Foo(foo.Name + " decorated")).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original decorated");
        }
        
        [Fact]
        public void ShouldReplaceInstanceDecorator()
        {
            _services.AddSingleton<IFoo>(new Foo());
            var diverter = new DiverterBuilder().Decorate<IFoo>(foo => new Foo(foo.Name + " decorated")).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original decorated");
        }
        
        [Fact]
        public void ShouldReplaceFactoryDecorator()
        {
            _services.AddSingleton<IFoo>(_ => new Foo());
            var diverter = new DiverterBuilder().Decorate<IFoo>(foo => new Foo(foo.Name + " decorated")).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original decorated");
        }
        
        [Fact]
        public void ShouldDecorateWithDiverterParam()
        {
            _services.AddSingleton<IFoo, Foo>();

            var diverter = new DiverterBuilder()
                .IncludeRedirect<IBar>()
                .Decorate<IFoo>((foo, diverter) =>
                {
                    var bar = diverter.Redirect<IBar>().Proxy(new Bar("test"));
                    return new Foo($"{foo.Name} {bar.Name} decorated");
                })
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original test decorated");
        }
        
        [Fact]
        public void ShouldDecorateWithProviderParam()
        {
            _services.AddSingleton<IFoo, Foo>();
            _services.AddSingleton<IBar>(_ => new Bar("test"));

            var diverter = new DiverterBuilder()
                .IncludeRedirect<IBar>()
                .Decorate<IFoo>((foo, diverter, provider) =>
                {
                    var bar = diverter.Redirect<IBar>().Proxy(provider.GetRequiredService<IBar>());
                    return new Foo($"{foo.Name} {bar.Name} decorated");
                })
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original test decorated");
        }
        
        [Fact]
        public void ShouldDecorateWithObjectDelegate()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder().Decorate(typeof(IFoo), foo => new Foo(((IFoo) foo).Name + " decorated")).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original decorated");
        }
        
        [Fact]
        public void ShouldDecorateWithObjectDelegateAndDiverterParam()
        {
            _services.AddSingleton<IFoo, Foo>();

            var diverter = new DiverterBuilder()
                .IncludeRedirect<IBar>()
                .Decorate(typeof(IFoo), (foo, diverter) =>
                {
                    var bar = diverter.Redirect<IBar>().Proxy(new Bar("test"));
                    return new Foo($"{((IFoo) foo).Name} {bar.Name} decorated");
                })
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original test decorated");
        }
        
        [Fact]
        public void ShouldDecorateWithObjectDelegateAndProviderParam()
        {
            _services.AddSingleton<IFoo, Foo>();
            _services.AddSingleton<IBar>(_ => new Bar("test"));

            var diverter = new DiverterBuilder()
                .IncludeRedirect<IBar>()
                .Decorate(typeof(IFoo), (foo, diverter, provider) =>
                {
                    var bar = diverter.Redirect<IBar>().Proxy(provider.GetRequiredService<IBar>());
                    return new Foo($"{((IFoo) foo).Name} {bar.Name} decorated");
                })
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original test decorated");
        }
        
        [Fact]
        public void ShouldReplaceMultipleDecorators()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder()
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 1"))
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 2"))
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 3"))
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original 1 2 3");
        }
        
        [Fact]
        public void ShouldReplaceMultipleDecoratorsIncludingRedirect()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder()
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 1"))
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 2"))
                .Register<IFoo>()
                .Decorate<IFoo>(Spy.On)
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();

            Spy.Of(foo).To(x => x.Name).Via(call => call.CallNext() + " 3");

            diverter
                .Redirect<IFoo>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            foo.Name.ShouldBe("original 1 2 redirected 3");
        }
        
        [Fact]
        public void ShouldReplaceNamedDecorators()
        {
            _services.AddSingleton<IFoo, Foo>();
            var diverter = new DiverterBuilder()
                .Decorate<IFoo>("test", foo => new Foo(foo.Name + " 1"))
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 2"))
                .Decorate<IFoo>("test", foo => new Foo(foo.Name + " 3"))
                .Create();
            
            _services.Divert(diverter, "test");
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            foo.Name.ShouldBe("original 1 3");
        }
        
        [Fact]
        public void ShouldCacheDecoratedInstancesWithSameRoot()
        {
            var fooRoot = new Foo();
            _services.AddTransient<IFoo>(_ => fooRoot);
            var diverter = new DiverterBuilder().Decorate<IFoo>(foo => new Foo(foo.Name + " 1")).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo1 = provider.GetRequiredService<IFoo>();
            var foo2 = provider.GetRequiredService<IFoo>();
            
            foo1.Name.ShouldBe("original 1");
            foo2.ShouldBeSameAs(foo1);
        }
        
        [Fact]
        public void ShouldCacheMultiDecoratedInstancesWithSameRoot()
        {
            var fooRoot = new Foo();
            _services.AddTransient<IFoo>(_ => fooRoot);
            var diverter = new DiverterBuilder()
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 1"))
                .Decorate<IFoo>(foo => new Foo(foo.Name + " 2"))
                .Register<IFoo>()
                .Decorate<IFoo>(Spy.On)
                .Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            var foo2 = provider.GetRequiredService<IFoo>();

            Spy.Of(foo).To(x => x.Name).Via(call => call.CallNext() + " 3");

            diverter
                .Redirect<IFoo>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            foo.Name.ShouldBe("original 1 2 redirected 3");
            foo2.ShouldBeSameAs(foo);
        }
        
        [Fact]
        public void GivenTypedDecoratorShouldDecorateStructTypes()
        {
            _services.AddTransient(typeof(int), _ => 10);
            var diverter = new DiverterBuilder().Decorate<int>(x => x + 1).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var i = provider.GetRequiredService<int>();
            
            i.ShouldBe(11);
        }
        
        [Fact]
        public void GivenGenericDecoratorShouldDecorateStructTypes()
        {
            _services.AddTransient(typeof(int), _ => 10);
            var diverter = new DiverterBuilder().Decorate(typeof(int), x => (int) x + 1).Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var i = provider.GetRequiredService<int>();
            
            i.ShouldBe(11);
        }
        
        [Fact]
        public void GivenServiceTypeMissingShouldThrowException()
        {
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            Action test = () => _services.Divert(diverter);
            test.ShouldThrow<DiverterException>().Message.ShouldContain($"{typeof(IFoo).FullName}");
        }
        
        [Fact]
        public void GivenListDecoratorCanRedirectProxyItems()
        {
            _services.AddTransient<List<IFoo>>(_ => new List<IFoo> { new Foo("1"), new Foo("2") });
            var diverterBuilder = new DiverterBuilder();
            var diverter = diverterBuilder
                .IncludeRedirect<IFoo>()
                .Decorate<List<IFoo>>((foos, diverter) => foos
                    .Select(x => diverter.Redirect<IFoo>().Proxy(x))
                    .ToList()).Create();
            
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            var foos = provider.GetRequiredService<List<IFoo>>();

            diverter.Redirect<IFoo>().To(x => x.Name).Via(call => call.CallNext() + " diverted");
            
            foos[0].Name.ShouldBe("1 diverted");
            foos[1].Name.ShouldBe("2 diverted");
        }
        
        [Fact]
        public void ShouldReplaceMultipleRegistrations()
        {
            var fooRegistrations = Enumerable.Range(0, 10)
                .Select((_, i) => new Foo($"Foo{i}")).ToList();
            fooRegistrations.ForEach(foo => _services.AddSingleton<IFoo>(foo));
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();

            diverter.Redirect<IFoo>()
                .To(x => x.Name)
                .Via(() => "Diverted: " + diverter.Redirect<IFoo>().Relay.Next.Name);
            
            var fooInstances = provider.GetServices<IFoo>().ToList();
            
            fooInstances.Select(x => x.Name).ShouldBe(fooRegistrations.Select(foo => "Diverted: " + foo.Name));
        }
        
        [Fact]
        public void GivenResolvedInstancesBeforeAndAfterRegisteringVia_ShouldRedirect()
        {
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            
            _services.AddTransient<IFoo, Foo>();
            _services.Divert(diverter);
            var provider = _services.BuildServiceProvider();
            
            var fooBefore = provider.GetRequiredService<IFoo>();
            redirect.To(x => x.Name).Via("Diverted");
            var fooAfter = provider.GetRequiredService<IFoo>();
            
            fooBefore.Name.ShouldBe("Diverted");
            fooAfter.Name.ShouldBe("Diverted");
            
            diverter.ResetAll();
            
            fooBefore.Name.ShouldBe("original");
            fooAfter.Name.ShouldBe("original");
        }
        
        [Fact]
        public void ShouldNotProxyNullDependencies()
        {
            _services.AddSingleton<IFoo>(_ => null!);
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            _services.Divert(diverter);
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
            
            var diverter = new DiverterBuilder().Register<ITestNonDisposable>().Create();
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
            
            var diverter = new DiverterBuilder().Register<ITestDisposable>().Create();
            _services.Divert(diverter);

            var disposeCalls = diverter
                .Redirect<ITestDisposable>()
                .To(x => x.Dispose())
                .Via(() => { })
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
            
            var diverter = new DiverterBuilder().Register<ITestAsyncDisposable>().Create();
            _services.Divert(diverter);

            var disposeCalls = diverter
                .Redirect<ITestAsyncDisposable>()
                .To(x => x.DisposeAsync())
                .Via(() => new ValueTask())
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