using System;
using System.Linq;
using System.Reflection;
using DivertR.Core;

namespace DivertR.Internal
{
    public class ParsedCall
    {
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _parameterInfos;
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConstraints;

        internal ParsedCall(MethodInfo method, ParameterInfo[] parameterInfos, IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConstraints)
        {
            _method = method;
            _parameterInfos = parameterInfos;
            _methodConstraint = methodConstraint;
            _argumentConstraints = argumentConstraints;
        }
        
        public ICallConstraint CreateCallConstraint()
        {
            return new MethodCallConstraint(_method, _methodConstraint, _argumentConstraints);
        }

        public void Validate(Delegate redirectDelegate)
        {
            if (ReturnTypeValid(redirectDelegate) && ParametersValid(redirectDelegate))
            {
                return;
            }
            
            throw new DiverterException(CreateIncompatibleMessage(redirectDelegate, _method));
        }
        
        private bool ParametersValid(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();

            return DelegateParametersValid(delegateParameters, _parameterInfos);
        }

        private bool ReturnTypeValid(Delegate redirectDelegate)
        {
            var returnType = redirectDelegate.Method.ReturnType;

            if (ReferenceEquals(returnType, _method.ReturnType))
            {
                return true;
            }

            if (ReferenceEquals(returnType, typeof(void)) ||
                ReferenceEquals(_method.ReturnType, typeof(void)))
            {
                return false;
            }

            if (_method.ReturnType.IsAssignableFrom(returnType))
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
        
        private static string CreateDelegateSignature(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();
            var parameterTypes = delegateParameters.Select(x => x.ParameterType.FullName);
            return $"{redirectDelegate.Method.ReturnType.FullName} Invoke({string.Join(", ", parameterTypes)})";
        }

        private static string CreateIncompatibleMessage(Delegate redirectDelegate, MethodInfo methodInfo)
        {
            return $"To() delegate '{CreateDelegateSignature(redirectDelegate)}' invalid for redirect method '{methodInfo}'";
        }
    }
}