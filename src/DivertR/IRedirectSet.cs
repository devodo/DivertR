using System;

namespace DivertR
{
    /// <summary>
    /// Contains and manages a set of <see cref="IRedirect"/> instances that are unique by <see cref="RedirectId"/> key.
    /// </summary>
    public interface IRedirectSet
    {
        /// <summary>
        /// The <see cref="DiverterSettings" /> used by this set and all its <see cref="IRedirect"/> instances.
        /// </summary>
        DiverterSettings Settings { get; }
        
        /// <summary>
        /// Get the <see cref="IRedirect{TTarget}"/> in this set by <see cref="RedirectId"/> key generated from <typeparamref name="TTarget"/> and optional <paramref name="name"/>.
        /// If the <see cref="IRedirect"/> does not exist in this set, a new one is created and returned.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <typeparam name="TTarget">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</typeparam>
        /// <returns>The existing or created <see cref="IRedirect"/>.</returns>
        IRedirect<TTarget> GetOrCreate<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> in this set by <see cref="RedirectId"/> key generated from <paramref name="type"/> and optional <paramref name="name"/>.
        /// If the <see cref="IRedirect"/> does not exist in this set, a new one is created and returned.
        /// </summary>
        /// <param name="type">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</param>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <returns>The existing or created <see cref="IRedirect"/>.</returns>
        IRedirect GetOrCreate(Type type, string? name = null);

        /// <summary>
        /// Get the <see cref="IRedirect"/> in this set by <paramref name="redirectId"/> key.
        /// If the <see cref="IRedirect"/> does not exist in this set, a new one is created and returned.
        /// </summary>
        /// <param name="redirectId">The <see cref="IRedirect"/> key.</param>
        /// <returns>The existing or created <see cref="IRedirect"/>.</returns>
        IRedirect GetOrCreate(RedirectId redirectId);
        
        /// <summary>
        /// Get the <see cref="IRedirect{TTarget}"/> in this set by <see cref="RedirectId"/> key generated from <typeparamref name="TTarget"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <typeparam name="TTarget">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</typeparam>
        /// <returns>The <see cref="IRedirect"/> instance or null if it does not exist in the set.</returns>
        IRedirect<TTarget>? Get<TTarget>(string? name = null) where TTarget : class?;
        
        /// <summary>
        /// Get the <see cref="IRedirect"/> in this set by <see cref="RedirectId"/> key generated from <paramref name="type"/> and optional <paramref name="name"/>.
        /// </summary>
        /// <param name="type">The <see cref="RedirectId.Type" /> of the <see cref="IRedirect"/>.</param>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/>.</param>
        /// <returns>The <see cref="IRedirect"/> instance or null if it does not exist in the set.</returns>
        IRedirect? Get(Type type, string? name = null);

        /// <summary>
        /// Get the <see cref="IRedirect"/> in this set by <paramref name="redirectId"/> key.
        /// </summary>
        /// <param name="redirectId">The <see cref="IRedirect"/> key.</param>
        /// <returns>The <see cref="IRedirect"/> instance or null if it does not exist in the set.</returns>
        IRedirect? Get(RedirectId redirectId);

        /// <summary>
        /// Reset an <see cref="IRedirect"/> group in this set with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IRedirectSet"/> instance.</returns>
        IRedirectSet Reset(string? name = null);
        
        /// <summary>
        /// Reset all <see cref="IRedirect"/> instances in this set.
        /// </summary>
        /// <returns>This <see cref="IRedirectSet"/> instance.</returns>
        IRedirectSet ResetAll();
        
        /// <summary>
        /// Enable strict mode on an <see cref="IRedirect"/> group in this set with name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the <see cref="IRedirect"/> group.</param>
        /// <returns>This <see cref="IRedirectSet"/> instance.</returns>
        IRedirectSet Strict(string? name = null);
        
        /// <summary>
        /// Enable strict mode on all <see cref="IRedirect"/> instances in this set.
        /// </summary>
        /// <returns>This <see cref="IRedirectSet"/> instance.</returns>
        IRedirectSet StrictAll();
    }
}