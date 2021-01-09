using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace NMorph
{
    internal class Alteration {}

    internal class Alteration<T> : Alteration where T : class
    {
        private readonly List<Substitution<T>> _substitutions;

        public InvocationStack<T> InvocationStack { get; }

        public Alteration(Substitution<T> substitution, InvocationStack<T> invocationStack)
        {
            _substitutions = new List<Substitution<T>> { substitution };
            InvocationStack = invocationStack;
        }

        private Alteration(List<Substitution<T>> substitutions, InvocationStack<T> invocationStack)
        {
            _substitutions = substitutions;
            InvocationStack = invocationStack;
        }

        public SubstitutionState<T> CreateSubstitutionState(T origin)
        {
            return new SubstitutionState<T>(origin, _substitutions);
        }

        public Alteration<T> Append(Substitution<T> substitution)
        {
            var substitutions = new[] {substitution}.Concat(_substitutions).ToList();
            return new Alteration<T>(substitutions, InvocationStack);
        }
    }
}