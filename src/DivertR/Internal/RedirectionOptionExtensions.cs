using System;

namespace DivertR.Internal
{
    internal static class RedirectionOptionExtensions
    {
        public static RedirectOptions Create(this Action<IRedirectOptionsBuilder>? optionsAction, IVia via)
        {
            var builder = new RedirectOptionsBuilder(via);
            optionsAction?.Invoke(builder);
            
            return builder.Build();
        }
    }
}