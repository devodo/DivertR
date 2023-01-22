using System;
using DivertR.Record;

namespace DivertR.Dummy
{
    public interface IDummyBuilder<TReturn>
    {
        IDummyBuilder<TReturn> Filter(ICallConstraint callConstraint);
        IDummyBuilder<TReturn> Via(TReturn instance, Action<IViaOptionsBuilder>? optionsAction = null);
        IDummyBuilder<TReturn> Via(Func<TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IDummyBuilder<TReturn> Via(Func<IFuncRedirectCall<TReturn>, TReturn> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IFuncCallStream<TReturn> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
    
    public interface IDummyBuilder
    {
        IDummyBuilder Filter(ICallConstraint callConstraint);
        IDummyBuilder Via(Func<object> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IDummyBuilder Via(Func<IRedirectCall, object> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRecordStream Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}