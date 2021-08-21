using DivertR.DynamicProxy;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaClassTests
    {
        private static readonly DiverterSettings DiverterSettings = new()
        {
            ProxyFactory = new DynamicProxyFactory()
        };

        private readonly Via<Foo> _via;

        public ViaClassTests()
        {
            _via = new Via<Foo>(DiverterSettings);
        }
        
        [Fact]
        public void GivenClassProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = proxy.NameVirtual;
            
            // ASSERT
            message.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenTargetRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");

            // ACT
            _via.Retarget(foo);

            // ASSERT
            proxy.NameVirtual.ShouldBe(foo.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenDelegateRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            _via.To(x => x.NameVirtual).Redirect(() => _via.Relay.Next.NameVirtual + " bar");

            // ASSERT
            proxy.NameVirtual.ShouldBe("hello foo bar");
        }
    }
}