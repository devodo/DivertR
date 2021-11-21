﻿using System;
using System.Collections.Generic;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        Redirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IReadOnlyCollection<TMap> Spy<TMap>(Func<IRecordedCall<TTarget>, TMap> mapper, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
