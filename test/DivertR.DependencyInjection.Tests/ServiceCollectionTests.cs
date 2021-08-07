using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.DependencyInjection.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DivertR.DependencyInjection.Tests
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly Diverter _diverter = new();

        [Fact]
        public void ShouldReplaceRegistration()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Divert<IFoo>(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            _diverter.Via<IFoo>().When(x => x.Message).Redirect("Diverted");
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceMultipleRegistrations()
        {
            var fooRegistrations = Enumerable.Range(0, 10)
                .Select((_, i) => new Foo {Message = $"Foo{i}"}).ToList();
            fooRegistrations.ForEach(foo => _services.AddSingleton<IFoo>(foo));
            _services.Divert<IFoo>(_diverter);
            var provider = _services.BuildServiceProvider();

            _diverter.Via<IFoo>()
                .When(x => x.Message)
                .Redirect(() => "Diverted: " + _diverter.Via<IFoo>().Next.Message);
            
            var fooInstances = provider.GetServices<IFoo>().ToList();
            
            fooInstances.Select(x => x.Message).ShouldBe(fooRegistrations.Select(foo => "Diverted: " + foo.Message));
        }
        
        [Fact]
        public void GivenResolvedInstancesBeforeAndAfterRegisteringRedirect_ShouldRedirect()
        {
            var via = _diverter.Via<IFoo>();
            
            _services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
            _services.Divert(via);
            var provider = _services.BuildServiceProvider();
            
            var fooBefore = provider.GetRequiredService<IFoo>();
            via.When(x => x.Message).Redirect("Diverted");
            var fooAfter = provider.GetRequiredService<IFoo>();
            
            fooBefore.Message.ShouldBe("Diverted");
            fooAfter.Message.ShouldBe("Diverted");
            
            _diverter.ResetAll();
            
            fooBefore.Message.ShouldBe("Original");
            fooAfter.Message.ShouldBe("Original");
        }
        
        [Fact]
        public void GivenOpenGenericWithClosedRegistrationShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.Divert<IList<string>>(_diverter);

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            _diverter.Via<IList<string>>().When(x => x.Count).Redirect(10);
            
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }

        [Fact]
        public void GivenOpenGenericShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            List<Type> typesDiverted = null;
            _services.Divert(_diverter, builder =>
            {
                builder.Include<IList<string>>();
                builder.WithOnCompleteCallback(types => { typesDiverted = types; });
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            _diverter.Via<IList<string>>().When(x => x.Count).Redirect(10);

            typesDiverted.ShouldBe(new[] {typeof(IList<string>)});
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }

        [Fact]
        public void GivenOpenGeneric_ShouldRedirect()
        {
            var via = _diverter.Via<IList<string>>();
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.Divert(via);

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            via.When(x => x.Count).Redirect(10);
            
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenOpenAndClosedGenericRegistrationsShouldRegisterSingleDiverter()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.Divert(_diverter, builder =>
            {
                builder.Include<IList<string>>();
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            _diverter.Via<IList<string>>().When(x => x.Count).Redirect(10);
            
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }
    }
}