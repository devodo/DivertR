using System;

namespace NMorph
{
    internal readonly struct MorphId : IEquatable<MorphId>
    {
        public Type Type { get; }
        public string Name { get; }

        public MorphId(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(MorphId other)
        {
            return ReferenceEquals(Type, other.Type) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is MorphId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }

        public static MorphId From(Type type, string name)
        {
            return new MorphId(type, name);
        }

        public static MorphId From<T>(string name)
        {
            return new MorphId(typeof(T), name);
        }
    }
}
