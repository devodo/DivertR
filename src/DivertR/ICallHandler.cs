namespace DivertR
{
    public interface IBaseCallHandler<in TRedirectCall>
        where TRedirectCall : IRedirectCall
    {
        object? Handle(TRedirectCall call);
    }
    
    public interface ICallHandler : IBaseCallHandler<IRedirectCall>
    {
    }
    
    public interface ICallHandler<TTarget> : IBaseCallHandler<IRedirectCall<TTarget>>
        where TTarget : class
    {
    }
}