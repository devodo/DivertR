namespace DivertR.Internal
{
    internal class RedirectState<TTarget> where TTarget : class
    {
        public RedirectState(Redirect<TTarget>[] redirectItems, bool isStrict)
        {
            RedirectItems = redirectItems;
            IsStrict = isStrict;
        }

        public Redirect<TTarget>[] RedirectItems { get; }
        public bool IsStrict { get; }
    }
}