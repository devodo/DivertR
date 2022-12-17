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
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly IFoo _proxy;

        public ValueTupleMapperTests()
        {
            _proxy = _redirect.Proxy(new Foo("MrFoo"));
        }

        [Fact]
        public void GivenViaValueTupleWithTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Via<(__, string input, __)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{call.Args.input} viaed";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello viaed", true));
        }
        
        [Fact]
        public void GivenViaValueTupleWithLessTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Via<(__, string input)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{args[1]} viaed";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello viaed", true));
        }
        
        [Fact]
        public void GivenViaValueTupleWithMoreTypeAndDiscards_ShouldMap()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Via<(__, string input, bool input2, __)>(call =>
                {
                    var args = call.CallInfo.Arguments.ToArray();
                    args[1] = $"{args[1]} viaed";

                    return call.CallNext(args);
                });

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello viaed", true));
        }
        
        [Fact]
        public void GivenViaValueTupleWithOnlyDiscards_ShouldIgnore()
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

            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any, Is<string>.Any, Is<bool>.Any))
                .Via<ValueTuple>(Callback)
                .Via<ValueTuple<__>>(Callback)
                .Via<(__, __)>(Callback)
                .Via<(__, __, __)>(Callback)
                .Via<(__, __, __, __)>(Callback)
                .Via<(__, __, __, __, __)>(Callback)
                .Via<(__, __, __, __, __, __)>(Callback)
                .Via<(__, __, __, __, __, __, __)>(Callback)
                .Via<(__, __, __, __, __, __, __, __)>(Callback)
                .Via<(__, __, __, __, __, __, __, __, __)>(Callback)
                .Via<(__, __, __, __, __, __, __, __, __, __)>(Callback);

            // ACT
            var result = _proxy.EchoGeneric(1, "hello", true);
            
            // ASSERT
            result.ShouldBe((1, "hello 10 9 8 7 6 5 4 3 2 1 0", true));
        }
    }
}