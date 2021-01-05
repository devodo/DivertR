using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace NMorph.UnitTests.MorphTests
{
    public class RecursiveAsyncTests
    {
        private readonly MorphSet _morphSet = new MorphSet();

        [Fact]
        public async Task TestRecursiveAsync()
        {
            const int factorialInput = 10;

            var controlResult = GetFactorial(factorialInput);
            
            IAsyncFactorial MorphFactorialFactory(int n)
            {
                return _morphSet.CreateMorph<IAsyncFactorial>(new Factorial(n, MorphFactorialFactory));
            }

            var morph = MorphFactorialFactory(factorialInput);
            _morphSet.Substitute<IAsyncFactorial>(src => new FactorialTest(src));

            var result = await morph.Result();
            result.ShouldBe(controlResult);
        }

        [Fact]
        public async Task TestRecursiveAsyncMultiThreaded()
        {
            const int factorialInput = 10;
            const int taskCount = 10;

            var controlResult = GetFactorial(factorialInput);

            IAsyncFactorial MorphFactorialFactory(int n)
            {
                return _morphSet.CreateMorph<IAsyncFactorial>(new Factorial(n, MorphFactorialFactory));
            }

            var tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(async () =>
            {
                var morph = MorphFactorialFactory(factorialInput);
                _morphSet.Substitute<IAsyncFactorial>(src => new FactorialTest(src));

                return await morph.Result();
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
        
        private class Factorial : IAsyncFactorial
        {
            private readonly Func<int, IAsyncFactorial> _factorialFactory;

            public Factorial(int number, Func<int, IAsyncFactorial> factorialFactory)
            {
                Number = number;
                _factorialFactory = factorialFactory;
            }

            public int Number { get; }

            public async Task<int> Result()
            {
                if (Number <= 0)
                {
                    return 1;
                }

                await Task.Yield();

                var result = (await _factorialFactory(Number - 1).Result()) * Number;
                
                await Task.Yield();

                return result;
            }
        }

        private class FactorialTest : IAsyncFactorial
        {
            private readonly IInvocationContext<IAsyncFactorial> _src;

            public FactorialTest(IInvocationContext<IAsyncFactorial> src)
            {
                _src = src;
            }

            public int Number => _src.Previous.Number;

            public async Task<int> Result()
            {
                var result = await _src.Previous.Result();

                return result;
            }
        }
    }
    
    public interface IAsyncFactorial
    {
        public int Number { get; }
        Task<int> Result();
    }
}
