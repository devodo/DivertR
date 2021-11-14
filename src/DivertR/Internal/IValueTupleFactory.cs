using System;

namespace DivertR.Internal
{
    internal interface IValueTupleFactory
    {
        Type[] ArgumentTypes { get; }
        object Create(CallArguments args);
        object Create(CallArguments args, int offset);
    }
}