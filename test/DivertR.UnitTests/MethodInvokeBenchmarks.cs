using System;
using System.Diagnostics;
using System.Reflection;
using DivertR.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class MethodInvokeBenchmarks
    {
        private const int Iterations = 1000;
        private const int SpeedupFactor = 4;
        private readonly ITestOutputHelper _output;

        public MethodInvokeBenchmarks(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void DelegateInvokeTest()
        {
            Func<int, int, int> testDelegate = (a, b) => a + b;
            var args = new object[] {1, 2};
            
            var dynamicInvoke = DelegateDynamicInvoke(testDelegate, args);
            var fastInvoke = DelegateFastInvoke(testDelegate, args);

            PrintResult("DynamicInvoke", dynamicInvoke);
            PrintResult("FastInvoke", fastInvoke);
            
            fastInvoke.Initial.ElapsedTicks.ShouldBeGreaterThan(dynamicInvoke.Initial.ElapsedTicks * SpeedupFactor);
            fastInvoke.Iterations.ElapsedTicks.ShouldBeLessThan(dynamicInvoke.Iterations.ElapsedTicks / SpeedupFactor);
        }

        [Fact]
        public void MethodInvokeTest()
        {
            var methodInfo = GetType().GetTypeInfo().GetMethod(nameof(TestMethod), BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] {1, 1};
            
            var methodInvoke = MethodInfoInvoke(methodInfo, args);
            var fastInvoke = MethodFastInvoke(methodInfo, args);
            
            PrintResult("DynamicInvoke", methodInvoke);
            PrintResult("FastInvoke", fastInvoke);
            
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

        private static (Stopwatch Initial, Stopwatch Iterations) DelegateDynamicInvoke(Delegate testDelegate, object[] args)
        {
            var sw1 = Stopwatch.StartNew();
            testDelegate.DynamicInvoke(args);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                testDelegate.DynamicInvoke(args);
            }
            
            sw2.Stop();

            return (sw1, sw2);
        }

        private static (Stopwatch Initial, Stopwatch Iterations) DelegateFastInvoke(Delegate testDelegate, object[] args)
        {
            var sw1 = Stopwatch.StartNew();
            var fastDelegate = testDelegate.ToDelegate();
            fastDelegate.Invoke(args);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                fastDelegate.Invoke(args);
            }
            
            sw2.Stop();
            
            return (sw1, sw2);
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
            var fastDelegate = methodInfo.ToDelegate(GetType());
            fastDelegate.Invoke(this, args);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            for (var i = 0; i < Iterations; i++)
            {
                fastDelegate.Invoke(this, args);
            }
            
            sw2.Stop();

            return (sw1, sw2);
        }
    }
}