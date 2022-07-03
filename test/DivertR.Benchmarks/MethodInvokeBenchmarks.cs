using System.Diagnostics;
using System.Reflection;
using DivertR.Internal;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.Benchmarks
{
    public class MethodInvokeBenchmarks
    {
        private const int Iterations = 1000;
        private const int SpeedupFactor = 2;
        private readonly ITestOutputHelper _output;

        public MethodInvokeBenchmarks(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void MethodInvokeTest()
        {
            var methodInfo = GetType().GetTypeInfo().GetMethod(nameof(TestMethod), BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] { 1, 1 };
            
            var methodInvoke = MethodInfoInvoke(methodInfo, args);
            var fastInvoke = MethodFastInvoke(methodInfo, args);
            
            PrintResult("DynamicInvoke", methodInvoke);
            PrintResult("FastInvoke", fastInvoke);
            _output.WriteLine($"Invoke speedup: {(double) methodInvoke.Iterations.ElapsedTicks / fastInvoke.Iterations.ElapsedTicks}");
            
            fastInvoke.Initial.ElapsedTicks.ShouldBeGreaterThan(methodInvoke.Initial.ElapsedTicks * SpeedupFactor);
            fastInvoke.Iterations.ElapsedTicks.ShouldBeLessThan(methodInvoke.Iterations.ElapsedTicks / SpeedupFactor);
        }
        
        private int TestMethod(int a, int b)
        {
            return a + b;
        }

        private void PrintResult(string name, (Stopwatch Initial, Stopwatch Iterations) result)
        {
            _output.WriteLine($"{name} | Initial: {result.Initial.Elapsed.TotalMilliseconds} ms | Iterations: {result.Iterations.Elapsed.TotalMilliseconds} ms");
        }

        private (Stopwatch Initial, Stopwatch Iterations) MethodInfoInvoke(MethodInfo methodInfo, object[] args)
        {
            var sw1 = Stopwatch.StartNew();
            methodInfo.Invoke(this, args);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                methodInfo.Invoke(this, args);
            }
            
            sw2.Stop();

            return (sw1, sw2);
        }

        private (Stopwatch Initial, Stopwatch Iterations) MethodFastInvoke(MethodInfo methodInfo, object[] args)
        {
            var sw1 = Stopwatch.StartNew();
            var callInvoker = new LambdaExpressionCallInvoker();
            var lambdaDelegate = callInvoker.CreateDelegate(this.GetType(), methodInfo);
            lambdaDelegate.Invoke(this, args);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                lambdaDelegate.Invoke(this, args);
            }
            
            sw2.Stop();

            return (sw1, sw2);
        }
    }
}