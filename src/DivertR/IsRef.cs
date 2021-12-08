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
    
    public class RefValue<T>
    {
        internal static readonly RefValue<T> Instance = new RefValue<T>();

        private RefValue() { }
        
        // ReSharper disable once InconsistentNaming (Used for call match Expression consistency)
        public T Value = default!;
    }
}