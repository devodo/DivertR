using System;

namespace DivertR
{
    public static class Is<T>
    {
        public static T Any => default!;
        public static T Return => default!;
        
        public static T Match(Func<T, bool> match)
        {
            return default!;
        }
    }
}