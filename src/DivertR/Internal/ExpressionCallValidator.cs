using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DivertR.Internal
{
    internal class ExpressionCallValidator : ICallValidator
    {
        private readonly IMethodConstraint _methodConstraint;
        private readonly IArgumentConstraint[] _argumentConstraints;
        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _parameterInfos;


        internal ExpressionCallValidator(MethodInfo method, ParameterInfo[] parameterInfos, IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConstraints)
        {
            _method = method;
            _parameterInfos = parameterInfos;
            _methodConstraint = methodConstraint;
            _argumentConstraints = argumentConstraints;
        }

        public void Validate(IValueTupleMapper valueTupleMapper)
        {
            var argumentTypes = valueTupleMapper.ArgumentTypes
                .Select(x => (x, (ParameterInfo?) null))
                .ToArray();
            
            var validations = ValidateArgumentTypes(argumentTypes, false).ToArray();

            if (validations.All(x => x.isValid))
            {
                return;
            }

            var valueTupleArguments = string.Join(", ", valueTupleMapper.ArgumentTypes.Select(x => x.Name));
            var details = $"{string.Join(Environment.NewLine, validations.Select(x => x.message))}";

            throw new DiverterValidationException(
                $"ValueTuple ({valueTupleArguments}) arguments invalid for call to {GetMethodParameterSignature()}" +
                $"{Environment.NewLine}{details}");
        }

        public ICallConstraint CreateCallConstraint()
        {
            return new MethodCallConstraint(_methodConstraint, _argumentConstraints);
        }
        
        private string GetMethodParameterSignature()
        {
            var methodParameters = string.Join(", ", _parameterInfos.Select(x => x.ParameterType.Name));

            string? genericArguments = null;
            
            if (_method.IsGenericMethod)
            {
                var joinedArgs = string.Join(", ", _method.GetGenericArguments().Select(x => x.Name));
                genericArguments = $"<{joinedArgs}>";
            }
            
            return $"{_method.Name}{genericArguments}({methodParameters})";
        }

        private IEnumerable<(bool isValid, string? message)> ValidateArgumentTypes((Type type, ParameterInfo? parameter)[] argumentTypes, bool isStrict)
        {
            var parameterTypes = _parameterInfos
                .Select(x => x)
                .Concat(Enumerable
                    .Range(0, Math.Max(0, argumentTypes.Length - _parameterInfos.Length))
                    .Select(_ => (ParameterInfo?) null));
            
            var argTypes = argumentTypes
                .Select(x => x)
                .Concat(Enumerable
                    .Range(0, Math.Max(0, _parameterInfos.Length - argumentTypes.Length))
                    .Select(_ => ((Type type, ParameterInfo? parameter)) default));

            var zip = parameterTypes
                .Zip(argTypes, (parameterType, argumentType) => (parameterType, argumentType))
                .Select((x, i) => (x.parameterType, x.argumentType, i));

            foreach (var (parameter, argumentType, index) in zip)
            {
                var isValid = IsArgumentTypeValid(argumentType, parameter, isStrict);
                
                var parameterTypeName = parameter?.ParameterType.Name;
                var argumentTypeName = argumentType.type?.Name;
                
                string message;

                if (isValid)
                {
                    message = $"valid";
                }
                else
                {
                    if (parameterTypeName == argumentTypeName)
                    {
                        parameterTypeName = parameter?.ParameterType.FullName;
                        argumentTypeName = argumentType.type?.FullName;
                    }
                    
                    if (parameter != null && argumentType.type != null)
                    {
                        message = $"invalid assignment from ({argumentTypeName})";
                    }
                    else if (parameter == null)
                    {
                        message = $"parameter index out of range";
                    }
                    else
                    {
                        message = $"method parameter required";
                    }
                }

                yield return (isValid, $"{(isValid ? "---" : "***")} [{index}:{parameterTypeName ?? "???"}] {message}");
            }
        }

        private static bool IsArgumentTypeValid((Type type, ParameterInfo? parameter) test, ParameterInfo? parameter, bool isStrict = true)
        {
            if (parameter == null)
            {
                return false;
            }
            
            if (test.type == null)
            {
                return !isStrict;
            }

            if (!parameter.ParameterType.IsByRef)
            {
                return IsTypeValid(test.type, parameter.ParameterType);
            }

            return IsRefTypeValid(test, parameter);
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

        private static bool IsRefTypeValid((Type type, ParameterInfo? parameter) test, ParameterInfo parameter)
        {
            if (!parameter.ParameterType.IsByRef)
            {
                return false;
            }

            if (test.parameter != null)
            {
                if (!test.type.IsByRef)
                {
                    return false;
                }

                return test.parameter.IsOut == parameter.IsOut && test.parameter.IsIn == parameter.IsIn;
            }
            
            if (!test.type.IsGenericType ||
                test.type.GenericTypeArguments.Length != 1 ||
                test.type.GetGenericTypeDefinition() != typeof(Ref<>))
            {
                return false;
            }

            var elementType = parameter.ParameterType.GetElementType();

            if (elementType == null)
            {
                return false;
            }
            
            return IsTypeValid(test.type.GenericTypeArguments[0], elementType);
        }
    }
}