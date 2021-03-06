using System;
using System.Reflection;

namespace DivertR.Internal
{
    internal readonly struct MethodId : IEquatable<MethodId>
    {
        public Type TargetType { get; }
        public MethodInfo MethodInfo { get; }

        public MethodId(Type targetType, MethodInfo methodInfo)
        {
            TargetType = targetType;
            MethodInfo = methodInfo;
        }

        public bool Equals(MethodId other)
        {
            return ReferenceEquals(TargetType, other.TargetType) && MethodInfo == other.MethodInfo;
        }

        public override bool Equals(object obj)
        {
            return obj is MethodId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17 * 31 + TargetType.GetHashCode();
                hash = hash * 31 + MethodInfo.GetHashCode();
                
                return hash;
            }
        }
    }
}
