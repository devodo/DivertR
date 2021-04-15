namespace DivertR.Redirects
{
    public static class RecordViaExtensions
    {
        public static ICallsRecord<TTarget> Record<TTarget>(this IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
            where TTarget : class
        {
            var recordRedirect = new CallsRecordRedirect<TTarget>(via);
            via.InsertRedirect(recordRedirect, int.MaxValue);

            return recordRedirect;
        }
    }
}