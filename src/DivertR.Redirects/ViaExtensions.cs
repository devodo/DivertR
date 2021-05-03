namespace DivertR.Redirects
{
    public static class ViaExtensions
    {
        public static ICallRecord<TTarget> Record<TTarget>(this IVia<TTarget> via, ICallConstraint<TTarget>? callConstraint = null)
            where TTarget : class
        {
            var recorderRedirect = new CallRecorderRedirect<TTarget>(via);
            via.InsertRedirect(recorderRedirect, int.MaxValue);

            return recorderRedirect;
        }
    }
}
