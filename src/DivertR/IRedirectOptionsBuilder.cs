using System;

namespace DivertR
{
    public interface IRedirectOptionsBuilder
    {
        IRedirectOptionsBuilder OrderWeight(int orderWeight);
        IRedirectOptionsBuilder OrderFirst();
        IRedirectOptionsBuilder OrderLast();
        IRedirectOptionsBuilder DisableSatisfyStrict(bool disableStrict = true);
        
        IRedirectOptionsBuilder Decorate(Func<IRedirect, IRedirect> decorator);
        IRedirectOptionsBuilder Repeat(int repeatCount);
        IRedirectOptionsBuilder Skip(int skipCount);
    }
}