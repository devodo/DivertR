using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.Core;
using DivertR.DependencyInjection.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DivertR.DependencyInjection.Tests
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IDiverter _diverter = new Diverter().Register<IFoo>();

        [Fact]
        public void ShouldReplaceRegistration()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            _diverter.Via<IFoo>().To(x => x.Message).Redirect("Diverted");
            
            foo.Message.ShouldBe("Diverted");
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
                .Select((_, i) => new Foo {Message = $"Foo{i}"}).ToList();
            fooRegistrations.ForEach(foo => _services.AddSingleton<IFoo>(foo));
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();

            _diverter.Via<IFoo>()
                .To(x => x.Message)
                .Redirect(() => "Diverted: " + _diverter.Via<IFoo>().Relay.Next.Message);
            
            var fooInstances = provider.GetServices<IFoo>().ToList();
            
            fooInstances.Select(x => x.Message).ShouldBe(fooRegistrations.Select(foo => "Diverted: " + foo.Message));
        }
        
        [Fact]
        public void GivenResolvedInstancesBeforeAndAfterRegisteringRedirect_ShouldRedirect()
        {
            var via = _diverter.Via<IFoo>();
            
            _services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
            _services.Divert(_diverter);
            var provider = _services.BuildServiceProvider();
            
            var fooBefore = provider.GetRequiredService<IFoo>();
            via.To(x => x.Message).Redirect("Diverted");
            var fooAfter = provider.GetRequiredService<IFoo>();
            
            fooBefore.Message.ShouldBe("Diverted");
            fooAfter.Message.ShouldBe("Diverted");
            
            _diverter.ResetAll();
            
            fooBefore.Message.ShouldBe("Original");
            fooAfter.Message.ShouldBe("Original");
        }
    }
}