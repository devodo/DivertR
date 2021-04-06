using System.Linq;
using System.Threading.Tasks;
using DivertR.DynamicProxy;
using DivertR.Setup;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class RecursiveTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IVia<INumber> _via = new Via<INumber>();

        public RecursiveTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestRecursiveSync()
        {
            const int input = 20;
            var proxy = InitTestProxy();
            var result = proxy.GetNumber(input);
            var controlResult = Fibonacci(input) * 2;
            result.ShouldBe(controlResult);
        }

        [Fact]
        public async Task TestRecursiveSyncBenchmark()
        {
            const int input = 20;
            const int taskCount = 10;
            
            var tasks = Enumerable.Range(0, taskCount).Select(_ => 
                Task.Run(() => Fibonacci(input))).ToArray();
            
            var controlResult = Fibonacci(input);
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }
        
        [Fact]
        public async Task TestRecursiveSyncMultiThreaded()
        {
            const int input = 20;
            const int taskCount = 10;
            var controlResult = Fibonacci(input) * 2;
            
            var proxy = InitTestProxy();
            
            var tasks = Enumerable.Range(0, taskCount).Select(_ => 
                Task.Run(() => proxy.GetNumber(input))).ToArray();
            
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }
        
        private INumber InitTestProxy()
        {
            var proxy = _via.Proxy(new Number());

            var fibonacci = new Number(i =>
            {
                if (i < 2)
                {
                    return _via.Next.GetNumber(i);
                }

                return proxy.GetNumber(i - 1) + proxy.GetNumber(i - 2);
            });

            _via
                .Redirect(x => x.GetNumber(Is<int>.Any))
                .To<int>(i => _via.Relay.Original.GetNumber(i) + _via.Next.GetNumber(i))
                .RedirectTo(fibonacci);
            
            return _via.Proxy(new Number());
        }

        private static int Fibonacci(int n)
        {
            if (n < 2)
            {
                return n;
            }

            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}
