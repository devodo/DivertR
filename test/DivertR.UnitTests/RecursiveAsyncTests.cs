using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecursiveAsyncTests
    {
        private readonly IRedirect<IAsyncNumber> _redirect = new Redirect<IAsyncNumber>();

        [Fact]
        public async Task TestRecursiveAsync()
        {
            const int Input = 20;
            var proxy = InitTestProxy();
            var result = await proxy.GetNumber(Input);
            result.ShouldBe(Fibonacci(Input) * 2);
        }

        [Fact]
        public async Task TestRecursiveAsyncMultiThreaded()
        {
            const int Input = 20;
            const int TaskCount = 10;
            var proxy = InitTestProxy();
            
            var tasks = Enumerable.Range(0, TaskCount).Select(_ => 
                Task.Run(() => proxy.GetNumber(Input))).ToArray();

            var controlResult = Fibonacci(Input) * 2;
            foreach (var task in tasks)
            {
                (await task).ShouldBe(controlResult);
            }
        }
        
        private IAsyncNumber InitTestProxy()
        {
            var proxy = _redirect.Proxy(new AsyncNumber());

            var fibonacci = new AsyncNumber(async i =>
            {
                if (i < 2)
                {
                    return await _redirect.Relay.Next.GetNumber(i);
                }

                var num1Task = proxy.GetNumber(i - 1);
                var num2Task = proxy.GetNumber(i - 2);
                return await num1Task + await num2Task;
            });

            _redirect
                .To(x => x.GetNumber(Is<int>.Any))
                .Via<(int i, __)>(async call => await call.Root.GetNumber(call.Args.i) + await _redirect.Relay.Next.GetNumber(call.Args.i))
                .Retarget(fibonacci);
            
            return _redirect.Proxy(new AsyncNumber());
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