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


        internal ExpressionCallValidator(MethodInfo method, IMethodConstraint methodConstraint, IArgumentConstraint[] argumentConstraints)
        {
            _method = method;
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
            var methodParameters = string.Join(", ", _argumentConstraints.Select(x => x.Parameter.ParameterType.Name));

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
            var argumentConstraints = _argumentConstraints
                .Concat(Enumerable
                    .Range(0, Math.Max(0, argumentTypes.Length - _argumentConstraints.Length))
                    .Select(_ => (IArgumentConstraint?) null));

            var argTypes = argumentTypes
                .Concat(Enumerable
                    .Range(0, Math.Max(0, _argumentConstraints.Length - argumentTypes.Length))
                    .Select(_ => ((Type type, ParameterInfo? parameter)) default));

            var zip = argumentConstraints
                .Zip(argTypes, (argumentConstraint, argumentType) => (argumentConstraint, argumentType))
                .Select((x, i) => (x.argumentConstraint, x.argumentType, i));

            foreach (var (argumentConstraint, argumentType, index) in zip)
            {
                var isValid = IsArgumentTypeValid(argumentType, argumentConstraint, isStrict);
                
                var parameterTypeName = argumentConstraint?.Parameter.ParameterType.Name;
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
                        parameterTypeName = argumentConstraint?.Parameter.ParameterType.FullName;
                        argumentTypeName = argumentType.type?.FullName;
                    }
                    
                    if (argumentConstraint != null && argumentType.type != null)
                    {
                        message = $"invalid assignment to ({argumentTypeName})";
                    }
                    else if (argumentConstraint == null)
                    {
                        message = $"parameter index out of range";
                    }
                    else
                    {
                        message = $"method parameter required";
                    }
                }

                var paramType = parameterTypeName ?? (argumentType.type == typeof(__) ? "__" : "???");

                yield return (isValid, $"{(isValid ? "  =" : "  *")} [{index}:{paramType}] {message}");
            }
        }

        private static bool IsArgumentTypeValid((Type type, ParameterInfo? parameter) test, IArgumentConstraint? argumentConstraint, bool isStrict = true)
        {
            if (test.type == typeof(__))
            {
                return true;
            }
            
            if (argumentConstraint == null)
            {
                return false;
            }
            
            if (test.type == null)
            {
                return !isStrict;
            }

            if (!argumentConstraint.Parameter.ParameterType.IsByRef)
            {
                return IsTypeValid(test.type, argumentConstraint.Parameter.ParameterType, argumentConstraint.ArgumentType);
            }

            return IsRefTypeValid(test, argumentConstraint);
        }
        
        private static bool IsTypeValid(Type testType, Type parameterType, Type? argumentType = null)
        {
            if (ReferenceEquals(testType, parameterType))
            {
                return true;
            }
                
            if (testType.IsAssignableFrom(parameterType))
            {
                return true;
            }

            if (argumentType != null && testType.IsAssignableFrom(argumentType))
            {
                return true;
            }

            return false;
        }

        private static bool IsRefTypeValid((Type type, ParameterInfo? parameter) test, IArgumentConstraint argumentConstraint)
        {
            if (!argumentConstraint.Parameter.ParameterType.IsByRef)
            {
                return false;
            }

            if (test.parameter != null)
            {
                if (!test.type.IsByRef)
                {
                    return false;
                }

                return test.parameter.IsOut == argumentConstraint.Parameter.IsOut && test.parameter.IsIn == argumentConstraint.Parameter.IsIn;
            }
            
            if (!test.type.IsGenericType ||
                test.type.GenericTypeArguments.Length != 1 ||
                test.type.GetGenericTypeDefinition() != typeof(Ref<>))
            {
                return false;
            }

            var elementType = argumentConstraint.Parameter.ParameterType.GetElementType();

            if (elementType == null)
            {
                return false;
            }
            
            return IsTypeValid(test.type.GenericTypeArguments[0], elementType);
        }
    }
}