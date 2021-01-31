using System;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class DiverterTests
    {
        private readonly Diverter<ITestSubject> _diverter = new Diverter<ITestSubject>();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var proxy = _diverter.Proxy(original);
            
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
            var subject = _diverter.Proxy(original);
            _diverter.Redirect(new SubstituteTest(" world", _diverter.CallCtx));

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }
        
        [Fact]
        public void Docs()
        {
            // ARRANGE
            var diverter = new Diverter<IFoo>();
            
            var fooA = diverter.Proxy(new Foo {Message = "foo A"});
            var fooB = diverter.Proxy(new Foo { Message = "foo B" });

            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"Hello {diverter.CallCtx.Replaced.Message}");

            diverter.Redirect(mock.Object);

            Console.WriteLine(fooA.Message); // "Hello foo A"
            Console.WriteLine(fooB.Message); // "Hello foo B"

            // ACT
            var message = fooB.Message;

            // ASSERT
            fooA.Message.ShouldBe("Hello foo A");
            message.ShouldBe("Hello foo B");
        }
        
        [Fact]
        public void GivenMockedDivert_ShouldRedirect()
        {
            // ARRANGE
            var original = new TestA("hello");
            var subject = _diverter.Proxy(original);
            
            var mock = new Mock<ITestSubject>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_diverter.CallCtx.Original.Message} world");

            _diverter.Redirect(mock.Object);

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
            var subject = _diverter.Proxy(new TestA("hello world"));

            // ACT
            _diverter
                .AddRedirect(new SubstituteTest(" me", _diverter.CallCtx))
                .AddRedirect(new SubstituteTest(" me", _diverter.CallCtx))
                .AddRedirect(new SubstituteTest(" again", _diverter.CallCtx));


            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _diverter.Proxy(original);

            // ACT
            _diverter.AddRedirect(new SubstituteTest(" me", _diverter.CallCtx));
            _diverter.Reset();
            _diverter.AddRedirect(new SubstituteTest(" again", _diverter.CallCtx));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenRedirects_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _diverter.Proxy(original);

            // ACT
            _diverter.AddRedirect(new SubstituteTest(" me", _diverter.CallCtx));
            _diverter.AddRedirect(new SubstituteTest(" again", _diverter.CallCtx));
            _diverter.Reset();


            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
