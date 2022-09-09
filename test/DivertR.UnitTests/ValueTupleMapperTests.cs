using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ValueTupleMapperTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public ValueTupleMapperTests()
        {
            _proxy = _via.Proxy(new Foo("MrFoo"));
        }

        [Fact]
        public void GivenRedirectValueTupleWithTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Redirect<(__, string input, __)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{call.Args.input} redirected";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello redirected", true));
        }
        
        [Fact]
        public void GivenRedirectValueTupleWithLessTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Redirect<(__, string input)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{args[1]} redirected";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello redirected", true));
        }
        
        [Fact]
        public void GivenRedirectValueTupleWithMoreTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Redirect<(__, string input, bool input2, __)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{args[1]} redirected";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello redirected", true));
        }
        
        [Fact]
        public void GivenRedirectValueTupleWithOnlyDiscards_ShouldIgnore()
        {
            // ARRANGE

            TReturn Callback<TTarget, TReturn, TArgs>(IFuncRedirectCall<TTarget, TReturn, TArgs> call)
                where TTarget : class?
                where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
            {
                var args = call.CallInfo.Arguments.ToArray();
                args[1] = $"{args[1]} {((ITuple) call.Args).Length}";

                return call.CallNext(args);
            }

            _via
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Redirect<ValueTuple>(Callback)
                .Redirect<ValueTuple<__>>(Callback)
                .Redirect<(__, __)>(Callback)
                .Redirect<(__, __, __)>(Callback)
                .Redirect<(__, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __, __, __, __, __)>(Callback)
                .Redirect<(__, __, __, __, __, __, __, __, __, __)>(Callback);

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello 10 9 8 7 6 5 4 3 2 1 0", true));
        }
    }
}