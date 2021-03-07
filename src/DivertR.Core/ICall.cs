using System.Reflection;

namespace DivertR.Core
{
    public interface ICall
    {
        public MethodInfo Method { get; }

        public object[] Arguments { get; }
    }
}