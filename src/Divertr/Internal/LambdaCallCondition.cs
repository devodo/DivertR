using System.Collections.Generic;
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
        private readonly IArgumentCondition[] _argumentConditions;

        public LambdaCallCondition(MethodInfo methodInfo, IArgumentCondition[] argumentConditions)
        {
            _methodInfo = methodInfo;
            _argumentConditions = argumentConditions;
        }

        public bool IsMatch(ICall call)
        {
            if (_methodInfo != call.Method)
            {
                return false;
            }

            for (var i = 0; i < _argumentConditions.Length; i++)
            {
                if (!_argumentConditions[i].IsMatch(call.Arguments[i]))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
