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
        IRedirectOptionsBuilder<TTarget> AddCallConstraint(ICallConstraint<TTarget> callConstraint);
        
        IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount);
        IRedirectOptionsBuilder<TTarget> Skip(int skipCount);
        IRedirectOptionsBuilder<TTarget> AddSwitch(IRedirectSwitch redirectSwitch);
    }
    
    public interface IRedirectOptionsBuilder
    {
        IRedirectOptionsBuilder OrderWeight(int orderWeight);
        IRedirectOptionsBuilder OrderFirst();
        IRedirectOptionsBuilder OrderLast();
        IRedirectOptionsBuilder DisableSatisfyStrict(bool disableStrict = true);
    }
}