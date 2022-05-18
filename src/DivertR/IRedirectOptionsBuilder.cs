using System;

namespace DivertR
{
    public interface IRedirectOptionsBuilderBase<out TBuilder>
    {
        TBuilder OrderWeight(int orderWeight);
        TBuilder OrderFirst();
        TBuilder OrderLast();
        TBuilder DisableSatisfyStrict(bool disableStrict = true);
        TBuilder Repeat(int repeatCount);
        TBuilder Skip(int skipCount);
        TBuilder AddSwitch(IRedirectSwitch redirectSwitch);
    }
    
    public interface IRedirectOptionsBuilder<TTarget> : IRedirectOptionsBuilderBase<IRedirectOptionsBuilder<TTarget>> where TTarget : class
    {
        IRedirectOptionsBuilder<TTarget> DecorateCallHandler(Func<ICallHandler<TTarget>, ICallHandler<TTarget>> decorator);
        IRedirectOptionsBuilder<TTarget> DecorateCallConstraint(Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>> decorator);
    }
    
    public interface IRedirectOptionsBuilder : IRedirectOptionsBuilderBase<IRedirectOptionsBuilder>
    {
        IRedirectOptionsBuilder DecorateCallHandler(Func<ICallHandler, ICallHandler> decorator);
        IRedirectOptionsBuilder DecorateCallConstraint(Func<ICallConstraint, ICallConstraint> decorator);
    }
}