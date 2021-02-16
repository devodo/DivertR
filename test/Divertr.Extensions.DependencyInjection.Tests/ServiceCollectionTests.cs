﻿using System;
using System.Collections.Generic;
using System.Linq;
using Divertr.Extensions.DependencyInjection.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.Extensions.DependencyInjection.Tests
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly DiverterCollection _diverters = new();
        private List<Type> _typesRegistered;
        
        [Fact]
        public void ShouldInjectDiverterSet()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Divert<IFoo>(_diverters);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverters.Of<IFoo>().SendTo(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldInjectDiverter()
        {
            var diverter = _diverters.Of<IFoo>();
            _services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
            _services.Divert(diverter);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            diverter.SendTo(mock.Object);
            
            var foo2 = provider.GetRequiredService<IFoo>();
            
            foo.Message.ShouldBe("Diverted");
            foo2.Message.ShouldBe("Diverted");
            
            _diverters.ResetAll();
            
            foo.Message.ShouldBe("Original");
            foo2.Message.ShouldBe("Original");
        }
        
        [Fact]
        public void GivenOpenGenericWithClosedRegistrationShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.Divert<IList<string>>(_diverters);

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverters.Of<IList<string>>().SendTo(mock.Object);
            
            list.Count.ShouldBe(10);
            _diverters.ResetAll();
            list.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenOpenGenericShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.Divert(_diverters, builder =>
            {
                builder.Include<IList<string>>();
                builder.WithDiversionsRegisteredHandler(TypesRegisteredHandler);
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverters.Of<IList<string>>().SendTo(mock.Object);
            
            _typesRegistered.ShouldBe(new[] { typeof(IList<string>)});
            list.Count.ShouldBe(10);
            _diverters.ResetAll();
            list.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenOpenAndClosedGenericRegistrationsShouldRegisterSingleDiversion()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.Divert(_diverters, builder =>
            {
                builder.Include<IList<string>>();
                builder.WithDiversionsRegisteredHandler(TypesRegisteredHandler);
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverters.Of<IList<string>>().SendTo(mock.Object);
            
            list.Count.ShouldBe(10);
            _diverters.ResetAll();
            list.Count.ShouldBe(0);
        }

        private void TypesRegisteredHandler(object sender, IEnumerable<Type> typesRegistered)
        {
            _typesRegistered = typesRegistered.ToList();
        }
    }
}