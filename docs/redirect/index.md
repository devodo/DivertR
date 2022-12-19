---
layout: default
title: Redirect
nav_order: 2
has_children: true
---

# Redirect

Redirects are the main DivertR entities used to create and configure proxies.
`Redirect` instances are instantiated from the generic `Redirect<TTarget>` class:

```csharp
IRedirect<IFoo> fooRedirect = new Redirect<IFoo>();
```
