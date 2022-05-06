using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository<TTarget> where TTarget : class
    {
        private readonly ConcurrentStack<RedirectPlan<TTarget>> _redirectPlans = new ConcurrentStack<RedirectPlan<TTarget>>();
        private readonly object _lockObject = new object();

        public RedirectPlan<TTarget> RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            // ReSharper disable once InconsistentlySynchronizedField
            get => _redirectPlans.TryPeek(out var redirectPlan) ? redirectPlan : RedirectPlan<TTarget>.Empty;
        }

        public void InsertRedirect(IRedirect<TTarget> redirect)
        {
            MutateRedirectPlan(original => original.InsertRedirect(redirect));
        }
        
        public void InsertRedirects(IEnumerable<IRedirect<TTarget>> redirects)
        {
            MutateRedirectPlan(original => original.InsertRedirects(redirects));
        }
        
        public void SetStrictMode(bool isStrict = true)
        {
            MutateRedirectPlan(original => original.SetStrictMode(isStrict));
        }

        public void Reset()
        {
            lock (_lockObject)
            {
                _redirectPlans.Clear();
            }
        }

        private void MutateRedirectPlan(Func<RedirectPlan<TTarget>, RedirectPlan<TTarget>> mutateAction)
        {
            lock (_lockObject)
            {
                var current = RedirectPlan;
                var mutated = mutateAction(current);
                _redirectPlans.Push(mutated);
            }
        }
    }
}