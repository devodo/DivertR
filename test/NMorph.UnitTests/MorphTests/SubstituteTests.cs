using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests.MorphTests
{
    public class SubstituteTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Morph<ITestSubject> _morph = new Morph<ITestSubject>();

        public SubstituteTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration()
        {
            var subject = _morph.CreateSubject(new TestA("hello"));
            _morph.Substitute(src => new SubstituteTest(" world", src));

            subject.Message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration2()
        {
            var subject = _morph.CreateSubject(new TestA("hello"));
            _morph.Substitute(src => new SubstituteTest(" world", src));
            _morph.Substitute(src => new SubstituteTest(" again", src));

            subject.Message.ShouldBe("hello world again");
        }
    }

    
}
