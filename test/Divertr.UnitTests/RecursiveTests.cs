using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class RecursiveTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IRouter<IFactorial> _router = new Router<IFactorial>();

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
                return _router.Proxy(new Factorial(n, FactorialFactory));
            }
            
            _router.AddRedirect(new FactorialTest(_router.Relay.Next, _output));

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
                return _router.Proxy(new Factorial(n, FactorialFactory));
            }

            var tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(() =>
            {
                _router.AddRedirect(new FactorialTest(_router.Relay.Next, _output));

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
            private readonly IFactorial _src;
            private readonly ITestOutputHelper _output;

            public FactorialTest(IFactorial src, ITestOutputHelper output)
            {
                _src = src;
                _output = output;
            }

            public int Number => _src.Number;

            public int Result()
            {
                var result = _src.Result();
                
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
