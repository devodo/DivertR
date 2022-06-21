﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : CallStream<IRecordedCall<TTarget>>, IActionCallStream<TTarget>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> calls, ParsedCallExpression parsedCallExpression) : base(calls)
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IActionCallStream<TTarget, TArgs> WithArgs<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>(Calls, _parsedCallExpression);
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget>, CallArguments, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IRecordedCall<TTarget>> Verify(Action<IRecordedCall<TTarget>, CallArguments> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget>>> Verify(Func<IRecordedCall<TTarget>, CallArguments, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify();
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return WithArgs<TArgs>().Verify(visitor);
        }
        
        internal static IActionCallStream<TTarget, TArgs> WithArgs<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, ParsedCallExpression parsedCallExpression) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            parsedCallExpression.Validate(valueTupleMapper);
            var mappedCall = calls.Select(call => 
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.CallInfo.Arguments.InternalArgs)));
            
            return new ActionCallStream<TTarget, TArgs>(mappedCall, parsedCallExpression);
        }
    }
    
    internal class ActionCallStream<TTarget, TArgs> : CallStream<IRecordedCall<TTarget, TArgs>>, IActionCallStream<TTarget, TArgs>
        where TTarget : class
    {
        private readonly ParsedCallExpression _parsedCallExpression;
        
        public ActionCallStream(IEnumerable<IRecordedCall<TTarget, TArgs>> calls, ParsedCallExpression parsedCallExpression) : base(calls)
        {
            _parsedCallExpression = parsedCallExpression;
        }

        public IActionCallStream<TTarget, TNewArgs> WithArgs<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return ActionCallStream<TTarget>.WithArgs<TNewArgs>(Calls, _parsedCallExpression);
        }

        public ICallStream<TMap> Map<TMap>(Func<IRecordedCall<TTarget, TArgs>, TArgs, TMap> mapper)
        {
            return new CallStream<TMap>(Calls.Select(call => mapper.Invoke(call, call.Args)));
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) base.Verify();
            }
            
            return WithArgs<TNewArgs>().Verify();
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IRecordedCall<TTarget, TArgs>> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) base.Verify(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>, TNewArgs> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IRecordedCall<TTarget, TArgs>, TArgs> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) VerifyInternal(v);
            }
            
            return WithArgs<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IRecordedCall<TTarget, TArgs>, Task> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) await base.Verify(v).ConfigureAwait(false);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }

        public async Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, TNewArgs, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) await VerifyInternal(v).ConfigureAwait(false);
            }
            
            return await WithArgs<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }
        
        private IVerifySnapshot<IRecordedCall<TTarget, TArgs>> VerifyInternal(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        private Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> VerifyInternal(Func<IRecordedCall<TTarget, TArgs>, TArgs, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }
    }
}