using Divertr.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests.Extensions
{
    public class ServiceCollectionTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly Diverter _diverter = new Diverter();
        
        public ServiceCollectionTests()
        {
            
        }
        
        [Fact]
        public void CanDecorate()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.Decorate<IFoo>(_diverter.Of<IFoo>().Proxy);
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverter.Of<IFoo>().Redirect(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void CanIntercept()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _diverter.Intercept(_services);
            
            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverter.Of<IFoo>().Redirect(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
        
        [Fact]
        public void CanDivert()
        {
            _services.AddSingleton<IFoo>(new Foo {Message = "Original"});
            _services.DiverterDecorate(_diverter);

            var provider = _services.BuildServiceProvider();
            var foo = provider.GetRequiredService<IFoo>();
            
            var mock = new Mock<IFoo>();
            mock.Setup(x => x.Message).Returns("Diverted");
            _diverter.Of<IFoo>().Redirect(mock.Object);
            
            foo.Message.ShouldBe("Diverted");
        }
    }
}