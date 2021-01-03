using System;

namespace NMorph
{
    public interface IMutator<T> where T : class
    {
        void Substitute(T substitution);
        void Substitute(string groupName, T substitution);
        void Substitute(Func<T, T> substituteFactory);
        void Substitute(string groupName, Func<T, T> substituteFactory);
        void AppendSubstitute(Func<T, T> substituteFactory);
        void AppendSubstitute(string groupName, Func<T, T> substituteFactory);
        bool Reset(string groupName = null);
    }
}