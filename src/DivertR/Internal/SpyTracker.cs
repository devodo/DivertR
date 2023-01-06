using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class SpyTracker
    {
        private readonly ConditionalWeakTable<object, ISpy> _spyTable = new();

        public void AddSpy<TTarget>(Spy<TTarget> spy, [DisallowNull] TTarget mock) where TTarget : class?
        {
            _spyTable.Add(mock, spy);
        }

        public Spy<TTarget> GetSpy<TTarget>([DisallowNull] TTarget mock) where TTarget : class?
        {
            if (!_spyTable.TryGetValue(mock, out var spy))
            {
                throw new DiverterException("Spy not found");
            }

            if (spy is not Spy<TTarget> spyOf)
            {
                throw new DiverterException($"Spy target type: {spy.RedirectId.Type} does not match mock type: {typeof(TTarget)}");
            }

            return spyOf;
        }
    }
}