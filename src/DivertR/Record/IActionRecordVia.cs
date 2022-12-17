namespace DivertR.Record
{
    public interface IActionRecordVia<TTarget> where TTarget : class?
    {
        IVia Via { get; }
        public IActionCallStream<TTarget> CallStream { get; }
    }
    
    public interface IActionRecordVia<TTarget, TArgs> where TTarget : class?
    {
        IVia Via { get; }
        public IActionCallStream<TTarget, TArgs> CallStream { get; }
    }
}