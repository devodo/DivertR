using System;
using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal abstract class RecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
        private readonly object _returnedLock = new object();
        
        private ICallReturn? _callReturn;
        public CallInfo<TTarget> CallInfo { get; }
        
        public ICallReturn? Returned
        {
            get
            {
                lock (_returnedLock)
                {
                    return _callReturn;
                }
            }

            protected set
            {
                lock (_returnedLock)
                {
                    _callReturn = value;
                }
            }
        }

        protected RecordedCall(CallInfo<TTarget> callInfo)
        {
            CallInfo = callInfo;
        }

        internal abstract void SetReturned(object? returnedObject);

        internal abstract void SetException(Exception exception);
    }
    
    internal class RecordedCall<TTarget, TReturn> : RecordedCall<TTarget>, IRecordedCall<TTarget, TReturn> where TTarget : class
    {
        public new ICallReturn<TReturn>? Returned => (ICallReturn<TReturn>?) base.Returned;
        
        internal RecordedCall(CallInfo<TTarget> callInfo)
            : base(callInfo)
        {
        }
        
        internal override void SetReturned(object? returnedObject)
        {
            base.Returned = new CallReturn<TReturn>((TReturn) returnedObject!, null);
        }
        
        internal override void SetException(Exception exception)
        {
            base.Returned = new CallReturn<TReturn>(default!, exception);
        }
    }

    internal class RecordedCall<TTarget, TReturn, T1> : RecordedCall<TTarget, TReturn>, IRecordedCall<TTarget, TReturn, T1> where TTarget : class
    {
        internal RecordedCall(CallInfo<TTarget> callInfo) : base(callInfo) { }

        public T1 Arg1 => (T1) CallInfo.Arguments[0];
    }
    
    internal class RecordedCall<TTarget, TReturn, T1, T2> : RecordedCall<TTarget, TReturn, T1>, IRecordedCall<TTarget, TReturn, T1, T2> where TTarget : class
    {
        internal RecordedCall(CallInfo<TTarget> callInfo) : base(callInfo) { }

        public T2 Arg2 => (T2) CallInfo.Arguments[1];
    }
    
    internal class RecordedCall<TTarget, TReturn, T1, T2, T3> : RecordedCall<TTarget, TReturn, T1, T2>, IRecordedCall<TTarget, TReturn, T1, T2, T3> where TTarget : class
    {
        internal RecordedCall(CallInfo<TTarget> callInfo) : base(callInfo) { }

        public T3 Arg3 => (T3) CallInfo.Arguments[2];
    }
    
    internal class RecordedCall<TTarget, TReturn, T1, T2, T3, T4> : RecordedCall<TTarget, TReturn, T1, T2, T3>, IRecordedCall<TTarget, TReturn, T1, T2, T3, T4> where TTarget : class
    {
        internal RecordedCall(CallInfo<TTarget> callInfo) : base(callInfo) { }

        public T4 Arg4 => (T4) CallInfo.Arguments[3];
    }
}
