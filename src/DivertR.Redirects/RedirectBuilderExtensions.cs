namespace DivertR.Redirects
{
    public static class RedirectBuilderExtensions
    {
        public static IRedirectBuilder<TTarget> Repeat<TTarget>(this IRedirectBuilder<TTarget> redirectBuilder, int repeatCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new RepeatRedirect<TTarget>(via, redirect, repeatCount));
        }
        
        public static IFuncRedirectBuilder<TTarget, TReturn> Repeat<TTarget, TReturn>(this IFuncRedirectBuilder<TTarget, TReturn> redirectBuilder, int repeatCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new RepeatRedirect<TTarget>(via, redirect, repeatCount));
        }
        
        public static IActionRedirectBuilder<TTarget> Repeat<TTarget>(this IActionRedirectBuilder<TTarget> redirectBuilder, int repeatCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new RepeatRedirect<TTarget>(via, redirect, repeatCount));
        }
        
        public static IRedirectBuilder<TTarget> Skip<TTarget>(this IRedirectBuilder<TTarget> redirectBuilder, int skipCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new SkipRedirect<TTarget>(via, redirect, skipCount));
        }
        
        public static IFuncRedirectBuilder<TTarget, TReturn> Skip<TTarget, TReturn>(this IFuncRedirectBuilder<TTarget, TReturn> redirectBuilder, int skipCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new SkipRedirect<TTarget>(via, redirect, skipCount));
        }
        
        public static IActionRedirectBuilder<TTarget> Skip<TTarget>(this IActionRedirectBuilder<TTarget> redirectBuilder, int skipCount)
            where TTarget : class
        {
            return redirectBuilder.Chain((via, redirect) => new SkipRedirect<TTarget>(via, redirect, skipCount));
        }
    }
}
