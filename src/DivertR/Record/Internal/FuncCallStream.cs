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

        public IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
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

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }
        
        internal static IFuncCallStream<TTarget, TReturn, TArgs> WithArgs<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, ParsedCallExpression parsedCallExpression) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            parsedCallExpression.Validate(valueTupleMapper);
            var mappedCall = calls.Select(call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.CallInfo.Arguments.InternalArgs)));
            
            return new FuncCallStream<TTarget, TReturn, TArgs>(mappedCall, parsedCallExpression);
        }
    }
        
    internal class FuncCallStream<TTarget, TReturn, TArgs> : CallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>, IFuncCallStream<TTarget, TReturn, TArgs>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> calls, ParsedCallExpression parsedCallExpression) : base(calls)
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IFuncCallStream<TTarget, TReturn, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IFuncCallStream<TTarget, TReturn, TNewArgs>) this;
            }
            
            return FuncCallStream<TTarget, TReturn>.WithArgs<TNewArgs>(Calls, _parsedCallExpression);
        }

        public ICallStream<TMap> Map<TMap>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) base.Verify();
            }

            return WithArgs<TNewArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) base.Verify(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) VerifyInternal(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) await base.Verify(v).ConfigureAwait(false);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) await VerifyInternal(v).ConfigureAwait(false);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }
        
        private IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> VerifyInternal(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        private Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> VerifyInternal(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }
    }
}