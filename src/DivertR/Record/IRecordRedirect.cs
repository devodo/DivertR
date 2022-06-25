namespace DivertR.Record
{
    public interface IRecordRedirect<TTarget> where TTarget : class
    {
        IRedirect<TTarget> Redirect { get; }
        public IRecordStream<TTarget> RecordStream { get; }
    }
}