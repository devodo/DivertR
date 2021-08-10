using DivertR.Core;

namespace DivertR.Record.Internal
{
    internal abstract class RecordedCallFactory<TTarget> where TTarget : class
    {
        internal abstract RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo);
    }
    
    internal class RecordedCallFactory<TTarget, TReturn> : RecordedCallFactory<TTarget> where TTarget : class
    {
        internal override RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo)
        {
            return new RecordedCall<TTarget, TReturn>(callInfo);
        }
    }
    
    internal class RecordedCallFactory<TTarget, TReturn, T1> : RecordedCallFactory<TTarget> where TTarget : class
    {
        internal override RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo)
        {
            return new RecordedCall<TTarget, TReturn, T1>(callInfo);
        }
    }
    
    internal class RecordedCallFactory<TTarget, TReturn, T1, T2> : RecordedCallFactory<TTarget> where TTarget : class
    {
        internal override RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo)
        {
            return new RecordedCall<TTarget, TReturn, T1, T2>(callInfo);
        }
    }
    
    internal class RecordedCallFactory<TTarget, TReturn, T1, T2, T3> : RecordedCallFactory<TTarget> where TTarget : class
    {
        internal override RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo)
        {
            return new RecordedCall<TTarget, TReturn, T1, T2, T3>(callInfo);
        }
    }
    
    internal class RecordedCallFactory<TTarget, TReturn, T1, T2, T3, T4> : RecordedCallFactory<TTarget> where TTarget : class
    {
        internal override RecordedCall<TTarget> Create(CallInfo<TTarget> callInfo)
        {
            return new RecordedCall<TTarget, TReturn, T1, T2, T3, T4>(callInfo);
        }
    }
}
