using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using DivertR.Core;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class MethodInvokerTests
    {
        private readonly ITestOutputHelper _output;

        public MethodInvokerTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void DelegateInvokeBenchmark()
        {
            const int iterations = 100000;
            var count = 0;
            Func<int, int, bool> func = (a, b) =>
            {
                Interlocked.Increment(ref count);
                return a == b;
            };

            var ticks0 = DelegateDynamicInvoke(func, iterations);
            Assert.Equal(iterations, count);

            var ticks1 = FastDelegateInvoke(func, iterations);
            Assert.Equal(iterations * 2, count);

            Assert.True(ticks1 * 5 < ticks0);
        }
        
        private long DelegateDynamicInvoke(Delegate d, int iterations)
        {
            var sw0 = Stopwatch.StartNew();
            d.DynamicInvoke(1, 1);
            sw0.Stop();

            _output.WriteLine($"DelegateDynamicInvoke - First - MS: {sw0.ElapsedMilliseconds}");

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < iterations; i++)
            {
                d.DynamicInvoke(i, i);
            }
            
            sw1.Stop();

            _output.WriteLine($"DelegateDynamicInvoke -  Rest - MS: {sw1.ElapsedMilliseconds}");
            return sw1.ElapsedTicks;
        }

        private long FastDelegateInvoke(Delegate d, int iterations)
        {
            var sw0 = Stopwatch.StartNew();
            var x = d.ToDelegate();
            x.Invoke(new object[] {1, 1});
            sw0.Stop();

            _output.WriteLine($"FastDelegateInvoke - First - MS: {sw0.ElapsedMilliseconds}");

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < iterations; i++)
            {
                x.Invoke(new object[] {1, 1});
            }
            
            sw1.Stop();

            _output.WriteLine($"FastDelegateInvoke -  Rest - MS: {sw1.ElapsedMilliseconds}");
            return sw1.ElapsedTicks;
        }
        
        private bool TestMethod(int a, int b)
        {
            return a == b;
        }
        
        [Fact]
        public void MethodInvokeBenchmark()
        {
            const int iterations = 1000000;
            var methodInfo = GetType().GetTypeInfo().GetMethod("TestMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] {1, 1};
            
            var ticks0 = MethodInfoInvoke(methodInfo, args, iterations);
            var ticks2 = MethodToDelegateInvoke(methodInfo, args, iterations);
            
            Assert.True(ticks2 * 8 < ticks0);
        }
        
        private long MethodInfoInvoke(MethodInfo methodInfo, object[] args, int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            methodInfo.Invoke(this, args);
            stopwatch.Stop();

            _output.WriteLine($"MethodInfoInvoke - First - MS: {stopwatch.ElapsedMilliseconds}");

            stopwatch.Restart();
            for (var i = 1; i < iterations; i++)
            {
                methodInfo.Invoke(this, args);
            }
            
            stopwatch.Stop();
            _output.WriteLine($"MethodInfoInvoke -  Rest - MS: {stopwatch.ElapsedMilliseconds}");
            return stopwatch.ElapsedTicks;
        }

        private long MethodToDelegateInvoke(MethodInfo methodInfo, object[] args, int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            var x = methodInfo.ToDelegate(GetType());
            x.Invoke(this, args);
            stopwatch.Stop();
            _output.WriteLine($"MethodEfficientInvoker - First - MS: {stopwatch.ElapsedMilliseconds}");
            
            stopwatch.Restart();
            for (var i = 1; i < iterations; i++)
            {
                x.Invoke(this, args);
            }
            
            stopwatch.Stop();

            _output.WriteLine($"MethodEfficientInvoker -  Rest - MS: {stopwatch.ElapsedMilliseconds}");
            return stopwatch.ElapsedTicks;
        }
    }
}