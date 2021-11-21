﻿using System;
using System.Linq;
using System.Reflection;

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
            var returnType = redirectDelegate.Method.ReturnType;
            
            if (!ReturnTypeValid(returnType))
            {
                throw new InvalidRedirectException($"'{redirectDelegate.Method.ReturnType.FullName}' invalid redirect return type To method '{Method}'");
            }
            
            if (DelegateParametersValid(redirectDelegate))
            {
                return;
            }
            
            var delegateParameters = redirectDelegate.Method.GetParameters();
            var parameterTypes = delegateParameters.Select(x => $"{x.ParameterType.FullName} {x.Name}");
            var delegateSignature = $"Redirect({string.Join(", ", parameterTypes)})";
            
            throw new InvalidRedirectException($"'{delegateSignature}' parameters invalid for To method '{Method}'");
        }

        public void Validate(Type returnType, Type[] argumentTypes, bool isStrict = true)
        {
            if (!ReturnTypeValid(returnType))
            {
                throw new InvalidRedirectException($"'{returnType.FullName}' invalid return type for To method '{Method}'");
            }

            var checkArguments =
                isStrict ? (Func<Type[], ParameterInfo[], bool>) ArgumentTypesValidStrict : ArgumentTypesValid;
            
            if (checkArguments(argumentTypes, ParameterInfos))
            {
                return;
            }
            
            var parameterTypes = argumentTypes.Select(x => $"{x.FullName} {x.Name}");
            var delegateSignature = $"Redirect({string.Join(", ", parameterTypes)})";
            
            throw new InvalidRedirectException($"'{delegateSignature}' parameters invalid for To method '{Method}'");
        }

        public void ValidateArgumentTypes(params Type[] types)
        {
            if (ArgumentTypesValid(types, ParameterInfos))
            {
                return;
            }
            
            var errorMessage = $"Parameter types invalid for To method '{Method}'";
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

        private bool ReturnTypeValid(Type returnType)
        {
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
        
        private static bool ArgumentTypesValidStrict(Type[] testTypes, ParameterInfo[] callParams)
        {
            if (testTypes.Length == 0)
            {
                return true;
            }
            
            if (testTypes.Length != callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < testTypes.Length; i++)
            {
                if (!IsArgumentTypeValid(testTypes[i], callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ArgumentTypesValid(Type[] testTypes, ParameterInfo[] callParams)
        {
            if (testTypes.Length > callParams.Length)
            {
                return false;
            }

            for (var i = 0; i < testTypes.Length; i++)
            {
                if (!IsArgumentTypeValid(testTypes[i], callParams[i].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsArgumentTypeValid(Type testType, Type parameterType)
        {
            return !parameterType.IsByRef 
                ? IsTypeValid(testType, parameterType)
                : IsRefTypeValid(testType, parameterType);
        }
        
        private static bool IsTypeValid(Type testType, Type parameterType)
        {
            if (ReferenceEquals(testType, parameterType))
            {
                return true;
            }
                
            if (testType.IsAssignableFrom(parameterType))
            {
                return true;
            }

            return false;
        }

        private static bool IsRefTypeValid(Type refType, Type parameterType)
        {
            if (!refType.IsGenericType ||
                refType.GenericTypeArguments.Length != 1 ||
                refType.GetGenericTypeDefinition() != typeof(Ref<>))
            {
                return false;
            }

            var elementType = parameterType.GetElementType();
            
            if (ReferenceEquals(refType.GenericTypeArguments[0], elementType))
            {
                return true;
            }
                
            if (!refType.GenericTypeArguments[0].IsAssignableFrom(elementType))
            {
                return true;
            }

            return false;
        }
    }
}