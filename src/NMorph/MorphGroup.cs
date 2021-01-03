using System;

namespace NMorph
{
    internal readonly struct MorphGroup : IEquatable<MorphGroup>
    {
        public Type Type { get; }
        public string Name { get; }

        public MorphGroup(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(MorphGroup other)
        {
            return ReferenceEquals(Type, other.Type) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is MorphGroup other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }

        public static MorphGroup From(Type type, string name)
        {
            return new MorphGroup(type, name);
        }

        public static MorphGroup From<T>(string name)
        {
            return new MorphGroup(typeof(T), name);
        }
    }
}
