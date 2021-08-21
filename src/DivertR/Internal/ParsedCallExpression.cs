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
            if (ReturnTypeValid(redirectDelegate) && DelegateParametersValid(redirectDelegate))
            {
                return;
            }
            
            var delegateParameters = redirectDelegate.Method.GetParameters();
            var parameterTypes = delegateParameters.Select(x => x.ParameterType.FullName);
            var delegateSignature = $"{redirectDelegate.Method.ReturnType.FullName} Invoke({string.Join(", ", parameterTypes)})";

            var errorMessage = $"Redirect() delegate '{delegateSignature}' invalid for To() method '{Method}'";
            throw new DiverterException(errorMessage);
        }

        public void ValidateParameterTypes(params Type[] types)
        {
            if (ParametersTypesValid(types, ParameterInfos))
            {
                return;
            }
            
            var errorMessage = $"Parameter types invalid for To() method '{Method}'";
            throw new DiverterException(errorMessage);
        }

        public ICallConstraint<TTarget> ToCallConstraint<TTarget>() where TTarget : class
        {
            return new MethodCallConstraint<TTarget>(_methodConstraint, _argumentConstraints);
        }
        
        private bool DelegateParametersValid(Delegate redirectDelegate)
        {
            var delegateParameters = redirectDelegate.Method.GetParameters();

            return ParametersValidStrict(delegateParameters, ParameterInfos);
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
        
        private static bool ParametersValidStrict(ParameterInfo[] testParams, ParameterInfo[] callParams)
        {
            if (testParams.Length == 0)
            {
                return true;
            }
            
            if (testParams.Length != callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < testParams.Length; i++)
            {
                if (ReferenceEquals(testParams[i].ParameterType, callParams[i].ParameterType))
                {
                    continue;
                }
                
                if (!testParams[i].ParameterType.IsAssignableFrom(callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
        
        private static bool ParametersTypesValid(Type[] testTypes, ParameterInfo[] callParams)
        {
            if (testTypes.Length > callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < testTypes.Length; i++)
            {
                if (ReferenceEquals(testTypes[i], callParams[i].ParameterType))
                {
                    continue;
                }
                
                if (!testTypes[i].IsAssignableFrom(callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}