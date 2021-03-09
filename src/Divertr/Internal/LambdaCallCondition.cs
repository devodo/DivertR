using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using DivertR.Core;
using DivertR.Core.Internal;

namespace DivertR.Internal
{
    internal class LambdaCallCondition : ICallCondition
    {
        private readonly MethodInfo _methodInfo;
        private readonly ReadOnlyCollection<Expression> _arguments;
        private readonly ParameterInfo[] _parameters;

        public LambdaCallCondition(MethodInfo methodInfo, ReadOnlyCollection<Expression> arguments, ParameterInfo[] parameters)
        {
            _methodInfo = methodInfo;
            _arguments = arguments;
            _parameters = parameters;
        }

        public bool IsMatch(ICall call)
        {
            if (_methodInfo != call.Method)
            {
                return false;
            }

            for (var i = 0; i < _arguments.Count; i++)
            {
                if (!(_arguments[i] is ConstantExpression expressionValue))
                {
                    continue;
                }

                if (expressionValue.Value != call.Arguments[i])
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
