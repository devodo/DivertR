using System;

namespace NMorph
{
    public interface IAlterationBuilder<T> where T : class
    {
        IAlterationBuilder<T> Replace(T substitute);
        IAlterationBuilder<T> Replace(Func<IInvocationContext<T>, T> getSubstitute);
    }
}