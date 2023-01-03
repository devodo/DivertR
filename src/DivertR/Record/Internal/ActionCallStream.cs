using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class ActionCallStream<TTarget> : CallStream<IRecordedCall<TTarget>>, IActionCallStream<TTarget>
        where TTarget : class?
    {
        private readonly ICallValidator _callValidator;
        
        public ActionCallStream(IEnumerable<IRecordedCall<TTarget>> calls, ICallValidator callValidator) : base(calls)
        {
            _callValidator = callValidator;
        }

        public IActionCallStream<TTarget, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>(Calls, _callValidator);
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify();
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify<TArgs>(Action<IRecordedCall<TTarget, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IRecordedCall<TTarget, TArgs>>> Verify<TArgs>(Func<IRecordedCall<TTarget, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify(visitor);
        }

        internal static IActionCallStream<TTarget, TArgs> Args<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, ICallValidator callValidator) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            callValidator.Validate(valueTupleMapper);
            var mappedCall = calls.Select(call => 
                new RecordedCall<TTarget, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.CallInfo.Arguments.InternalArgs)));
            
            return new ActionCallStream<TTarget, TArgs>(mappedCall, callValidator);
        }
    }
    
    internal class ActionCallStream<TTarget, TArgs> : CallStream<IRecordedCall<TTarget, TArgs>>, IActionCallStream<TTarget, TArgs>
        where TTarget : class?
    {
        private readonly ICallValidator _callValidator;
        
        public ActionCallStream(IEnumerable<IRecordedCall<TTarget, TArgs>> calls, ICallValidator callValidator) : base(calls)
        {
            _callValidator = callValidator;
        }

        public IActionCallStream<TTarget, TNewArgs> Args<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IActionCallStream<TTarget, TNewArgs>) this;
            }

            return ActionCallStream<TTarget>.Args<TNewArgs>(Calls, _callValidator);
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TArgs>> Verify(Action<IRecordedCall<TTarget, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) base.Verify();
            }
            
            return Args<TNewArgs>().Verify();
        }

        public IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>> Verify<TNewArgs>(Action<IRecordedCall<TTarget, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IRecordedCall<TTarget, TArgs>> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) base.Verify(v);
            }
            
            return Args<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>> Verify<TNewArgs>(Func<IRecordedCall<TTarget, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IRecordedCall<TTarget, TArgs>, Task> v)
            {
                return (IVerifySnapshot<IRecordedCall<TTarget, TNewArgs>>) await base.Verify(v).ConfigureAwait(false);
            }
            
            return await Args<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }
    }
}
