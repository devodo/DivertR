using DivertR.DynamicProxy;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaClassTests
    {
        private static readonly DiverterSettings DiverterSettings = new DiverterSettings
        {
            ProxyFactory = new DynamicProxyFactory()
        };

        private Via<Foo> _via;

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
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenClassProxy_WhenTargetRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");

            // ACT
            _via.RedirectTo(foo);

            // ASSERT
            proxy.Message.ShouldBe(foo.Message);
        }
        
        [Fact]
        public void GivenClassProxy_WhenDelegateRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            _via.Redirect(x => x.Message).To(() => _via.Next.Message + " bar");

            // ASSERT
            proxy.Message.ShouldBe("hello foo bar");
        }
    }
}