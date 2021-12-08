using System;

namespace DivertR.Internal
{
    internal interface IValueTupleMapper
    {
        Type[] ArgumentTypes { get; }
        object ToTuple(Span<object> args);
        object?[] ToObjectArray(object boxedTuple);
        void WriteBackReferences(Span<object> args, object boxedTuple);
    }
}