namespace DivertR.Redirects
{
    public static class ViaExtensions
    {
        public static ICallRecord<TTarget> RecordCalls<TTarget>(this IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null, int orderWeight = int.MaxValue)
            where TTarget : class
        {
            var callCapture = new CallRecordRedirect<TTarget>(via);
            via.InsertRedirect(callCapture, orderWeight);

            return callCapture;
        }
    }
}