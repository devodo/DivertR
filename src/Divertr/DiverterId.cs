using System;

namespace Divertr
{
    public readonly struct DiverterId : IEquatable<DiverterId>
    {
        public Type Type { get; }
        public string? Name { get; }

        public DiverterId(Type type, string? name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(DiverterId other)
        {
            return ReferenceEquals(Type, other.Type) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is DiverterId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Type.GetHashCode();

                if (Name != null)
                {
                    hash = hash * 31 + Name.GetHashCode();
                }
                
                return hash;
            }
        }

        public static DiverterId From(Type type, string? name = null)
        {
            return new DiverterId(type, name);
        }

        public static DiverterId From<T>(string? name = null)
        {
            return new DiverterId(typeof(T), name);
        }
    }
}
