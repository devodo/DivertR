using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository : IRedirectRepository
    {
        private readonly ConcurrentStack<RedirectPlan> _redirectPlans = new();
        private readonly object _lockObject = new();

        public RedirectRepository()
        {
            _redirectPlans.Push(Internal.RedirectPlan.Empty);
        }

        public IRedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // ReSharper disable once InconsistentlySynchronizedField
                if (!_redirectPlans.TryPeek(out var redirectPlan))
                {
                    redirectPlan = Internal.RedirectPlan.Empty;
                }

                return redirectPlan;
            } 
        }

        public IRedirectRepository InsertRedirect(IRedirect redirect, IRedirectOptions? redirectOptions = null)
        {
            var container = new RedirectContainer(redirect, redirectOptions ?? RedirectOptions.Default);
            
            return InsertRedirect(container);
        }

        public IRedirectRepository InsertRedirect(IRedirectContainer redirect)
        {
            MutateRedirectPlan(original => original.InsertRedirect(redirect));

            return this;
        }

        public IRedirectRepository SetStrictMode(bool isStrict = true)
        {
            MutateRedirectPlan(original => original.SetStrictMode(isStrict));

            return this;
        }

        public IRedirectRepository Reset()
        {
            lock (_lockObject)
            {
                _redirectPlans.Clear();
                _redirectPlans.Push(Internal.RedirectPlan.Empty);
            }

            return this;
        }

        private void MutateRedirectPlan(Func<RedirectPlan, RedirectPlan> mutateAction)
        {
            lock (_lockObject)
            {
                _redirectPlans.TryPeek(out var redirectPlan);
                var mutated = mutateAction(redirectPlan);
                _redirectPlans.Push(mutated);
            }
        }
    }
}