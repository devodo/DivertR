using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests.MorphTests
{
    public class MorphGroupTests
    {
        private readonly ITestOutputHelper _output;
        private readonly MorphGroup _morphGroup = new MorphGroup();
        
        public MorphGroupTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void GivenMorphGroup_ShouldSelect()
        {
            var subject = _morphGroup.Select<ITestSubject>().CreateSubject();
            _morphGroup.Select<ITestSubject>().Substitute(src => new SubstituteTest("hello morph", src));

            subject.Message.ShouldBe("hello morph");
        }
    }
}