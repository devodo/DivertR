using System;

namespace DivertR
{
    public interface IRedirectOptions
    {
        int OrderWeight { get; }
        bool DisableSatisfyStrict { get; }
        Func<IRedirect, IRedirect>? RedirectDecorator { get; }
    }
}