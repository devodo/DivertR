using System;

namespace DivertR
{
    public static class Is<T>
    {
        public static T Any => default!;

        public static T AnyRef = default!;

        public static T Match(Func<T, bool> match)
        {
            return default!;
        }
    }
}