using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests.MorphTests
{
    public class RecursiveTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Morph _morph = new Morph();

        public RecursiveTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestRecursiveSync()
        {
            const int factorialInput = 10;

            IFactorial FactorialFactory(int n)
            {
                return _morph.Create<IFactorial>(new Factorial(n, FactorialFactory));
            }
            
            _morph.Alter<IFactorial>().Replace(src => new FactorialTest(src, _output));

            var result = FactorialFactory(factorialInput).Result();
            result.ShouldBe(GetFactorial(factorialInput));
        }

        [Fact]
        public async Task TestRecursiveSyncMultiThreaded()
        {
            const int factorialInput = 10;
            const int taskCount = 10;

            IFactorial FactorialFactory(int n)
            {
                return _morph.Create<IFactorial>(new Factorial(n, FactorialFactory));
            }

            var tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(() =>
            {
                _morph.Alter<IFactorial>().Replace(src => new FactorialTest(src, _output));

                return FactorialFactory(factorialInput).Result();
            })).ToArray();

            var controlResult = GetFactorial(factorialInput);
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }

        private static int GetFactorial(int n)
        {
            if (n <= 0)
            {
                return 1;
            }

            return n * GetFactorial(n - 1);
        }
        
        private class Factorial : IFactorial
        {
            private readonly Func<int, IFactorial> _factorialFactory;

            public Factorial(int number, Func<int, IFactorial> factorialFactory)
            {
                Number = number;
                _factorialFactory = factorialFactory;
            }

            public int Number { get; }

            public int Result()
            {
                if (Number <= 0)
                {
                    return 1;
                }

                return Number * _factorialFactory(Number - 1).Result();
            }
        }

        private class FactorialTest : IFactorial
        {
            private readonly IInvocationContext<IFactorial> _src;
            private readonly ITestOutputHelper _output;

            public FactorialTest(IInvocationContext<IFactorial> src, ITestOutputHelper output)
            {
                _src = src;
                _output = output;
            }

            public int Number => _src.Previous.Number;

            public int Result()
            {
                var result = _src.Previous.Result();
                
                _output.WriteLine($"{Number} - {result}");

                return result;
            }
        }
    }
    
    public interface IFactorial
    {
        public int Number { get; }
        int Result();
    }
}
