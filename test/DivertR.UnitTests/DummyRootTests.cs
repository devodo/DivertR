using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Dummy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DummyRootTests
    {
        private readonly IRedirect<IFoo> _redirect;
        private readonly DummyFactory _dummyFactory = new();

        public DummyRootTests()
        {
            var settings = new DiverterSettings(dummyFactory: _dummyFactory);
            _redirect = new Redirect<IFoo>(settings);
        }

        [Fact]
        public void GivenDummyRootProxy_WhenStringPropertyGetterCalled_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTypeReturn_ShouldReturnDefault()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric(5);

            // ASSERT
            result.ShouldBe(default);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTaskReturn_ShouldReturnCompletedTask()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task?>(null);

            // ASSERT
            result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasNestedTaskReturn_ShouldReturnCompletedNestedTask()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<Task>?>(null);

            // ASSERT
            result!.Status.ShouldBe(TaskStatus.RanToCompletion);
            result.Result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public async Task GivenDummyRootProxy_WhenCallHasRefTypeTaskReturn_ShouldReturnCompletedTaskWithNullResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<object>?>(null);

            // ASSERT
            var expectedResult = Task.FromResult<object>(null!);
            result!.Status.ShouldBe(expectedResult.Status);
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasNestedRefTypeTaskReturn_ShouldReturnCompletedTaskWithNestedNullResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<Task<object>>?>(null);

            // ASSERT
            var expectedResult = Task.FromResult(Task.FromResult<object>(null!));
            result!.Status.ShouldBe(expectedResult.Status);
            result.Result.Status.ShouldBe(expectedResult.Result.Status);
            result.Result.Result.ShouldBe(expectedResult.Result.Result);
        }
        
        [Fact]
        public async Task GivenDummyRootProxy_WhenCallHasValueTypeTaskReturn_ShouldReturnCompletedTaskWithDefaultResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Task<int>?>(null);

            // ASSERT
            var expectedResult = Task.FromResult<int>(default);
            result!.Status.ShouldBe(expectedResult.Status);
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallValueTaskReturn_ShouldReturnCompletedValueTask()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask>(default);

            // ASSERT
            result.IsCompleted.ShouldBeTrue();
            result.IsCanceled.ShouldBeFalse();
            result.IsFaulted.ShouldBeFalse();
            result.ShouldBeOfType<ValueTask>();
        }
        
        [Fact]
        public async Task GivenDummyRootProxy_WhenCallHasRefTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithNullResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<object>>(default);

            // ASSERT
            var expectedResult = new ValueTask<object>();
            result.IsCompleted.ShouldBeTrue();
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public async Task GivenDummyRootProxy_WhenCallHasValueTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithDefaultResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<int>>(default);

            // ASSERT
            var expectedResult = new ValueTask<int>();
            result.IsCompleted.ShouldBeTrue();
            result.Result.ShouldBe(expectedResult.Result);
            (await result).ShouldBe(await expectedResult);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasNestedRefTypeValueTaskReturn_ShouldReturnCompletedValueTaskWithNestedNullResult()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTask<ValueTask<Task>>>(default);

            // ASSERT
            result.IsCompleted.ShouldBeTrue();
            result.Result.IsCompleted.ShouldBeTrue();
            result.Result.Result.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasArrayReturn_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<object[]?>(default);

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasMultiDimensionalArrayReturn_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<object[][][]?>(default);

            // ASSERT
            result.ShouldBeNull();
        }

        [Fact]
        public void GivenDummyRootProxy_WhenCallHasIEnumerableReturn_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<IEnumerable?>(default);

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTypedIEnumerableReturn_ShouldReturnNull()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<IEnumerable<object>?>(default);

            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTuple1Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<ValueTuple<Task>>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTuple2Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTuple3Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTuple4Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<(Task, Task, Task, Task)>(default);

            // ASSERT
            result.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasValueTuple5Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

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
        public void GivenDummyRootProxy_WhenCallHasValueTuple6Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

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
        public void GivenDummyRootProxy_WhenCallHasValueTuple7Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

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
        public void GivenDummyRootProxy_WhenCallHasValueTuple8Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

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
        public void GivenDummyRootProxy_WhenCallHasValueTuple9Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

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
        public void GivenDummyRootProxy_WhenCallHasTuple1Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple2Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple3Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple4Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple5Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple6Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple7Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task, Task>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
        }
        
        [Fact]
        public void GivenDummyRootProxy_WhenCallHasTuple8Return_ShouldReturnTupleWithDefaults()
        {
            // ARRANGE
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric<Tuple<Task, Task, Task, Task, Task, Task, Task, Tuple<Task>>?>(default);

            // ASSERT
            result!.Item1.ShouldBe(Task.CompletedTask);
            result.Item2.ShouldBe(Task.CompletedTask);
            result.Item3.ShouldBe(Task.CompletedTask);
            result.Item4.ShouldBe(Task.CompletedTask);
            result.Item5.ShouldBe(Task.CompletedTask);
            result.Item6.ShouldBe(Task.CompletedTask);
            result.Item7.ShouldBe(Task.CompletedTask);
            result.Rest.Item1.ShouldBe(Task.CompletedTask);
        }

        [Fact]
        public void GivenDummyReturnTypeDelegateVia_WhenProxyMethodReturnTypeMatches_ShouldRedirect()
        {
            // ARRANGE
            _dummyFactory
                .To(() => Is<string>.Return)
                .Via(() => "redirected")
                .Via(call => $"{call.CallNext()} call {call.Args.LastOrDefault()}".Trim());

            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");

            // ASSERT
            result.ShouldBe("redirected call hello");
            name.ShouldBe("redirected call");
            objectReturn.ShouldBeNull();
        }
        
        [Fact]
        public void GivenDummyReturnTypeInstanceVia_WhenProxyMethodReturnSubTypeMatches_ShouldNotVia()
        {
            // ARRANGE
            _dummyFactory
                .To(() => Is<object>.Return)
                .Via("redirected");
            
            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");

            // ASSERT
            result.ShouldBeNull();
            name.ShouldBeNull();
            objectReturn.ShouldBe("redirected");
        }
        
        [Fact]
        public void GivenDummyConstraintVia_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _dummyFactory
                .To(new DelegateCallConstraint(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Via(() => "redirected")
                .Via(call => $"{call.CallNext()} call {call.Args.LastOrDefault()}".Trim());

            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");
            var intReturn = proxy.EchoGeneric(1);

            // ASSERT
            result.ShouldBe("redirected call hello");
            name.ShouldBe("redirected call");
            objectReturn.ShouldBe("redirected call hello");
            intReturn.ShouldBe(0);
        }
        
        [Fact]
        public void GivenDummyConstraintInstanceVia_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _dummyFactory
                .To(new DelegateCallConstraint(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Via(() => "redirected");

            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");
            var intReturn = proxy.EchoGeneric(1);

            // ASSERT
            result.ShouldBe("redirected");
            name.ShouldBe("redirected");
            objectReturn.ShouldBe("redirected");
            intReturn.ShouldBe(0);
        }
        
        [Fact]
        public void GivenDummyCallStream_WhenProxyCalled_ShouldRecord()
        {
            // ARRANGE
            var dummyCalls = _dummyFactory
                .To(() => Is<string>.Return)
                .Record();

            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;

            // ASSERT
            var returns = new[] { result, name };
            var index = 0;
            dummyCalls
                .Verify(call => call.Return.ShouldBe(returns[index++]))
                .Count.ShouldBe(2);
        }
        
        [Fact]
        public void GivenDummyRecordStream_WhenProxyCalled_ShouldRecord()
        {
            // ARRANGE
            var dummyCalls = _dummyFactory
                .To()
                .Record();

            var proxy = _redirect.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;

            // ASSERT
            var returns = new[] { result, name };
            var index = 0;
            dummyCalls
                .To(() => Is<string>.Return)
                .Verify(call => call.Return.ShouldBe(returns[index++]))
                .Count.ShouldBe(2);
        }
    }
}