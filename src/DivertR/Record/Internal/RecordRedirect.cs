namespace DivertR.Record.Internal
{
    internal class RecordRedirect<TTarget> : IRecordRedirect<TTarget> where TTarget : class?
    {
        public IRedirect Redirect { get; }
        public IRecordStream<TTarget> RecordStream { get; }

        public RecordRedirect(IRedirect redirect, IRecordStream<TTarget> recordStream)
        {
            Redirect = redirect;
            RecordStream = recordStream;
        }
    }
    
    internal class RecordRedirect : IRecordRedirect
    {
        public IRedirect Redirect { get; }
        public IRecordStream RecordStream { get; }

        public RecordRedirect(IRedirect redirect, IRecordStream recordStream)
        {
            Redirect = redirect;
            RecordStream = recordStream;
        }
    }
}