using System.Reflection;

namespace DivertR.Core
{
    public interface IMethodConstraint
    {
        bool IsMatch(MethodInfo methodInfo);
    }
}