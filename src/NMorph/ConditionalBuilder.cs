using System;
using System.Linq.Expressions;

namespace NMorph
{
    internal class ConditionalBuilder<T, TReturn> : IConditionalBuilder<T, TReturn> where T : class
    {
        private readonly DiversionBuilder<T> _diversionBuilder;
        private readonly Expression<Func<T, TReturn>> _expression;

        public ConditionalBuilder(DiversionBuilder<T> diversionBuilder, Expression<Func<T, TReturn>> expression)
        {
            _diversionBuilder = diversionBuilder;
            _expression = expression;
        }
        
        public IDiversionBuilder<T> SendTo(T substitute)
        {
            throw new NotImplementedException();
        }

        public IDiversionBuilder<T> Return(TReturn value)
        {
            throw new NotImplementedException();
        }
    }
}