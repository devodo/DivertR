using System.Collections.Generic;

namespace DivertR
{
    public interface IRedirectPlan<out TRedirect> where TRedirect : IRedirect
    {
        IReadOnlyList<TRedirect> Redirects { get; }

        bool IsStrictMode { get; }
    }
}