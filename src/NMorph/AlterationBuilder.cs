using System;

namespace NMorph
{
    internal class AlterationBuilder<T> : IAlterationBuilder<T> where T : class
    {
        private readonly AlterationStore _alterationStore;
        private readonly string _groupName;

        public AlterationBuilder(AlterationStore alterationStore, string groupName)
        {
            _alterationStore = alterationStore;
            _groupName = groupName;
        }
        
        public IAlterationBuilder<T> Replace(T substitute)
        {
            _alterationStore.AddAlteration<T>(_groupName, _ => substitute);

            return this;
        }

        public IAlterationBuilder<T> Replace(Func<IInvocationContext<T>, T> getSubstitute)
        {
            _alterationStore.AddAlteration(_groupName, getSubstitute);

            return this;
        }
    }
}