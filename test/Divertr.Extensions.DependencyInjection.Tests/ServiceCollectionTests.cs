using System.Collections.Generic;
using Divertr.Extensions.DependencyInjection;
using Divertr.Extensions.DependencyInjection.Tests.Model;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests.Extensions
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly DiverterSet _diverters = new();
        
        [Fact]
        public void ShouldInjectDiverterSet()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Divert<IFoo>(_diverters);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverters.Get<IFoo>().Redirect(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void ShouldInjectDiverter()
        {
            var diverter = _diverters.Get<IFoo>();
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
            
            _diverters.ResetAll();
            
            foo.Message.ShouldBe("Original");
            foo2.Message.ShouldBe("Original");
        }
        
        [Fact]
        public void GivenOpenGenericWithClosedDivertShouldRedirect()
        {
            _services.AddTransient(typeof(IList<>), typeof(List<>));
            _services.AddTransient<IList<string>, List<string>>();
            _services.DivertRange<IList<string>, IList<string>>(_diverters);

            var provider = _services.BuildServiceProvider();
            var list = provider.GetRequiredService<IList<string>>();
            
            var mock = new Mock<IList<string>>();
            mock.Setup(x => x.Count).Returns(10);
            _diverters.Get<IList<string>>().Redirect(mock.Object);
            
            list.Count.ShouldBe(10);
            _diverters.ResetAll();
            list.Count.ShouldBe(0);
        }
    }
}