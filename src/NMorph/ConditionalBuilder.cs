using System;
using System.Linq.Expressions;

namespace NMorph
{
    internal class ConditionalBuilder<T, TReturn> : IConditionalBuilder<T, TReturn> where T : class
    {
        private readonly AlterationBuilder<T> _alterationBuilder;
        private readonly Expression<Func<T, TReturn>> _expression;

        public ConditionalBuilder(AlterationBuilder<T> alterationBuilder, Expression<Func<T, TReturn>> expression)
        {
            _alterationBuilder = alterationBuilder;
            _expression = expression;
        }
        
        public IAlterationBuilder<T> Retarget(T substitute)
        {
            throw new NotImplementedException();
        }

        public IAlterationBuilder<T> Return(TReturn value)
        {
            throw new NotImplementedException();
        }
    }
}