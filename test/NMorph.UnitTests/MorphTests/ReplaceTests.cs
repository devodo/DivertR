using Moq;
using Shouldly;
using Xunit;

namespace NMorph.UnitTests.MorphTests
{
    public class ReplaceTests
    {
        private readonly Divertr _divertr = new Divertr();
        
        [Fact]
        public void GivenMorphProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var proxy = _divertr.Proxy<ITestSubject>(original);
            
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
            var subject = _divertr.Proxy<ITestSubject>(original);
            _divertr.Redirect<ITestSubject>(x => x.SendTo(new SubstituteTest(" world", x.CallContext)));

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
            var subject = _divertr.Proxy<ITestSubject>(original);
            
            var mock = new Mock<ITestSubject>();
            mock
                .Setup(x => x.Message)
                .Returns($"{_divertr.CallContext<ITestSubject>().Previous.Message} world");

            _divertr.Redirect<ITestSubject>().SendTo(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMockedDivert2_ShouldRedirect()
        {
            // ARRANGE
            var original = new TestA("hello");
            var subject = _divertr.Proxy<ITestSubject>(original);
            
            var mock = new Mock<ITestSubject>();
            _divertr.Redirect<ITestSubject>(x =>
            {
                mock
                    .Setup(x => x.Message)
                    .Returns($"{_divertr.CallContext<ITestSubject>().Previous.Message} world");
            }).SendTo(mock.Object);

            // ACT
            var message = subject.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void GivenMorphProxy_ShouldReplace()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _divertr.Proxy<ITestSubject>(original);
            
            var mock = new Mock<ITestSubject>();
            mock.Setup(x => x.Message).Returns("Hello");
            //mock.DefaultValueProvider

            // ACT
            _divertr
                .Redirect<ITestSubject>(d =>
                {
                    d.When(x => x.Message).Return("wow");
                    d.When(x => x.Message).SendTo(new SubstituteTest(" again", d.CallContext));
                    d.SendTo(new SubstituteTest(" again", d.CallContext));
                });

            // ASSERT
            subject.Message.ShouldBe(original.Message + " again");
        }
        
        [Fact]
        public void GivenMultipleReplacements_ShouldChain()
        {
            // ARRANGE
            var subject = _divertr.Proxy<ITestSubject>(new TestA("hello world"));
            
            // ACT
            _divertr.Redirect<ITestSubject>(diversion =>
            {
                diversion.SendTo(new SubstituteTest(" me", diversion.CallContext));
                diversion.SendTo(new SubstituteTest(" me", diversion.CallContext));
                diversion.SendTo(new SubstituteTest(" again", diversion.CallContext));
            });
                

            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenReplacements_ShouldOnlyReplaceAfterReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _divertr.Proxy<ITestSubject>(original);
            
            // ACT
            _divertr.Redirect<ITestSubject>(diversion =>
            {
                diversion.SendTo(new SubstituteTest(" me", diversion.CallContext));
                diversion.Reset();
                diversion.SendTo(new SubstituteTest(" again", diversion.CallContext));
            });

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenReplacements_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _divertr.Proxy<ITestSubject>(original);
            
            // ACT
            _divertr.Redirect<ITestSubject>(builder =>
            {
                builder
                    .SendTo(new SubstituteTest(" me", builder.CallContext))
                    .SendTo(new SubstituteTest(" again", builder.CallContext))
                    .Reset();
            });
                
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
