using System;
using System.Collections.Concurrent;

namespace NMorph
{
    internal class AlterationStore
    {
        private readonly ConcurrentDictionary<MorphGroup, Alteration> _alterations = new ConcurrentDictionary<MorphGroup, Alteration>();

        public Alteration<T> UpdateAlteration<T>(string groupName, Func<IMorphSource<T>, T> getSubstitute, bool chain) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException("Only interface types are supported", typeof(T).Name);
            }

            Alteration Create(MorphGroup morphGroup)
            {
                var invocationStack = new InvocationStack<T>();
                var source = new MorphSource<T>(invocationStack);
                var substitute = getSubstitute(source);
                return new Alteration<T>(substitute, invocationStack);
            }

            Alteration Update(MorphGroup morphGroup, Alteration existingAlteration)
            {
                if (!chain)
                {
                    return Create(morphGroup);
                }

                var alteration = (Alteration<T>) existingAlteration;
                var source = new MorphSource<T>(alteration.InvocationStack, alteration.Substitute);
                var substitute = getSubstitute(source);
                return new Alteration<T>(substitute, alteration.InvocationStack);
            }

            return (Alteration<T>)_alterations.AddOrUpdate(MorphGroup.From<T>(groupName), Create, Update);
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