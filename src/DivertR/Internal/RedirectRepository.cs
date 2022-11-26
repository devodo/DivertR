using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository : IRedirectRepository
    {
        private readonly ConcurrentStack<RedirectPlan> _planStack = new();
        private RedirectPlan _persistentPlan = Internal.RedirectPlan.Empty;
        private readonly object _lockObject = new();

        public RedirectRepository()
        {
            _planStack.Push(_persistentPlan);
        }

        public IRedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // ReSharper disable once InconsistentlySynchronizedField: lock free read optimisation
                if (!_planStack.TryPeek(out var redirectPlan))
                {
                    lock (_lockObject)
                    {
                        redirectPlan = _persistentPlan;
                    }
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
            lock (_lockObject)
            {
                if (redirect.Options.IsPersistent)
                {
                    _persistentPlan = _persistentPlan.InsertRedirect(redirect);
                }

                MutateRedirectPlan(original => original.InsertRedirect(redirect));
            }

            return this;
        }

        public IRedirectRepository SetStrictMode(bool isStrict = true)
        {
            lock (_lockObject)
            {
                MutateRedirectPlan(original => original.SetStrictMode(isStrict));
            }

            return this;
        }

        public IRedirectRepository Reset(bool includePersistent = false)
        {
            lock (_lockObject)
            {
                if (includePersistent)
                {
                    _persistentPlan = Internal.RedirectPlan.Empty;
                }
                
                _planStack.Clear();
                _planStack.Push(_persistentPlan);
            }

            return this;
        }

        private void MutateRedirectPlan(Func<RedirectPlan, RedirectPlan> mutateAction)
        {
            _planStack.TryPeek(out var redirectPlan);
            var mutated = mutateAction(redirectPlan);
            _planStack.Push(mutated);
        }
    }
}