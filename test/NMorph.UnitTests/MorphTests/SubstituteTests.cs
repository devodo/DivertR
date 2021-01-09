using Shouldly;
using Xunit;

namespace NMorph.UnitTests.MorphTests
{
    public class SubstituteTests
    {
        private readonly Morph _morph = new Morph();

        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration()
        {
            // ARRANGE
            var subject = _morph.Create<ITestSubject>(new TestA("hello world"));
            
            // ACT
            _morph
                .Alter<ITestSubject>()
                .Replace(src => new SubstituteTest(" again", src));
            
            // ASSERT
            subject.Message.ShouldBe("hello world again");
        }
        
        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration2()
        {
            // ARRANGE
            var subject = _morph.Create<ITestSubject>(new TestA("hello world"));
            
            // ACT
            _morph
                .Alter<ITestSubject>()
                .Replace(src => new SubstituteTest(" me", src))
                .Replace(src => new SubstituteTest(" again", src));
            
            // ASSERT
            subject.Message.ShouldBe("hello world me again");
        }
    }

    
}
