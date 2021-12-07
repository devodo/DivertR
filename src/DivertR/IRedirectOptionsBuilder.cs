using System;

namespace DivertR
{
    public interface IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        IRedirectOptionsBuilder<TTarget> OrderWeight(int orderWeight);
        IRedirectOptionsBuilder<TTarget> OrderFirst();
        IRedirectOptionsBuilder<TTarget> OrderLast();
        IRedirectOptionsBuilder<TTarget> DisableSatisfyStrict(bool disableStrict = true);
        IRedirectOptionsBuilder<TTarget> DecorateCallHandler(Func<ICallHandler<TTarget>, ICallHandler<TTarget>> decorator);
        IRedirectOptionsBuilder<TTarget> DecorateCallConstraint(Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>> decorator);
        IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount);
        IRedirectOptionsBuilder<TTarget> Skip(int skipCount);
        IRedirectOptionsBuilder<TTarget> AddSwitch(IRedirectSwitch redirectSwitch);
    }
}