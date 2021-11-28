using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecursiveTests
    {
        private readonly IVia<INumber> _via = Via.For<INumber>();

        [Fact]
        public void TestRecursiveSync()
        {
            const int Input = 20;
            var proxy = InitTestProxy();
            var result = proxy.GetNumber(Input);
            var controlResult = Fibonacci(Input) * 2;
            result.ShouldBe(controlResult);
        }

        [Fact]
        public async Task TestRecursiveSyncBenchmark()
        {
            const int Input = 20;
            const int TaskCount = 10;
            
            var tasks = Enumerable.Range(0, TaskCount).Select(_ => 
                Task.Run(() => Fibonacci(Input))).ToArray();
            
            var controlResult = Fibonacci(Input);
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }
        
        [Fact]
        public async Task TestRecursiveSyncMultiThreaded()
        {
            const int Input = 20;
            const int TaskCount = 10;
            var controlResult = Fibonacci(Input) * 2;
            
            var proxy = InitTestProxy();
            
            var tasks = Enumerable.Range(0, TaskCount).Select(_ => 
                Task.Run(() => proxy.GetNumber(Input))).ToArray();
            
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
                    return _via.Relay.Next.GetNumber(i);
                }

                return proxy.GetNumber(i - 1) + proxy.GetNumber(i - 2);
            });

            _via
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect<(int i, __)>(call => call.Relay.Root.GetNumber(call.Args.i) + _via.Relay.Next.GetNumber(call.Args.i))
                .Retarget(fibonacci);

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
