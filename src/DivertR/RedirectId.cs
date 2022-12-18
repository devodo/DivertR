using System;

namespace DivertR
{
    public readonly struct RedirectId : IEquatable<RedirectId>
    {
        private readonly (Type type, string name) _id;
        
        public RedirectId(Type type, string? name = null)
        {
            _id = (type, name ?? string.Empty);
        }

        public Type Type => _id.type;
        public string Name => _id.name;

        public bool Equals(RedirectId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            return obj is RedirectId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Type:{Type.Name}" + (string.IsNullOrEmpty(Name) ? $" Name:<empty>" : $" Name: {Name}");
        }

        public static RedirectId From(Type type, string? name = null)
        {
            return new RedirectId(type, name ?? string.Empty);
        }

        public static RedirectId From<TTarget>(string? name = null)
        {
            return new RedirectId(typeof(TTarget), name ?? string.Empty);
        }
    }
}
