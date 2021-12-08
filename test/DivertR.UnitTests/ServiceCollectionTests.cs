using System;
using System.Linq;
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
    }
}