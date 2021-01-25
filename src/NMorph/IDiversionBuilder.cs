using System;
using System.Linq.Expressions;

namespace NMorph
{
    public interface IDiversionBuilder<T> where T : class
    {
        IDiversionBuilder<T> SendTo(T substitute);
        IDiversionBuilder<T> Reset();
        
        IConditionalBuilder<T, TReturn> When<TReturn>(Expression<Func<T, TReturn>> expression);
        ICallContext<T> CallContext { get; }
    }
}