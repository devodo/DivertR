using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class RedirectTests
    {
        private readonly Diverter _diverter = new Diverter();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var proxy = _diverter.Of<ITestSubject>().Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenDivertedProxy_ShouldRedirect()
        {
            // ARRANGE
            var original = new TestA("hello");
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(original);
            diverter.AddRedirect(new SubstituteTest(" world", diverter.CallContext));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMockedDivert_ShouldRedirect()
        {
            // ARRANGE
            var original = new TestA("hello");
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(original);
            
            var mock = new Mock<ITestSubject>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{diverter.CallContext.Original.Message} world");

            diverter.Redirect(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        /*
        [Fact]
        public void GivenProxy_ShouldRedirect()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _diverter.For<ITestSubject>().Proxy(original);
            
            var mock = new Mock<ITestSubject>();
            mock.Setup(x => x.Message).Returns("Hello");
            //mock.DefaultValueProvider

            // ACT
            _diverter
                .For<ITestSubject>(d =>
                {
                    d.When(x => x.Message).Return("wow");
                    d.When(x => x.Message).SendTo(new SubstituteTest(" again", d.CallContext));
                    d.Redirect(new SubstituteTest(" again", d.CallContext));
                });

            // ASSERT
            subject.Message.ShouldBe(original.Message + " again");
        }
        */
        
        [Fact]
        public void MultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(new TestA("hello world"));

            // ACT
            diverter
                .AddRedirect(new SubstituteTest(" me", diverter.CallContext))
                .AddRedirect(new SubstituteTest(" me", diverter.CallContext))
                .AddRedirect(new SubstituteTest(" again", diverter.CallContext));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(original);

            // ACT
            diverter.AddRedirect(new SubstituteTest(" me", diverter.CallContext));
            diverter.Reset();
            diverter.AddRedirect(new SubstituteTest(" again", diverter.CallContext));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(original);

            // ACT
            diverter.AddRedirect(new SubstituteTest(" me", diverter.CallContext));
            diverter.AddRedirect(new SubstituteTest(" again", diverter.CallContext));
            diverter.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var diverter = _diverter.Of<ITestSubject>();
            var subject = diverter.Proxy(original);

            // ACT
            diverter.AddRedirect(new SubstituteTest(" me", diverter.CallContext));
            diverter.AddRedirect(new SubstituteTest(" again", diverter.CallContext));
            _diverter.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
