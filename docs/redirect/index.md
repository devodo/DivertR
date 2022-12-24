---
layout: default
title: Redirects
nav_order: 2
has_children: true
---

# Redirects

The `Redirect` is the main DivertR type used to create and configure test proxies.
`Redirect` instances are instantiated from the generic `Redirect<TTarget>` class:

```csharp
IRedirect<IFoo> fooRedirect = new Redirect<IFoo>();
```
