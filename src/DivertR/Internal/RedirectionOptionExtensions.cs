using System;

namespace DivertR.Internal
{
    internal static class RedirectionOptionExtensions
    {
        public static RedirectOptions<TTarget> Create<TTarget>(this Action<IRedirectOptionsBuilder<TTarget>>? optionsAction, IVia<TTarget> via) where TTarget : class
        {
            var builder = new RedirectOptionsBuilder<TTarget>(via);
            optionsAction?.Invoke(builder);
            
            return builder.Build();
        }
    }
}