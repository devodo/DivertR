using System.Collections.Generic;
using System.Linq;

namespace NMorph
{
    internal class Diversion {}

    internal class Diversion<T> : Diversion where T : class
    {
        private readonly List<Substitution<T>> _substitutions;

        public CallContext<T> CallContext { get; }
        
        public Diversion(CallContext<T> callContext)
        {
            _substitutions = new List<Substitution<T>>();
            CallContext = callContext;
        }

        public Diversion(Substitution<T> substitution, CallContext<T> callContext)
        {
            _substitutions = new List<Substitution<T>> {substitution};
            CallContext = callContext;
        }

        private Diversion(List<Substitution<T>> substitutions, CallContext<T> callContext)
        {
            _substitutions = substitutions;
            CallContext = callContext;
        }

        public SubstitutionState<T> CreateSubstitutionState(T origin)
        {
            return _substitutions.Count == 0 ? null : new SubstitutionState<T>(origin, _substitutions);
        }

        public Diversion<T> Append(Substitution<T> substitution)
        {
            var substitutions = new[] {substitution}.Concat(_substitutions).ToList();
            return new Diversion<T>(substitutions, CallContext);
        }
        
        public Diversion<T> Reset()
        {
            return new Diversion<T>(CallContext);
        }
    }
}