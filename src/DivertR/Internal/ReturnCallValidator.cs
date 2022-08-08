using System;

namespace DivertR.Internal
{
    internal class ReturnCallValidator : ICallValidator
    {
        private readonly Type _returnType;
        private readonly bool _matchSubType;

        public ReturnCallValidator(Type returnType, bool matchSubType = false)
        {
            _returnType = returnType;
            _matchSubType = matchSubType;
        }
        
        public void Validate(IValueTupleMapper valueTupleMapper)
        {
            throw new DiverterValidationException("ValueTuple arguments invalid for return type matched call");
        }

        public ICallConstraint CreateCallConstraint()
        {
            return new ReturnCallConstraint(_returnType, _matchSubType);
        }
    }
}