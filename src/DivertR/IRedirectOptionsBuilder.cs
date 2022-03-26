using System;

namespace DivertR
{
    public interface IRedirectOptionsBuilder
    {
        IRedirectOptionsBuilder OrderWeight(int orderWeight);
        IRedirectOptionsBuilder OrderFirst();
        IRedirectOptionsBuilder OrderLast();
        IRedirectOptionsBuilder DisableSatisfyStrict(bool disableStrict = true);
        IRedirectOptionsBuilder DecorateCallHandler(Func<ICallHandler, ICallHandler> decorator);
        IRedirectOptionsBuilder DecorateCallConstraint(Func<ICallConstraint, ICallConstraint> decorator);
        IRedirectOptionsBuilder Repeat(int repeatCount);
        IRedirectOptionsBuilder Skip(int skipCount);
        IRedirectOptionsBuilder AddSwitch(IRedirectSwitch redirectSwitch);
    }
}