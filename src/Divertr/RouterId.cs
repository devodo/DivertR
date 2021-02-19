using System;

namespace Divertr
{
    public readonly struct RouterId : IEquatable<RouterId>
    {
        public Type Type { get; }
        public string? Name { get; }

        public RouterId(Type type, string? name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(RouterId other)
        {
            return ReferenceEquals(Type, other.Type) && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is RouterId other && Equals(other);
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

        public static RouterId From(Type type, string? name = null)
        {
            return new RouterId(type, name);
        }

        public static RouterId From<T>(string? name = null)
        {
            return new RouterId(typeof(T), name);
        }
    }
}
