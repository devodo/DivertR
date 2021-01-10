using System;
using System.Linq.Expressions;

namespace NMorph
{
    public interface IAlterationBuilder<T> where T : class
    {
        IAlterationBuilder<T> Retarget(T substitute);
        IAlterationBuilder<T> Reset();
        
        IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression);
    }
}