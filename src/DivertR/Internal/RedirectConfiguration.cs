namespace DivertR.Internal
{
    internal class RedirectConfiguration<TTarget> where TTarget : class
    {
        public RedirectConfiguration(Redirect<TTarget>[] redirects, bool isStrictMode)
        {
            Redirects = redirects;
            IsStrictMode = isStrictMode;
        }

        public Redirect<TTarget>[] Redirects { get; }
        public bool IsStrictMode { get; }
    }
}