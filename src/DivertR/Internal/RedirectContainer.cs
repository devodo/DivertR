using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectContainer : IRedirectContainer
    {
        public RedirectContainer(IRedirect redirect, IRedirectOptions redirectOptions)
        {
            Redirect = redirectOptions.RedirectDecorator?.Invoke(redirect) ?? redirect;
            Options = redirectOptions;
        }
        
        public IRedirect Redirect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public IRedirectOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}