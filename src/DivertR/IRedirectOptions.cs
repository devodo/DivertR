using System;

namespace DivertR
{
    public interface IRedirectOptions<TTarget> where TTarget : class
    {
        int? OrderWeight { get; }
        bool? DisableSatisfyStrict { get; }
        Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; }
        Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; }
    }
    
    public interface IRedirectOptions
    {
        int? OrderWeight { get; }
        bool? DisableSatisfyStrict { get; }
        Func<ICallHandler, ICallHandler>? CallHandlerDecorator { get; }
        Func<ICallConstraint, ICallConstraint>? CallConstraintDecorator { get; }
    }
}