using System.Reflection;

namespace DivertR.Internal
{
    internal interface IMethodConstraint
    {
        bool IsMatch(MethodInfo methodInfo);
    }
}