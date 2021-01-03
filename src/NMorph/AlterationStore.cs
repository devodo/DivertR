using System;
using System.Collections.Concurrent;

namespace NMorph
{
    internal class AlterationStore
    {
        private readonly ConcurrentDictionary<MorphGroup, Alteration> _alterations = new ConcurrentDictionary<MorphGroup, Alteration>();
        private readonly ConcurrentDictionary<MorphGroup, object> _updateLocks = new ConcurrentDictionary<MorphGroup, object>();

        public Alteration<T> UpdateAlteration<T>(string groupName, Func<IMorphInvocation<T>, T> getSubstitute) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Alteration Create(MorphGroup _)
            {
                var invocationStack = new InvocationStack<T>();
                var source = new MorphInvocation<T>(invocationStack);
                var substitute = getSubstitute(source);
                return new Alteration<T>(substitute, invocationStack);
            }

            Alteration Update(MorphGroup _, Alteration existingAlteration)
            {
                var alteration = (Alteration<T>) existingAlteration;
                var source = new MorphInvocation<T>(alteration.InvocationStack, alteration.Substitute);
                var substitute = getSubstitute(source);
                return new Alteration<T>(substitute, alteration.InvocationStack);
            }

            var morphGroup = MorphGroup.From<T>(groupName);
            var lockObject = _updateLocks.GetOrAdd(morphGroup, _ => new object());
            lock (lockObject)
            {
                // The lock is to ensure getSubstitute can only get called once
                return (Alteration<T>)_alterations.AddOrUpdate(morphGroup, Create, Update);
            }
        }

        public Alteration<T> GetAlteration<T>(string groupName) where T : class
        {
            if (_alterations.TryGetValue(MorphGroup.From<T>(groupName), out var alteration))
            {
                return (Alteration<T>)alteration;
            }

            return null;
        }

        public void Reset()
        {
            _alterations.Clear();
        }

        public bool Reset<T>(string groupName = null)
        {
            return _alterations.TryRemove(MorphGroup.From<T>(groupName), out _);
        }
    }
}