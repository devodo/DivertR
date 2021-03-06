using System.Linq;
using System.Threading;
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

        private int _proxyCount = 0;
        private IAsyncNumber InitTestProxy()
        {
            var proxy = _via.Proxy(new AsyncNumber());

            var fibonacci = new AsyncNumber(async i =>
            {
                Interlocked.Increment(ref _proxyCount);
                if (i < 2)
                {
                    return await _via.Relay.Next.GetNumber(i);
                }

                return await proxy.GetNumber(i - 1) + await proxy.GetNumber(i - 2);
            });

            var times2 = new AsyncNumber(async i =>
                await _via.Relay.Original.GetNumber(i) + await _via.Relay.Next.GetNumber(i));

            _via
                .AddRedirect(fibonacci)
                .AddRedirect(times2);

            return _via.Proxy(new AsyncNumber());;
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