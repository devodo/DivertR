using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace NMorph.UnitTests.MorpherTests
{
    public class RecursiveTests
    {
        private readonly Morpher _morpher = new Morpher();

        [Fact]
        public void TestRecursiveSync()
        {
            const int factorialInput = 10;

            var controlResult = GetFactorial(factorialInput);
            
            IFactorial MorphFactorialFactory(int n)
            {
                return _morpher.CreateMorph<IFactorial>(new Factorial(n, MorphFactorialFactory));
            }

            var morph = MorphFactorialFactory(factorialInput);
            _morpher.Substitute<IFactorial>(src => new FactorialTest(src));

            var result = morph.Result();
            result.ShouldBe(controlResult);
        }

        [Fact]
        public async Task TestRecursiveSyncMultiThreaded()
        {
            const int factorialInput = 10;
            const int taskCount = 10;

            var controlResult = GetFactorial(factorialInput);

            IFactorial MorphFactorialFactory(int n)
            {
                return _morpher.CreateMorph<IFactorial>(new Factorial(n, MorphFactorialFactory));
            }

            var tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(() =>
            {
                var morph = MorphFactorialFactory(factorialInput);
                _morpher.Substitute<IFactorial>(src => new FactorialTest(src));

                return morph.Result();
            })).ToArray();

            Task.WaitAll(tasks);

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
            private readonly IMorphInvocation<IFactorial> _src;

            public FactorialTest(IMorphInvocation<IFactorial> src)
            {
                _src = src;
            }

            public int Number => _src.ReplacedTarget.Number;

            public int Result()
            {
                var result = _src.ReplacedTarget.Result();

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
