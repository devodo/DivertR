using Moq;
using Shouldly;
using Xunit;

namespace NMorph.UnitTests.MorphTests
{
    public class ReplaceTests
    {
        private readonly Morph _morph = new Morph();
        
        [Fact]
        public void GivenMorphProxy_ShouldDefaultToOrigin()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var proxy = _morph.Create<ITestSubject>(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }

        [Fact]
        public void GivenMorphProxy_ShouldReplace()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _morph.Create<ITestSubject>(original);
            
            var mock = new Mock<ITestSubject>();
            mock.Setup(x => x.Message).Returns("Hello");
            //mock.DefaultValueProvider

            // ACT
            _morph
                .Intercept<ITestSubject>(out var callContext)
                .When(x => x.Message)
                .Return("wow")
                .When(x => x.Message)
                .Retarget(new SubstituteTest(" again", callContext))
                .Retarget(new SubstituteTest(" again", callContext));

            _morph
                .Intercept<ITestSubject>()
                .When(x => x.Message)
                .Retarget(new SubstituteTest(" again", callContext));

            // ASSERT
            subject.Message.ShouldBe(original.Message + " again");
        }
        
        [Fact]
        public void GivenMultipleReplacements_ShouldChain()
        {
            // ARRANGE
            var subject = _morph.Create<ITestSubject>(new TestA("hello world"));
            
            // ACT
            _morph
                .Intercept<ITestSubject>(out var callContext)
                .Retarget(new SubstituteTest(" me", callContext))
                .Retarget(new SubstituteTest(" me", callContext))
                .Retarget(new SubstituteTest(" again", callContext));

            // ASSERT
            subject.Message.ShouldBe("hello world me me again");
        }
        
        [Fact]
        public void GivenResetBetweenReplacements_ShouldOnlyReplaceAfterReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _morph.Create<ITestSubject>(original);
            
            // ACT
            _morph
                .Intercept<ITestSubject>(out var callContext)
                .Retarget(new SubstituteTest(" me", callContext))
                .Reset()
                .Retarget(new SubstituteTest(" again", callContext));

            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenReplacements_WhenReset_ShouldReset()
        {
            // ARRANGE
            var original = new TestA("hello world");
            var subject = _morph.Create<ITestSubject>(original);
            
            // ACT
            _morph
                .Intercept<ITestSubject>(out var callContext)
                .Retarget(new SubstituteTest(" me", callContext))
                .Retarget(new SubstituteTest(" again", callContext))
                .Reset();
            
            // ASSERT
            subject.Message.ShouldBe(original.Message);
        }
    }
}
