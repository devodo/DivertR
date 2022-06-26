namespace DivertR.Record
{
    public interface IFuncRecordRedirect<TTarget, TReturn> where TTarget : class
    {
        IRedirect Redirect { get; }
        public IFuncCallStream<TTarget, TReturn> CallStream { get; }
    }
    
    public interface IFuncRecordRedirect<TTarget, TReturn, TArgs> where TTarget : class
    {
        IRedirect Redirect { get; }
        public IFuncCallStream<TTarget, TReturn, TArgs> CallStream { get; }
    }
}