namespace DivertR.Record.Internal
{
    public class RecordRedirect<TTarget> : IRecordRedirect<TTarget> where TTarget : class
    {
        public IRedirect<TTarget> Redirect { get; }
        public IRecordStream<TTarget> RecordStream { get; }

        public RecordRedirect(IRedirect<TTarget> redirect, IRecordStream<TTarget> recordStream)
        {
            Redirect = redirect;
            RecordStream = recordStream;
        }
    }
}