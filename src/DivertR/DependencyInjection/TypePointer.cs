using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DivertR.DependencyInjection
{
    public class TypePointer : TypeDelegator
    {
        public TypePointer(Type delegatingType) : base(delegatingType)
        {
        }
        
        public override bool Equals(Type? other) => ReferenceEquals(this, other);

        public override bool Equals(object? obj) => ReferenceEquals(this, obj);

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}