namespace DivertR
{
    public interface IRedirect
    {
        int OrderWeight { get; }
        bool DisableSatisfyStrict { get; }
    }

    public interface IRedirect<in TCallInfo> : IRedirect
        where TCallInfo : CallInfo
    {
        bool IsMatch(TCallInfo callInfo);
    }
    
    public interface IRedirect<in TCallInfo, in TRedirectCall> : IRedirect<TCallInfo>
        where TCallInfo : CallInfo
        where TRedirectCall : IRedirectCall
    {
        object? Handle(TRedirectCall redirectCall);
    }
}