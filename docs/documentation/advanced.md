---
layout: default
title: Advanced
nav_order: 5
parent: Documentation
---

# Advanced

# Redirect Id

Every `Redirect<TTarget>` instance has a readonly `RedirectId` property that is a composite key made up of:

1. The `TTarget` type.
2. An optional `string` name (defaults to an empty `string` if not specified).

```csharp
var fooRedirect = new Redirect<IFoo>();

Assert.Equal(typeof(IFoo), fooRedirect.RedirectId.Type);
Assert.Equal(string.Empty, fooRedirect.RedirectId.Name);
Assert.Equal(fooRedirect.RedirectId, new RedirectId(typeof(IFoo)));
```

The optional name can be specified on creation:

```csharp
var fooRedirect2 = new Redirect<IFoo>("GroupX");
        
Assert.Equal(typeof(IFoo), fooRedirect2.RedirectId.Type);
Assert.Equal("GroupX", fooRedirect2.RedirectId.Name);
Assert.Equal(fooRedirect2.RedirectId, new RedirectId(typeof(IFoo), "GroupX"));
```

The `RedirectId` property is intended to be used as a key for indexing Redirects in collections such as the [RedirectSet](#redirect-set) discussed next.

# Redirect Set

The `RedirectSet` class manages a collection of `Redirect` instances that are unique by their [RedirectId](#redirect-id) key.
The main functions of `RedirectSet` are:

1. Create and internally store a set of `Redirect` instances.
2. Retrieve stored Redirects by `RedirectId`.
3. Perform `Redirect` actions such as `Reset` across all stored Redirects or groups of named subsets.

```csharp
// Instantiate a new RedirectSet
IRedirectSet redirectSet = new RedirectSet();
// Create and store a Redirect instance
IRedirect<IFoo> fooRedirect = redirectSet.GetOrCreate<IFoo>();
// The Redirect has already been created so the existing instance is returned
IRedirect<IFoo> fooRedirect2 = redirectSet.GetOrCreate<IFoo>();

Assert.Same(fooRedirect, fooRedirect2);

// Create and store sets of Redirects for different target types
var barRedirect = redirectSet.GetOrCreate<IBar>();
// Or multiple with the same type using names
var fooRedirect3 = redirectSet.GetOrCreate<IFoo>("GroupX");

Assert.NotNull(barRedirect);
Assert.NotEqual(fooRedirect, fooRedirect3);

// Perform a Redirect action across all Redirects in the set
redirectSet.ResetAll();
// Or across a subset by name
redirectSet.Reset("GroupX");
```

An important usage of `RedirectSet` is by the `Diverter` class when integrating with [dependency injection](../dependency_injection/) containers.

## Named Redirect

# Via Options

## Persistent Via

## Via Order

## Repeat