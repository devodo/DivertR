using System;

namespace DivertR
{
    public readonly struct ViaId : IEquatable<ViaId>
    {
        private readonly (Type type, string name) _id;
        
        public ViaId(Type type, string? name = null)
        {
            _id = (type, name ?? string.Empty);
        }

        public Type Type => _id.type;
        public string Name => _id.name;

        public bool Equals(ViaId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            return obj is ViaId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Type:{Type.Name}" + (string.IsNullOrEmpty(Name) ? $" Name:<empty>" : $" Name: {Name}");
        }

        public static ViaId From(Type type, string? name = null)
        {
            return new ViaId(type, name ?? string.Empty);
        }

        public static ViaId From<TTarget>(string? name = null)
        {
            return new ViaId(typeof(TTarget), name ?? string.Empty);
        }
    }
}
