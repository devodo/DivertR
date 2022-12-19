---
layout: default
title: Proxy Lifetime
nav_order: 4
parent: Dependency Injection
---

# Proxy Lifetime

DivertR aims to leave the original system behaviour unchanged and therefore
when existing DI registrations are replaced by Redirect decorators the lifetime of the registration is preserved.

For multiple instance registrations such as transients, a separate proxy instance is created for each but all from the same Redirect instance.
In other words all proxies resolved from a Redirect decorated registration are managed from this single Redirect.

# Dispose

If a DI created root instance implements the `IDisposable` interface then the DI container manages its disposal, as usual, according to its registration lifetime.

If a DI Redirect proxy is an `IDisposable` then **only** the proxy instance is disposed by the DI container and not the root.
In this case the responsibilty is left to the proxy for forwarding the dispose call to its root (and it does this by default).

The above also applies to `IAsyncDisposable`.
