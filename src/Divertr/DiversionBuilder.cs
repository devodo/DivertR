using System;
using System.Linq.Expressions;

namespace Divertr
{
    internal class DiversionBuilder<T> : IDiversionBuilder<T> where T : class
    {
        private readonly DiversionStore _diversionStore;
        private readonly DiversionId _diversionId;

        public DiversionBuilder(DiversionStore diversionStore, DiversionId diversionId)
        {
            _diversionStore = diversionStore;
            _diversionId = diversionId;
        }

        public IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new ConditionalBuilder<T, TReturn>(this, expression);
        }
    }
}