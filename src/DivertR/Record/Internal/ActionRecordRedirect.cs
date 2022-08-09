namespace DivertR.Record.Internal
{
    public class ActionRecordRedirect<TTarget> : IActionRecordRedirect<TTarget> where TTarget : class?
    {
        public IRedirect Redirect { get; }
        public IActionCallStream<TTarget> CallStream { get; }

        public ActionRecordRedirect(IRedirect redirect, IActionCallStream<TTarget> callStream)
        {
            Redirect = redirect;
            CallStream = callStream;
        }
    }

    public class ActionRecordRedirect<TTarget, TArgs> : IActionRecordRedirect<TTarget, TArgs> where TTarget : class?
    {
        public IRedirect Redirect { get; }
        public IActionCallStream<TTarget, TArgs> CallStream { get; }

        public ActionRecordRedirect(IRedirect redirect, IActionCallStream<TTarget, TArgs> callStream)
        {
            Redirect = redirect;
            CallStream = callStream;
        }
    }
}