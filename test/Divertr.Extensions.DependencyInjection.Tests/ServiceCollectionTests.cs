using System;
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
        private readonly Diverter _diverter = new();
        private List<Type> _typesDiverted;
        
        [Fact]
        public void ShouldReplaceRegistration()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Divert<IFoo>(_diverter);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverter.Router<IFoo>().Redirect(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldReplaceMultipleRegistrations()
        {
            var diverter = _diverter.Router<IFoo>();
            _services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
            _services.Divert(diverter);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            diverter.Redirect(mock.Object);
            
            var foo2 = provider.GetRequiredService<IFoo>();
            
            foo.Message.ShouldBe("Diverted");
            foo2.Message.ShouldBe("Diverted");
            
            _diverter.ResetAll();
            
            foo.Message.ShouldBe("Original");
            foo2.Message.ShouldBe("Original");
        }
        
        [Fact]
        public void GivenOpenGenericWithClosedRegistrationShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.Divert<IList<string>>(_diverter);

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverter.Router<IList<string>>().Redirect(mock.Object);
            
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenOpenGenericShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.Divert(_diverter, builder =>
            {
                builder.Include<IList<string>>();
                builder.WithTypesDivertedHandler(TypesDivertedHandler);
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverter.Router<IList<string>>().Redirect(mock.Object);
            
            _typesDiverted.ShouldBe(new[] { typeof(IList<string>)});
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
                builder.WithTypesDivertedHandler(TypesDivertedHandler);
            });

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverter.Router<IList<string>>().Redirect(mock.Object);
            
            list.Count.ShouldBe(10);
            _diverter.ResetAll();
            list.Count.ShouldBe(0);
        }

        private void TypesDivertedHandler(object sender, IEnumerable<Type> typesDiverted)
        {
            _typesDiverted = typesDiverted.ToList();
        }
    }
}