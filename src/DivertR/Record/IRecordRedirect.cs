namespace DivertR.Record
{
    public interface IRecordRedirect<TTarget> where TTarget : class?
    {
        IRedirect Redirect { get; }
        public IRecordStream<TTarget> RecordStream { get; }
    }
    
    public interface IRecordRedirect
    {
        IRedirect Redirect { get; }
        public IRecordStream RecordStream { get; }
    }
}