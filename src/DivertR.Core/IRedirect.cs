using System.Reflection;

namespace DivertR.Core
{
    public interface IRedirect<out T> where T : class
    {
        object? State { get; }

        object? Invoke(MethodInfo methodInfo, object[] args);
        bool IsMatch(ICall call);
    }
}