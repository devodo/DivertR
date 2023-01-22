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
                        if (!_planStack.TryPeek(out redirectPlan))
                        {
                            // This state should never be possible
                            throw new InvalidOperationException("Unexpected empty redirect plan stack encountered.");
                        }
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
            return ResetInternal(null, includePersistent);
        }

        public IRedirectRepository Reset(IVia via, bool includePersistent = false)
        {
            return Reset(via, ViaOptions.Default, includePersistent);
        }
        
        public IRedirectRepository Reset(IVia via, IViaOptions viaOptions, bool includePersistent = false)
        {
            var configuredVia = new ConfiguredVia(via, viaOptions);
            
            return ResetInternal(configuredVia, includePersistent);
        }
        
        public IRedirectRepository Reset(IConfiguredVia configuredVia, bool includePersistent = false)
        {
            return ResetInternal(configuredVia, includePersistent);
        }

        private IRedirectRepository ResetInternal(IConfiguredVia? configuredVia, bool includePersistent = false)
        {
            lock (_lockObject)
            {
                if (includePersistent)
                {
                    _persistentPlan = Internal.RedirectPlan.Empty;
                }

                var resetPlan = configuredVia == null ? _persistentPlan : _persistentPlan.InsertVia(configuredVia);
                
                _planStack.Clear();
                _planStack.Push(resetPlan);
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