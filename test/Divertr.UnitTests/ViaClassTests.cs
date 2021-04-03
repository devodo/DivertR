using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaClassTests
    {
        [Fact(Skip = "Class support removed")]
        public void GivenClassProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = new Via<Foo>();
            var proxy = via.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact(Skip = "Class support removed")]
        public void GivenClassProxy_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<Foo>();
            var proxy = via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            via.RedirectTo(foo);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(foo.Message);
        }
    }
}