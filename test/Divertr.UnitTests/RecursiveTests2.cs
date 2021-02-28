using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class RecursiveTests2
    {
        private readonly ITestOutputHelper _output;
        private readonly IRouter<INumber> _router = new Router<INumber>();

        public RecursiveTests2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestRecursiveSync()
        {
            const int input = 20;
            var proxy = InitTestProxy();
            var result = proxy.GetNumber(input);
            result.ShouldBe(Fibonacci(input) * 2);
        }

        [Fact]
        public async Task TestRecursiveSync2()
        {
            const int input = 50;
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
            var proxy = InitTestProxy();
            
            var tasks = Enumerable.Range(0, taskCount).Select(_ => 
                Task.Run(() => proxy.GetNumber(input))).ToArray();

            var controlResult = Fibonacci(input) * 2;
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }

        private INumber InitTestProxy()
        {
            var proxy = _router.Proxy(new Number());

            var fibonacci = new Number(i =>
            {
                if (i < 2)
                {
                    return _router.Relay.Next.GetNumber(i);
                }

                return proxy.GetNumber(i - 1) + proxy.GetNumber(i - 2);
            });

            var times2 = new Number(i => _router.Relay.Original.GetNumber(i) + _router.Relay.Next.GetNumber(i));

            _router
                .AddRedirect(fibonacci)
                .AddRedirect(times2);

            return _router.Proxy(new Number());;
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
