using System.Collections.Generic;

namespace DivertR
{
    public interface IRedirectPlan
    {
        IReadOnlyList<Redirect> Redirects { get; }

        bool IsStrictMode { get; }
    }
}