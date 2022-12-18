namespace DivertR.Record.Internal
{
    internal class RecordVia<TTarget> : IRecordVia<TTarget> where TTarget : class?
    {
        public IVia Via { get; }
        public IRecordStream<TTarget> RecordStream { get; }

        public RecordVia(IVia via, IRecordStream<TTarget> recordStream)
        {
            Via = via;
            RecordStream = recordStream;
        }
    }
    
    internal class RecordVia : IRecordVia
    {
        public IVia Via { get; }
        public IRecordStream RecordStream { get; }

        public RecordVia(IVia via, IRecordStream recordStream)
        {
            Via = via;
            RecordStream = recordStream;
        }
    }
}