using System;

namespace DivertR
{
    public static class Is<T>
    {
        public static T Any => default!;

        public static T Match(Func<T, bool> match)
        {
            return default!;
        }
    }

    public class RefValue<T>
    {
        internal static readonly RefValue<T> Instance = new RefValue<T>();

        private RefValue() { }
        
        // ReSharper disable once InconsistentNaming (Used for call match Expression consistency)
        public T Value = default!;
    }
}