using System.Collections.Generic;

namespace DivertR
{
    public interface IRedirectPlan
    {
        IReadOnlyList<IRedirect> Redirects { get; }

        bool IsStrictMode { get; }
    }
}