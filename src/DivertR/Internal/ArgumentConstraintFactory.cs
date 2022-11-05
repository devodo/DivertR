using System.Linq.Expressions;
using System.Reflection;

namespace DivertR.Internal
{
    internal interface IArgumentConstraintFactory
    {
        IArgumentConstraint CreateTypeArgumentConstraint(ParameterInfo parameterInfo);
        IArgumentConstraint CreateLambdaArgumentConstraint(ParameterInfo parameterInfo, LambdaExpression lambdaExpression);
    }

    internal class ArgumentConstraintFactory<TArgument> : IArgumentConstraintFactory
    {
        public IArgumentConstraint CreateTypeArgumentConstraint(ParameterInfo parameterInfo)
        {
            return new TypeArgumentConstraint<TArgument>(parameterInfo);
        }

        public IArgumentConstraint CreateLambdaArgumentConstraint(ParameterInfo parameterInfo, LambdaExpression lambdaExpression)
        {
            return new LambdaArgumentConstraint<TArgument>(parameterInfo, lambdaExpression);
        }
    }
}