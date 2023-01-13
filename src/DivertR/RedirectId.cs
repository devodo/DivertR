﻿using System;

namespace DivertR
{
    /// <summary>
    /// A simple immutable struct intended to be used as an <see cref="IRedirect"/> composite key.
    /// </summary>
    public readonly struct RedirectId : IEquatable<RedirectId>
    {
        private readonly (Type type, string name) _id;
        
        /// <summary>
        /// RedirectId constructor
        /// </summary>
        /// <param name="type">The <see cref="IRedirect"/> target type.</param>
        /// <param name="name">The <see cref="IRedirect"/> target name.</param>
        public RedirectId(Type type, string? name = null)
        {
            _id = (type, name ?? string.Empty);
        }
        
        /// <summary>
        /// The <see cref="IRedirect"/> target type.
        /// </summary>
        public Type Type => _id.type;
        
        /// <summary>
        /// The <see cref="IRedirect"/> target name.
        /// </summary>
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
        
        /// <summary>
        /// Static wrapper for the <see cref="RedirectId"/> constructor
        /// </summary>
        /// <param name="type">The <see cref="IRedirect"/> target type.</param>
        /// <param name="name">The <see cref="IRedirect"/> target name.</param>
        /// <returns>The constructed <see cref="RedirectId"/> instance.</returns>
        public static RedirectId From(Type type, string? name = null)
        {
            return new RedirectId(type, name ?? string.Empty);
        }
        
        /// <summary>
        /// Static wrapper for the <see cref="RedirectId"/> constructor
        /// </summary>
        /// <param name="name">The <see cref="IRedirect"/> target name.</param>
        /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
        /// <returns>The constructed <see cref="RedirectId"/> instance.</returns>
        public static RedirectId From<TTarget>(string? name = null)
        {
            return new RedirectId(typeof(TTarget), name ?? string.Empty);
        }
    }
}
