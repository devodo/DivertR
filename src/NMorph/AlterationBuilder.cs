using System;
using System.Linq.Expressions;

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
        
        public IAlterationBuilder<T> Retarget(T substitute)
        {
            _alterationStore.AddAlteration(_groupName, substitute);

            return this;
        }
        
        public IAlterationBuilder<T> Reset()
        {
            _alterationStore.Reset<T>(_groupName);

            return this;
        }

        public IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new ConditionalBuilder<T, TReturn>(this, expression);
        }
    }
}