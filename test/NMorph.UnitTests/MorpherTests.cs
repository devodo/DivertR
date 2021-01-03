using System;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests
{
    public class MorpherTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Morpher _morpher = new Morpher();

        public MorpherTests(ITestOutputHelper output)
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

        [Fact]
        public void TestRecursive()
        {
            var test = _morpher.CreateMorph<ICounter>(new Counter());
            _morpher.Substitute<ICounter>(src => new CounterTest(test, _output));

            var result = test.Sum(5);
            _output.WriteLine($"Result: {result}");
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
        private readonly IMorphSource<ITest1> _morphSource;

        public SubstituteTest(string message, IMorphSource<ITest1> morphSource)
        {
            _message = message;
            _morphSource = morphSource;
        }

        public string Message => _morphSource.ReplacedTarget.Message + _message;
    }
    
    public interface ICounter
    {
        int Sum(int n);
    }

    public class Counter : ICounter
    {
        public int Sum(int n)
        {
            if (n <= 0)
            {
                return 1;
            }
            
            return n + Sum(n - 1);
        }
    }

    public class CounterTest : ICounter
    {
        private readonly ICounter _counter;
        private readonly ITestOutputHelper _output;

        public CounterTest(ICounter counter, ITestOutputHelper output)
        {
            _counter = counter;
            _output = output;
        }
        
        public int Sum(int n)
        {
            _output.WriteLine($"{n}");
            
            if (n <= 0)
            {
                return 1;
            }
            
            return n + _counter.Sum(n - 1);
        }
    }
}
