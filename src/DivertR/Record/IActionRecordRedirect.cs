namespace DivertR.Record
{
    public interface IActionRecordRedirect<TTarget> where TTarget : class?
    {
        IRedirect Redirect { get; }
        public IActionCallStream<TTarget> CallStream { get; }
    }
    
    public interface IActionRecordRedirect<TTarget, TArgs> where TTarget : class?
    {
        IRedirect Redirect { get; }
        public IActionCallStream<TTarget, TArgs> CallStream { get; }
    }
}