using System.Collections.Generic;

namespace DivertR.Record.Internal
{
    internal static class ReadOnlyListExtensions
    {
        public static IReadOnlyList<TDerived> UnsafeCast<TDerived>(this IReadOnlyList<object> baseList)
        {
            return new CastedReadOnlyList<TDerived>(baseList);
        }
    }
}