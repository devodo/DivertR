using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectTracker
    {
        private readonly ConditionalWeakTable<object, IRedirect> _redirectTable = new();

        public void AddRedirect<TTarget>(Redirect<TTarget> redirect, [DisallowNull] TTarget proxy) where TTarget : class?
        {
            _redirectTable.Add(proxy, redirect);
        }

        public Redirect<TTarget> GetRedirect<TTarget>([DisallowNull] TTarget proxy) where TTarget : class?
        {
            if (!_redirectTable.TryGetValue(proxy, out var redirect))
            {
                throw new DiverterException("Redirect not found");
            }

            if (redirect is not Redirect<TTarget> redirectOf)
            {
                throw new DiverterException($"Redirect target type: {redirect.RedirectId.Type} does not match proxy type: {typeof(TTarget)}");
            }

            return redirectOf;
        }
    }
}