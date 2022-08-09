namespace DivertR
{
    public interface IFuncRelay<out TReturn> : IRelay
    {
        new TReturn CallNext();
        new TReturn CallNext(CallArguments args);
        new TReturn CallRoot();
        new TReturn CallRoot(CallArguments args);
    }
    
    public interface IFuncRelay<TTarget, out TReturn> : IRelay<TTarget> where TTarget : class?
    {
        new TReturn CallNext();
        new TReturn CallNext(CallArguments args);
        new TReturn CallRoot();
        new TReturn CallRoot(CallArguments args);
    }
}