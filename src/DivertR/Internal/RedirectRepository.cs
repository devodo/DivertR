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

        public IRedirectRepository InsertVia(IVia via, IViaOptions? viaOptions = null)
        {
            var configuredVia = new ConfiguredVia(via, viaOptions ?? ViaOptions.Default);
            
            return InsertVia(configuredVia);
        }

        public IRedirectRepository InsertVia(IConfiguredVia configuredVia)
        {
            lock (_lockObject)
            {
                if (configuredVia.Options.IsPersistent)
                {
                    _persistentPlan = _persistentPlan.InsertVia(configuredVia);
                }

                MutatePlan(original => original.InsertVia(configuredVia));
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