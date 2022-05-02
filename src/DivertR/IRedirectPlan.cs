using System.Collections.Generic;

namespace DivertR
{
    public interface IRedirectPlan<TTarget> where TTarget : class
    {
        IReadOnlyList<IRedirect<TTarget>> Redirects { get; }

        bool IsStrictMode { get; }
    }
}