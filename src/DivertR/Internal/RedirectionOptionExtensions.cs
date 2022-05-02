using System;

namespace DivertR.Internal
{
    internal static class RedirectionOptionExtensions
    {
        public static IRedirectOptions<TTarget> Create<TTarget>(this Action<IRedirectOptionsBuilder<TTarget>>? optionsAction) where TTarget : class
        {
            var builder = new RedirectOptionsBuilder<TTarget>();
            optionsAction?.Invoke(builder);
            
            return builder.Build();
        }
    }
}