using System;
using System.Collections.Concurrent;

namespace NMorph
{
    internal class DiversionStore
    {
        private readonly ConcurrentDictionary<DiversionId, Diversion> _diversions = new ConcurrentDictionary<DiversionId, Diversion>();

        public void AddRedirect<T>(string groupName, T substitute, IInvocationCondition<T> invocationCondition = null) where T : class
        {
            Diversion Create(DiversionId _)
            {
                var invocationStack = new InvocationStack<T>();
                var callContext = new CallContext<T>(invocationStack);
                return new Diversion<T>(new Substitution<T>(substitute, invocationCondition), callContext);
            }

            Diversion Update(DiversionId _, Diversion existingAlteration)
            {
                var alteration = (Diversion<T>) existingAlteration;
                return alteration.Append(new Substitution<T>(substitute, invocationCondition));
            }

            var morphGroup = DiversionId.From<T>(groupName);
            _diversions.AddOrUpdate(morphGroup, Create, Update);
        }

        public Diversion<T> GetDiversion<T>(string groupName) where T : class
        {
            if (_diversions.TryGetValue(DiversionId.From<T>(groupName), out var alteration))
            {
                return (Diversion<T>)alteration;
            }

            return null;
        }
        
        public Diversion<T> GetOrAddAlteration<T>(string groupName) where T : class
        {
            var alteration = _diversions.GetOrAdd(DiversionId.From<T>(groupName), id =>
            {
                var invocationStack = new InvocationStack<T>();
                var callContext = new CallContext<T>(invocationStack);
                return new Diversion<T>(callContext);
            });

            return (Diversion<T>) alteration;
        }

        public bool Reset<T>(string groupName = null)
        {
            return _diversions.TryRemove(DiversionId.From<T>(groupName), out _);
        }
        
        public void Reset()
        {
            _diversions.Clear();
        }
    }
}