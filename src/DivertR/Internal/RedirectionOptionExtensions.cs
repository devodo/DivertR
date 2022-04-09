using System;

namespace DivertR.Internal
{
    internal static class RedirectionOptionExtensions
    {
        public static RedirectOptions Create(this Action<IRedirectOptionsBuilder>? optionsAction)
        {
            var builder = new RedirectOptionsBuilder();
            optionsAction?.Invoke(builder);
            
            return builder.Build();
        }
    }
}