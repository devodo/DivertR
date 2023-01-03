using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Internal;

namespace DivertR.Record.Internal
{
    internal class FuncCallStream<TTarget, TReturn> : CallStream<IFuncRecordedCall<TTarget, TReturn>>, IFuncCallStream<TTarget, TReturn>
        where TTarget : class?
    {
        private readonly ICallValidator _callValidator;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn>> calls, ICallValidator callValidator) : base(calls)
        {
            _callValidator = callValidator;
        }

        public IFuncCallStream<TTarget, TReturn, TArgs> Args<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>(Calls, _callValidator);
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn>>> Verify(Func<IFuncRecordedCall<TTarget, TReturn>, CallArguments, Task> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>() where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify<TArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify(visitor);
        }

        public Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>>> Verify<TArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> visitor) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            return Args<TArgs>().Verify(visitor);
        }

        internal static IFuncCallStream<TTarget, TReturn, TArgs> Args<TArgs>(IEnumerable<IRecordedCall<TTarget>> calls, ICallValidator callValidator) where TArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            var valueTupleMapper = ValueTupleMapperFactory.Create<TArgs>();
            callValidator.Validate(valueTupleMapper);
            var mappedCall = calls.Select(call => 
                new FuncRecordedCall<TTarget, TReturn, TArgs>(call, (TArgs) valueTupleMapper.ToTuple(call.CallInfo.Arguments.InternalArgs)));
            
            return new FuncCallStream<TTarget, TReturn, TArgs>(mappedCall, callValidator);
        }
    }
        
    internal class FuncCallStream<TTarget, TReturn, TArgs> : CallStream<IFuncRecordedCall<TTarget, TReturn, TArgs>>, IFuncCallStream<TTarget, TReturn, TArgs>
        where TTarget : class?
    {
        private readonly ICallValidator _callValidator;

        public FuncCallStream(IEnumerable<IFuncRecordedCall<TTarget, TReturn, TArgs>> calls, ICallValidator callValidator) : base(calls)
        {
            _callValidator = callValidator;
        }

        public IFuncCallStream<TTarget, TReturn, TNewArgs> Args<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IFuncCallStream<TTarget, TReturn, TNewArgs>) this;
            }
            
            return FuncCallStream<TTarget, TReturn>.Args<TNewArgs>(Calls, _callValidator);
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TArgs>> Verify(Action<IFuncRecordedCall<TTarget, TReturn, TArgs>, TArgs> visitor)
        {
            return base.Verify(call => visitor.Invoke(call, call.Args));
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>() where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (typeof(TNewArgs) == typeof(TArgs))
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) base.Verify();
            }

            return Args<TNewArgs>().Verify();
        }

        public IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> Verify<TNewArgs>(Action<IFuncRecordedCall<TTarget, TReturn, TNewArgs>> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Action<IFuncRecordedCall<TTarget, TReturn, TArgs>> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) base.Verify(v);
            }
            
            return Args<TNewArgs>().Verify(visitor);
        }

        public async Task<IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>> Verify<TNewArgs>(Func<IFuncRecordedCall<TTarget, TReturn, TNewArgs>, Task> visitor) where TNewArgs : struct, IStructuralComparable, IStructuralEquatable, IComparable
        {
            if (visitor is Func<IFuncRecordedCall<TTarget, TReturn, TArgs>, Task> v)
            {
                return (IVerifySnapshot<IFuncRecordedCall<TTarget, TReturn, TNewArgs>>) await base.Verify(v).ConfigureAwait(false);
            }
            
            return await Args<TNewArgs>().Verify(visitor).ConfigureAwait(false);
        }
    }

    internal class FuncCallStream<TReturn> : CallStream<IFuncRecordedCall<TReturn>>, IFuncCallStream<TReturn>
    {
        public FuncCallStream(IEnumerable<IFuncRecordedCall<TReturn>> calls) : base(calls)
        {
        }
    }
}
