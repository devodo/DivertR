using System;

namespace NMorph
{
    public class Morph<T> where T : class
    {
        private readonly AlterationStore _alterationStore;

        public Morph() : this(new AlterationStore())
        {
        }

        internal Morph(AlterationStore alterationStore)
        {
            _alterationStore = alterationStore;
        }
        
        public T CreateSubject(T origin = null)
        {
            return SubjectFactory.Instance.Create(_alterationStore, origin);
        }
        
        public void Substitute(T substitute)
        {
            _alterationStore.AddAlteration<T>(null, _ => substitute);
        }

        public void Substitute(Func<IInvocationContext<T>, T> getSubstitute)
        {
            _alterationStore.AddAlteration(null, getSubstitute);
        }
        
        public bool Reset()
        {
            return _alterationStore.Reset<T>();
        }
    }
}