using System;
using System.Linq;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ParsedCallExpression
    {
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConstraints;
        
        public MethodInfo Method { get; }
        public ParameterInfo[] ParameterInfos { get; }

        internal ParsedCallExpression(MethodInfo method, ParameterInfo[] parameterInfos, IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConstraints)
        {
            Method = method;
            ParameterInfos = parameterInfos;
            _methodConstraint = methodConstraint;
            _argumentConstraints = argumentConstraints;
        }

        public void Validate(Delegate redirectDelegate)
        {
            if (ReturnTypeValid(redirectDelegate) && ParametersValid(redirectDelegate))
            {
                return;
            }
            
            string DelegateSignatureToString()
            {
                var delegateParameters = redirectDelegate.Method.GetParameters();
                var parameterTypes = delegateParameters.Select(x => x.ParameterType.FullName);
                return $"{redirectDelegate.Method.ReturnType.FullName} Invoke({string.Join(", ", parameterTypes)})";
            }

            var errorMessage = $"To() delegate '{DelegateSignatureToString()}' invalid for redirect method '{Method}'";
            throw new DiverterException(errorMessage);
        }

        public ICallConstraint<T> ToCallConstraint<T>() where T : class
        {
            return new MethodCallConstraint<T>(_methodConstraint, _argumentConstraints);
        }
        
        private bool ParametersValid(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();

            return DelegateParametersValid(delegateParameters, ParameterInfos);
        }

        private bool ReturnTypeValid(Delegate redirectDelegate)
        {
            var returnType = redirectDelegate.Method.ReturnType;

            if (ReferenceEquals(returnType, Method.ReturnType))
            {
                return true;
            }
            
            if (Method.ReturnType.IsAssignableFrom(returnType))
            {
                return true;
            }

            return false;
        }
        
        private static bool DelegateParametersValid(ParameterInfo[] delegateParams, ParameterInfo[] callParams)
        {
            if (delegateParams.Length == 0)
            {
                return true;
            }
            
            if (delegateParams.Length != callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < delegateParams.Length; i++)
            {
                if (ReferenceEquals(delegateParams[i].ParameterType, callParams[i].ParameterType))
                {
                    continue;
                }
                
                if (!delegateParams[i].ParameterType.IsAssignableFrom(callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}