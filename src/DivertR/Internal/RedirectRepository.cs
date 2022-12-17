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

        public IRedirectRepository InsertVia(IVia redirect, IViaOptions? redirectOptions = null)
        {
            var container = new ViaContainer(redirect, redirectOptions ?? ViaOptions.Default);
            
            return InsertVia(container);
        }

        public IRedirectRepository InsertVia(IViaContainer redirect)
        {
            lock (_lockObject)
            {
                if (redirect.Options.IsPersistent)
                {
                    _persistentPlan = _persistentPlan.InsertVia(redirect);
                }

                MutatePlan(original => original.InsertVia(redirect));
            }

            return this;
        }

        public IRedirectRepository SetStrictMode(bool isStrict = true)
        {
            lock (_lockObject)
            {
                MutatePlan(original => original.SetStrictMode(isStrict));
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

        private void MutatePlan(Func<RedirectPlan, RedirectPlan> mutateAction)
        {
            _planStack.TryPeek(out var redirectPlan);
            var mutated = mutateAction(redirectPlan);
            _planStack.Push(mutated);
        }
    }
}