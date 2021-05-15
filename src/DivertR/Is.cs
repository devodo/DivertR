using System;

namespace DivertR
{
    public static class Is<TTarget>
    {
        public static TTarget Any => default!;

        public static TTarget AnyRef = default!;

        public static TTarget Match(Func<TTarget, bool> match)
        {
            return default!;
        }
    }
}