using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Divertr.UnitTests
{
    public class RecursiveAsyncTests
    {
        private readonly IDirector<IAsyncFactorial> _director = new Director<IAsyncFactorial>();

        [Fact]
        public async Task TestRecursiveAsync()
        {
            const int factorialInput = 10;

            IAsyncFactorial FactorialFactory(int n)
            {
                return _director.Proxy(new Factorial(n, FactorialFactory));
            }

            _director.AddRedirect(new FactorialTest(_director.CallCtx));

            var result = await FactorialFactory(factorialInput).Result();
            result.ShouldBe(GetFactorial(factorialInput));
        }

        [Fact]
        public async Task TestRecursiveAsyncMultiThreaded()
        {
            const int factorialInput = 10;
            const int taskCount = 10;

            var controlResult = GetFactorial(factorialInput);

            IAsyncFactorial FactorialFactory(int n)
            {
                return _director.Proxy(new Factorial(n, FactorialFactory));
            }

            var tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(async () =>
            {
                _director.AddRedirect(new FactorialTest(_director.CallCtx));
                return await FactorialFactory(factorialInput).Result();
            })).ToArray();

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
            private readonly ICallContext<IAsyncFactorial> _src;

            public FactorialTest(ICallContext<IAsyncFactorial> src)
            {
                _src = src;
            }

            public int Number => _src.Replaced.Number;

            public async Task<int> Result()
            {
                var result = await _src.Replaced.Result();

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
