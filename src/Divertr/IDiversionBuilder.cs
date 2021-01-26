using System;
using System.Linq.Expressions;

namespace Divertr
{
    public interface IDiversionBuilder<T> where T : class
    {
        IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression);
    }
}