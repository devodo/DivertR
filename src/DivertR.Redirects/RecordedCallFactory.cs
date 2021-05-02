using DivertR.Core;

namespace DivertR.Redirects
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
}
