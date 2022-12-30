namespace DivertR.Record.Internal
{
    internal class FuncRecordedCall<TTarget, TReturn> : RecordedCallWrapped<TTarget>, IFuncRecordedCall<TTarget, TReturn> where TTarget : class?
    {
        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall) : base(recordedCall)
        {
        }

        public new TReturn? Return => (TReturn?) base.Return;

        public new TReturn? ReturnOrDefault => (TReturn?) (base.ReturnOrDefault ?? default(TReturn?));
    }
    
    internal class FuncRecordedCall<TTarget, TReturn, TArgs> : RecordedCall<TTarget, TArgs>, IFuncRecordedCall<TTarget, TReturn, TArgs> where TTarget : class?
    {
        public FuncRecordedCall(IRecordedCall<TTarget> recordedCall, TArgs args) : base(recordedCall, args)
        {
        }

        public new TReturn? Return => (TReturn?) base.Return;
        
        public new TReturn? ReturnOrDefault => (TReturn?) (base.ReturnOrDefault ?? default(TReturn?));
    }
    
    internal class FuncRecordedCall<TReturn> : RecordedCallWrapped, IFuncRecordedCall<TReturn>
    {
        public FuncRecordedCall(IRecordedCall recordedCall) : base(recordedCall)
        {
        }
        
        public new TReturn? Return => (TReturn?) base.Return;
        
        public new TReturn? ReturnOrDefault => (TReturn?) (base.ReturnOrDefault ?? default(TReturn?));
    }
}
