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
        
        // ReSharper disable once InconsistentNaming
        public T Value = default!;
    }
}