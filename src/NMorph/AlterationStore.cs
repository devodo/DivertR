using System;
using System.Collections.Concurrent;

namespace NMorph
{
    internal class AlterationStore
    {
        private readonly ConcurrentDictionary<MorphId, Alteration> _alterations = new ConcurrentDictionary<MorphId, Alteration>();
        private readonly ConcurrentDictionary<MorphId, object> _updateLocks = new ConcurrentDictionary<MorphId, object>();

        public Alteration<T> AddAlteration<T>(string groupName, Func<IInvocationContext<T>, T> getSubstitute) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Alteration Create(MorphId _)
            {
                var invocationStack = new InvocationStack<T>();
                var source = new InvocationContext<T>(invocationStack);
                var substitute = getSubstitute(source);
                return new Alteration<T>(new Substitution<T>(substitute), invocationStack);
            }

            Alteration Update(MorphId _, Alteration existingAlteration)
            {
                var alteration = (Alteration<T>) existingAlteration;
                var source = new InvocationContext<T>(alteration.InvocationStack);
                var substitute = getSubstitute(source);
                return alteration.Append(new Substitution<T>(substitute));
            }

            var morphGroup = MorphId.From<T>(groupName);
            var lockObject = _updateLocks.GetOrAdd(morphGroup, _ => new object());
            lock (lockObject)
            {
                // The lock is to ensure getSubstitute can only get called once
                return (Alteration<T>)_alterations.AddOrUpdate(morphGroup, Create, Update);
            }
        }

        public Alteration<T> GetAlteration<T>(string groupName) where T : class
        {
            if (_alterations.TryGetValue(MorphId.From<T>(groupName), out var alteration))
            {
                return (Alteration<T>)alteration;
            }

            return null;
        }

        public bool Reset<T>(string groupName = null)
        {
            return _alterations.TryRemove(MorphId.From<T>(groupName), out _);
        }
        
        public void Reset()
        {
            _alterations.Clear();
        }
    }
}