using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository : IRedirectRepository
    {
        private readonly ConcurrentStack<RedirectPlan> _redirectPlans = new ConcurrentStack<RedirectPlan>();
        private readonly object _lockObject = new object();

        public RedirectRepository()
        {
            _redirectPlans.Push(Internal.RedirectPlan.Empty);
        }
        
        public RedirectRepository(IEnumerable<IRedirect> redirects)
        {
            var redirectPlan = Internal.RedirectPlan.Empty.InsertRedirects(redirects);
            _redirectPlans.Push(redirectPlan);
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

        public void InsertRedirect(IRedirect redirect)
        {
            MutateRedirectPlan(original => original.InsertRedirect(redirect));
        }
        
        public void InsertRedirects(IEnumerable<IRedirect> redirects)
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
                _redirectPlans.Push(Internal.RedirectPlan.Empty);
            }
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