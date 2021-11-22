using System;

namespace DivertR
{
    public static class IsRef<T>
    {
        public static T Any = default!;
        
        public static RefValue<T> Match(Func<T, bool> match)
        {
            return RefValue<T>.Instance;
        }
    }
}