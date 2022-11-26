using System;

namespace DivertR
{
    public interface IRedirectOptions
    {
        int OrderWeight { get; }
        bool DisableSatisfyStrict { get; }  
        bool IsPersistent { get; }
        Func<IRedirect, IRedirect>? RedirectDecorator { get; }
    }
}