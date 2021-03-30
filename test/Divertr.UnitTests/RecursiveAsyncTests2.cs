using System.Linq;
using System.Threading.Tasks;
using DivertR.Core;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class RecursiveAsyncTests2
    {
        private readonly ITestOutputHelper _output;
        private readonly IVia<IAsyncNumber> _via = new Via<IAsyncNumber>();

        public RecursiveAsyncTests2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestRecursiveAsync()
        {
            const int input = 20;
            var proxy = InitTestProxy();
            var result = await proxy.GetNumber(input);
            result.ShouldBe(Fibonacci(input) * 2);
        }

        [Fact]
        public async Task TestRecursiveAsyncMultiThreaded()
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
        
        private IAsyncNumber InitTestProxy()
        {
            var proxy = _via.Proxy(new AsyncNumber());

            var fibonacci = new AsyncNumber(async i =>
            {
                if (i < 2)
                {
                    return await _via.Relay.Next.GetNumber(i);
                }

                var num1Task = proxy.GetNumber(i - 1);
                var num2Task = proxy.GetNumber(i - 2);
                return await num1Task + await num2Task;
            });

            var times2 = new AsyncNumber(async i =>
                await _via.Relay.Original.GetNumber(i) + await _via.Relay.Next.GetNumber(i));

            _via
                .RedirectTo(times2)
                .RedirectTo(fibonacci);
            
            return _via.Proxy(new AsyncNumber());
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