namespace DivertR.Record
{
    public interface IRecordVia<TTarget> where TTarget : class?
    {
        IVia Via { get; }
        public IRecordStream<TTarget> RecordStream { get; }
    }
    
    public interface IRecordVia
    {
        IVia Via { get; }
        public IRecordStream RecordStream { get; }
    }
}