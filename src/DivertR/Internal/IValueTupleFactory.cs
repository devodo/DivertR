using System;

namespace DivertR.Internal
{
    internal interface IValueTupleFactory
    {
        Type[] ArgumentTypes { get; }
        object Create(CallArguments args);
    }
}