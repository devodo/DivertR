using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : CallStream<IFuncRecordedCall<TTarget, TReturn>>, IFuncCallStream<TTarget, TReturn>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn>> calls, ParsedCallExpression parsedCallExpression) : base(calls)
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IFuncCallStream<TTarget, TArgs, TReturn> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>(Calls, _parsedCallExpression);
        }
        
        public ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>> Verify(Action<IFuncRecordedCall<TTarget, TReturn>, CallArguments> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return await WithArgs<TArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return await WithArgs<TArgs>().Verify(visitor);
        }
        
        internal static IFuncCallStream<TTarget, TArgs, TReturn> WithArgs<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, ParsedCallExpression parsedCallExpression) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            parsedCallExpression.Validate(valueTupleMapper);
            var mappedCall = calls.Select(call => 
                new FuncRecordedCall<TTarget, TArgs, TReturn>(call, (TArgs) valueTupleMapper.ToTuple(call.CallInfo.Arguments.InternalArgs)));
            
            return new FuncCallStream<TTarget, TArgs, TReturn>(mappedCall, parsedCallExpression);
        }
    }
        
    internal class FuncCallStream<TTarget, TArgs, TReturn> : CallStream<IFuncRecordedCall<TTarget, TArgs, TReturn>>, IFuncCallStream<TTarget, TArgs, TReturn>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TArgs, TReturn>> calls, ParsedCallExpression parsedCallExpression) : base(calls)
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IFuncCallStream<TTarget, TNewArgs, TReturn> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return FuncCallStream<TTarget, TReturn>.WithArgs<TNewArgs>(Calls, _parsedCallExpression);
        }

        public ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> Verify(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>) base.Verify();
            }

            return WithArgs<TNewArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IFuncRecordedCall<TTarget, TArgs, TReturn>> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>) base.Verify(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>) VerifyInternal(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, Task> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>) await base.Verify(v);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TNewArgs, TReturn>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TNewArgs, TReturn>>) await VerifyInternal(v);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor);
        }
        
        private IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>> VerifyInternal(Action<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        private Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TArgs, TReturn>>> VerifyInternal(Func<IFuncRecordedCall<TTarget, TArgs, TReturn>, TArgs, Task> visitor)
        {
            return base.Verify(async call => await visitor.Invoke(call, call.Args).ConfigureAwait(false));
        }
    }
}