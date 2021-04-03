using System;

namespace DivertR.Core
{
    public readonly struct ViaId : IEquatable<ViaId>
    {
        public Type Type { get; }
        public string? Name { get; }

        public ViaId(Type type, string? name)
        {
            Type = type;
            Name = name;
        }

        public bool Equals(ViaId other)
        {
            return Type == other.Type && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is ViaId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + Type.GetHashCode();

                if (Name != null)
                {
                    hash = hash * 31 + Name.GetHashCode();
                }
                
                return hash;
            }
        }

        public static ViaId From(Type type, string? name = null)
        {
            return new ViaId(type, name);
        }

        public static ViaId From<TTarget>(string? name = null)
        {
            return new ViaId(typeof(TTarget), name);
        }
    }
}
