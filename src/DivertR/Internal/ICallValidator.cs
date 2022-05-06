using System;

namespace DivertR.Internal
{
    internal interface ICallValidator
    {
        void Validate(IValueTupleMapper valueTupleMapper);
        void Validate(Delegate redirectDelegate);
    }
}