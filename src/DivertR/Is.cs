using System;

namespace DivertR
{
    public static class Is<TTarget>
    {
        public static TTarget Any => default!;

        public static TTarget Match(Func<TTarget, bool> match)
        {
            return default!;
        }
    }
    
    public static class IsRef<TTarget>
    {
        public static TTarget Any = default!;
        
        public static RefValue<TTarget> Match(Func<TTarget, bool> match)
        {
            return RefValue<TTarget>.Instance;
        }
    }
    
    public class RefValue<TTarget>
    {
        internal static readonly RefValue<TTarget> Instance = new RefValue<TTarget>();
        private RefValue() { }
        
        public TTarget Value = default!;
    }
}