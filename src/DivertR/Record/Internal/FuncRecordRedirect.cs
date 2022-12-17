namespace DivertR.Record.Internal
{
    public class FuncRecordVia<TTarget, TReturn> : IFuncRecordVia<TTarget, TReturn> where TTarget : class?
    {
        public IVia Via { get; }
        public IFuncCallStream<TTarget, TReturn> CallStream { get; }

        public FuncRecordVia(IVia via, IFuncCallStream<TTarget, TReturn> callStream)
        {
            Via = via;
            CallStream = callStream;
        }
    }

    public class FuncRecordVia<TTarget, TReturn, TArgs> : IFuncRecordVia<TTarget, TReturn, TArgs> where TTarget : class?
    {
        public IVia Via { get; }
        public IFuncCallStream<TTarget, TReturn, TArgs> CallStream { get; }

        public FuncRecordVia(IVia via, IFuncCallStream<TTarget, TReturn, TArgs> callStream)
        {
            Via = via;
            CallStream = callStream;
        }
    }
    
    public class FuncRecordVia<TReturn> : IFuncRecordVia<TReturn>
    {
        public IVia Via { get; }
        public IFuncCallStream<TReturn> CallStream { get; }

        public FuncRecordVia(IVia via, IFuncCallStream<TReturn> callStream)
        {
            Via = via;
            CallStream = callStream;
        }
    }
}