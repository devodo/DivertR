using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests.MorpherTests
{
    public class SubstituteTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Morpher _morpher = new Morpher();

        public SubstituteTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration()
        {
            var test = _morpher.CreateMorph<ITest1>(new TestA("hello"));
            _morpher.Substitute<ITest1>(src => new SubstituteTest(" world", src));

            test.Message.ShouldBe("hello world");
        }
        
        [Fact]
        public void GivenMultipleIntercepts_ShouldChainAlteration2()
        {
            var test = _morpher.CreateMorph<ITest1>(new TestA("hello"));
            _morpher.Substitute<ITest1>(src => new SubstituteTest(" world", src));
            _morpher.Substitute<ITest1>(src => new SubstituteTest(" again", src));

            test.Message.ShouldBe("hello world again");
        }
    }

    public interface ITest1
    {
        string Message { get; }
    }
    
    public class TestA : ITest1
    {
        public string Message { get; }

        public TestA(string message)
        {
            Message = message;
        }
    }
    
    public class SubstituteTest : ITest1
    {
        private readonly string _message;
        private readonly IMorphInvocation<ITest1> _morphInvocation;

        public SubstituteTest(string message, IMorphInvocation<ITest1> morphInvocation)
        {
            _message = message;
            _morphInvocation = morphInvocation;
        }

        public string Message => _morphInvocation.ReplacedTarget.Message + _message;
    }
}
