namespace DivertR.Record.Internal
{
    public class ActionRecordVia<TTarget> : IActionRecordVia<TTarget> where TTarget : class?
    {
        public IVia Via { get; }
        public IActionCallStream<TTarget> CallStream { get; }

        public ActionRecordVia(IVia via, IActionCallStream<TTarget> callStream)
        {
            Via = via;
            CallStream = callStream;
        }
    }

    public class ActionRecordVia<TTarget, TArgs> : IActionRecordVia<TTarget, TArgs> where TTarget : class?
    {
        public IVia Via { get; }
        public IActionCallStream<TTarget, TArgs> CallStream { get; }

        public ActionRecordVia(IVia via, IActionCallStream<TTarget, TArgs> callStream)
        {
            Via = via;
            CallStream = callStream;
        }
    }
}