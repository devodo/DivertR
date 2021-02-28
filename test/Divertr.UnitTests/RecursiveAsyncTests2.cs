using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class RecursiveAsyncTests2
    {
        private readonly ITestOutputHelper _output;
        private readonly IRouter<IAsyncNumber> _router = new Router<IAsyncNumber>();

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
            const int input = 50;
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
            var proxy = _router.Proxy(new AsyncNumber());

            var fibonacci = new AsyncNumber(async i =>
            {
                if (i < 2)
                {
                    return await _router.Relay.Next.GetNumber(i);
                }

                return await proxy.GetNumber(i - 1) + await proxy.GetNumber(i - 2);
            });

            var times2 = new AsyncNumber(async i =>
                await _router.Relay.Original.GetNumber(i) + await _router.Relay.Next.GetNumber(i));

            _router
                .AddRedirect(fibonacci)
                .AddRedirect(times2);

            return _router.Proxy(new AsyncNumber());;
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
