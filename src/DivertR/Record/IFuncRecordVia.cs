namespace DivertR.Record
{
    public interface IFuncRecordVia<TTarget, TReturn> where TTarget : class?
    {
        IVia Via { get; }
        public IFuncCallStream<TTarget, TReturn> CallStream { get; }
    }
    
    public interface IFuncRecordVia<TTarget, TReturn, TArgs> where TTarget : class?
    {
        IVia Via { get; }
        public IFuncCallStream<TTarget, TReturn, TArgs> CallStream { get; }
    }
    
    public interface IFuncRecordVia<TReturn>
    {
        IVia Via { get; }
        public IFuncCallStream<TReturn> CallStream { get; }
    }
}