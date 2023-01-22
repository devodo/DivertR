using System;
using DivertR.Record;

namespace DivertR
{
    public interface ISpy : IRedirect
    {
        object Mock { get; }
        IRecordStream Calls { get; }
        
        new ISpy Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);
        new ISpy Reset(bool includePersistent = false);
        new ISpy Strict(bool? isStrict = true);
    }
    
    public interface ISpy<TTarget> : IRedirect<TTarget> where TTarget : class?
    {
        TTarget Mock { get; }
        IRecordStream<TTarget> Calls { get; }
        
        new ISpy<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);
        new ISpy<TTarget> Reset(bool includePersistent = false);
        new ISpy<TTarget> Strict(bool? isStrict = true);
        new ISpy<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}