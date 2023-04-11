using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class RedirectRepository : IRedirectRepository
    {
        private readonly ConcurrentStack<RedirectPlan> _planStack = new();
        private RedirectPlan _persistentPlan;
        private readonly object _lockObject = new();

        public RedirectRepository() : this(Internal.RedirectPlan.Empty)
        {
        }
        
        public RedirectRepository(RedirectPlan initialPlan)
        {
            _persistentPlan = initialPlan;
            _planStack.Push(_persistentPlan);
        }

        public IRedirectPlan RedirectPlan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_planStack.TryPeek(out var redirectPlan))
                {
                    return redirectPlan;
                }

                lock (_lockObject)
                {
                    if (!_planStack.TryPeek(out redirectPlan))
                    {
                        // This state should never be possible
                        throw new InvalidOperationException("Unexpected empty redirect plan stack encountered.");
                    }
                }

                return redirectPlan;
            } 
        }

        public IRedirectRepository InsertVia(IVia via, ViaOptions? viaOptions = null)
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
        
        public IRedirectRepository InsertVias(IEnumerable<IVia> vias, ViaOptions? viaOptions = null)
        {
            var configuredVias = vias.Select(via => new ConfiguredVia(via, viaOptions ?? ViaOptions.Default));
            
            return InsertVias(configuredVias);
        }

        public IRedirectRepository InsertVias(IEnumerable<IConfiguredVia> configuredVias)
        {
            var viaArray = configuredVias as IConfiguredVia[] ?? configuredVias.ToArray();
            
            lock (_lockObject)
            {
                foreach (var configuredVia in viaArray)
                {
                    if (configuredVia.Options.IsPersistent)
                    {
                        _persistentPlan = _persistentPlan.InsertVia(configuredVia);
                    }
                }
                

                MutatePlan(original => original.InsertVias(viaArray));
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
        
        public IRedirectRepository Reset()
        {
            lock (_lockObject)
            {
                _planStack.Clear();
                _planStack.Push(_persistentPlan);
            }

            return this;
        }

        public IRedirectRepository ResetAndInsert(IVia via)
        {
            return ResetAndInsert(via, ViaOptions.Default);
        }
        
        public IRedirectRepository ResetAndInsert(IVia via, ViaOptions viaOptions)
        {
            var configuredVia = new ConfiguredVia(via, viaOptions);
            
            return ResetAndInsert(configuredVia);
        }
        
        public IRedirectRepository ResetAndInsert(IConfiguredVia configuredVia)
        {
            lock (_lockObject)
            {
                var resetPlan = _persistentPlan.InsertVia(configuredVia);
                
                if (configuredVia.Options.IsPersistent)
                {
                    _persistentPlan = resetPlan;
                }
                
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