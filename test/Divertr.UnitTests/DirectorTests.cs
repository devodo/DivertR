using Divertr.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DirectorTests
    {
        private readonly Director<IFoo> _director = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var proxy = _director.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenDivertedProxy_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var subject = _director.Proxy(original);
            _director.Redirect(new FooSubstitute(" world", _director.CallCtx.Original));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMockedDivert_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("hello");
            var subject = _director.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_director.CallCtx.Original.Message} world");

            _director.Redirect(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void MultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var subject = _director.Proxy(new Foo("hello world"));

            // ACT
            _director
                .AddRedirect(new FooSubstitute(" me", _director.CallCtx.Replaced))
                .AddRedirect(new FooSubstitute(" me", _director.CallCtx.Replaced))
                .AddRedirect(new FooSubstitute(" again", _director.CallCtx.Replaced));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _director.Proxy(original);

            // ACT
            _director.AddRedirect(new FooSubstitute(" me", _director.CallCtx.Replaced));
            _director.Reset();
            _director.AddRedirect(new FooSubstitute(" again", _director.CallCtx.Replaced));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello world");
            var subject = _director.Proxy(original);

            // ACT
            _director.AddRedirect(new FooSubstitute(" me", _director.CallCtx.Replaced));
            _director.AddRedirect(new FooSubstitute(" again", _director.CallCtx.Replaced));
            _director.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
