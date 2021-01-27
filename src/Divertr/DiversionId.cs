using System;

namespace Divertr
{
    internal readonly struct DiversionId : IEquatable<DiversionId>
    {
        public Type Type { get; }
        public string Name { get; }

        public DiversionId(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(DiversionId other)
        {
            return ReferenceEquals(Type, other.Type) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is DiversionId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }

        public static DiversionId From(Type type, string name = null)
        {
            return new DiversionId(type, name);
        }

        public static DiversionId From<T>(string name = null)
        {
            return new DiversionId(typeof(T), name);
        }
    }
}
