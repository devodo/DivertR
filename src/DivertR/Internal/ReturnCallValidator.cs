using System;

namespace DivertR.Internal
{
    internal class ReturnCallValidator : ICallValidator
    {
        private readonly Type _returnType;
        private readonly bool _matchSubType;

        public ReturnCallValidator(Type returnType, bool matchSubType)
        {
            _returnType = returnType;
            _matchSubType = matchSubType;
        }
        
        public void Validate(IValueTupleMapper valueTupleMapper)
        {
            throw new DiverterValidationException("ValueTuple arguments invalid for return type matched call");
        }

        public void Validate(Delegate redirectDelegate)
        {
            var returnType = redirectDelegate.Method.ReturnType;
            
            if (!ReturnTypeValid(returnType))
            {
                var redirectReturnName = _returnType.Name;
                var returnTypeName = redirectDelegate.Method.ReturnType.Name;

                if (returnTypeName == redirectReturnName)
                {
                    returnTypeName = redirectDelegate.Method.ReturnType.FullName;
                    redirectReturnName = _returnType.FullName;
                }
                
                throw new DiverterValidationException($"Delegate return type ({returnTypeName}) invalid for redirect call returning type ({redirectReturnName})");
            }
        }

        public ICallConstraint<TTarget> CreateCallConstraint<TTarget>() where TTarget : class
        {
            return new ReturnCallConstraint<TTarget>(_returnType, _matchSubType);
        }
        
        public ICallConstraint CreateCallConstraint()
        {
            return new ReturnCallConstraint(_returnType, _matchSubType);
        }
        
        private bool ReturnTypeValid(Type callReturnType)
        {
            if (ReferenceEquals(callReturnType, _returnType))
            {
                return true;
            }
            
            return _matchSubType && _returnType.IsAssignableFrom(callReturnType);
        }
    }
}