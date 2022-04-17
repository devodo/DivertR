using System;

namespace DivertR
{
    public interface IBaseRedirectOptions<TCallHandler, TCallConstraint, TRedirectCall, TCallInfo>
        where TCallHandler : IBaseCallHandler<TRedirectCall>
        where TCallConstraint : IBaseCallConstraint<TCallInfo>
        where TRedirectCall : IRedirectCall
        where TCallInfo : CallInfo
    {
        int? OrderWeight { get; }
        bool? DisableSatisfyStrict { get; }
        
        Func<TCallHandler, TCallHandler>? CallHandlerDecorator { get; }
        Func<TCallConstraint, TCallConstraint>? CallConstraintDecorator { get; }
    }
    
    public interface IRedirectOptions : IBaseRedirectOptions<ICallHandler, ICallConstraint, IRedirectCall, CallInfo>
    {
    }
    
    public interface IRedirectOptions<TTarget> : IBaseRedirectOptions<ICallHandler<TTarget>, ICallConstraint<TTarget>, IRedirectCall<TTarget>, CallInfo<TTarget>>
        where TTarget : class
    {
    }
}