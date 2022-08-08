namespace DivertR.Internal
{
    internal interface ICallValidator
    {
        void Validate(IValueTupleMapper valueTupleMapper);
        ICallConstraint CreateCallConstraint();
    }
}