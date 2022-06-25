namespace DivertR.Record.Internal
{
    public class FuncRecordRedirect<TTarget, TReturn> : IFuncRecordRedirect<TTarget, TReturn> where TTarget : class
    {
        public IRedirect<TTarget> Redirect { get; }
        public IFuncCallStream<TTarget, TReturn> CallStream { get; }

        public FuncRecordRedirect(IRedirect<TTarget> redirect, IFuncCallStream<TTarget, TReturn> callStream)
        {
            Redirect = redirect;
            CallStream = callStream;
        }
    }

    public class FuncRecordRedirect<TTarget, TReturn, TArgs> : IFuncRecordRedirect<TTarget, TReturn, TArgs> where TTarget : class
    {
        public IRedirect<TTarget> Redirect { get; }
        public IFuncCallStream<TTarget, TReturn, TArgs> CallStream { get; }

        public FuncRecordRedirect(IRedirect<TTarget> redirect, IFuncCallStream<TTarget, TReturn, TArgs> callStream)
        {
            Redirect = redirect;
            CallStream = callStream;
        }
    }
}