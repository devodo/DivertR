using System;
using System.Linq.Expressions;

namespace NMorph
{
    internal class DiversionBuilder<T> : IDiversionBuilder<T> where T : class
    {
        private readonly DiversionStore _diversionStore;
        private readonly string _groupName;

        public DiversionBuilder(DiversionStore diversionStore, string groupName)
        {
            _diversionStore = diversionStore;
            _groupName = groupName;
        }
        
        public IDiversionBuilder<T> SendTo(T substitute)
        {
            _diversionStore.AddRedirect(_groupName, substitute);

            return this;
        }
        
        public IDiversionBuilder<T> Reset()
        {
            _diversionStore.Reset<T>(_groupName);

            return this;
        }

        public ICallContext<T> CallContext => _diversionStore.GetOrAddAlteration<T>(_groupName).CallContext;

        public IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new ConditionalBuilder<T, TReturn>(this, expression);
        }
    }
}