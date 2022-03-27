using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DefaultRootTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenDefaultRootProxy_WhenStringPropertyGetterCalled_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTypeReturn_ShouldReturnDefault()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric(5);

            // ASSERT
            result.ShouldBe(default);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTaskReturn_ShouldReturnCompletedTask()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task>(null);

            // ASSERT
            result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasNestedTaskReturn_ShouldReturnCompletedNestedTask()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<Task>>(null);

            // ASSERT
            result.Status.ShouldBe(TaskStatus.RanToCompletion);
            result.Result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public async Task GivenDefaultRootProxy_WhenCallHasRefTypeTaskReturn_ShouldReturnCompletedTaskWithNullResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<object>>(null);

            // ASSERT
            var expectedResult = Task.FromResult<object>(null);
            result.Status.ShouldBe(expectedResult.Status);
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasNestedRefTypeTaskReturn_ShouldReturnCompletedTaskWithNestedNullResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<Task<object>>>(null);

            // ASSERT
            var expectedResult = Task.FromResult(Task.FromResult<object>(null));
            result.Status.ShouldBe(expectedResult.Status);
            result.Result!.Status.ShouldBe(expectedResult.Result!.Status);
            result.Result!.Result.ShouldBe(expectedResult.Result!.Result);
        }
        
        [Fact]
        public async Task GivenDefaultRootProxy_WhenCallHasValueTypeTaskReturn_ShouldReturnCompletedTaskWithDefaultResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<int>>(null);

            // ASSERT
            var expectedResult = Task.FromResult<int>(default);
            result.Status.ShouldBe(expectedResult.Status);
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallValueTaskReturn_ShouldReturnCompletedValueTask()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask>(default);

            // ASSERT
            result.IsCompleted.ShouldBeTrue();
            result.IsCanceled.ShouldBeFalse();
            result.IsFaulted.ShouldBeFalse();
            result.ShouldBeOfType<ValueTask>();
        }
        
        [Fact]
        public async Task GivenDefaultRootProxy_WhenCallHasRefTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithNullResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<object>>(default);

            // ASSERT
            var expectedResult = new ValueTask<object>();
            result.IsCompleted.ShouldBeTrue();
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public async Task GivenDefaultRootProxy_WhenCallHasValueTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithDefaultResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<int>>(default);

            // ASSERT
            var expectedResult = new ValueTask<int>();
            result.IsCompleted.ShouldBeTrue();
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasNestedRefTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithNestedNullResult()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<ValueTask<Task>>>(default);

            // ASSERT
            result.IsCompleted.ShouldBeTrue();
            result.Result.IsCompleted.ShouldBeTrue();
            result.Result.Result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasArrayReturn_ShouldReturnEmptyArray()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<object[]>(default);

            // ASSERT
            result.ShouldBe(Array.Empty<object>());
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasMultiDimensionalArrayReturn_ShouldReturnEmptyArray()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<object[][][]>(default);

            // ASSERT
            result.ShouldBe(Array.Empty<object[][]>());
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasIEnumerableReturn_ShouldReturnEmptyIEnumerable()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<IEnumerable>(default);

            // ASSERT
            result.ShouldBe(new ArrayList());
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTypedIEnumerableReturn_ShouldReturnEmptyIEnumerable()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<IEnumerable<object>>(default);

            // ASSERT
            result.ShouldBe(Array.Empty<object>());
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple1Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTuple<Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple2Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple3Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple4Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple5Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple6Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple7Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple8Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task, Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
            result.Item8.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasValueTuple9Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task, Task, Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
            result.Item8.ShouldBe(Task.CompletedTask);
            result.Item9.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple1Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple2Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple3Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple4Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple5Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple6Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple7Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task, Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDefaultRootProxy_WhenCallHasTuple8Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task, Task, Tuple<Task>>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
            result.Rest.Item1.ShouldBe(Task.CompletedTask);
        }
    }
}