using System;
using DivertR.Record;

namespace DivertR.Dummy
{
    public interface IDummyBuilder<TReturn>
    {
        IDummyBuilder<TReturn> AddConstraint(ICallConstraint callConstraint);
        IDummyBuilder<TReturn> Redirect(TReturn instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder<TReturn> Redirect(Func<TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder<TReturn> Redirect(Func<IFuncRedirectCall<TReturn>, CallArguments, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IFuncCallStream<TReturn> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
    
    public interface IDummyBuilder
    {
        IDummyBuilder AddConstraint(ICallConstraint callConstraint);
        IDummyBuilder Redirect(object instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder Redirect(Func<object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder Redirect(Func<IRedirectCall, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDummyBuilder Redirect(Func<IRedirectCall, CallArguments, object> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRecordStream Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}