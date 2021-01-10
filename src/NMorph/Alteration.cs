using System.Collections.Generic;
using System.Linq;

namespace NMorph
{
    internal class Alteration {}

    internal class Alteration<T> : Alteration where T : class
    {
        private readonly List<Substitution<T>> _substitutions;

        public CallContext<T> CallContext { get; }
        
        public Alteration(CallContext<T> callContext)
        {
            _substitutions = new List<Substitution<T>>();
            CallContext = callContext;
        }

        public Alteration(Substitution<T> substitution, CallContext<T> callContext)
        {
            _substitutions = new List<Substitution<T>> {substitution};
            CallContext = callContext;
        }

        private Alteration(List<Substitution<T>> substitutions, CallContext<T> callContext)
        {
            _substitutions = substitutions;
            CallContext = callContext;
        }

        public SubstitutionState<T> CreateSubstitutionState(T origin)
        {
            return _substitutions.Count == 0 ? null : new SubstitutionState<T>(origin, _substitutions);
        }

        public Alteration<T> Append(Substitution<T> substitution)
        {
            var substitutions = new[] {substitution}.Concat(_substitutions).ToList();
            return new Alteration<T>(substitutions, CallContext);
        }
    }
}