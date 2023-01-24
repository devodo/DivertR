---
layout: default
title: Proxy Factory
nav_order: 4
parent: Documentation
---

# Proxy Factory

DivertR dynamically generate its proxies using an underlying proxy factory. The proxy factory interface is abstracted and the concrete implementation can be configured and switched. 

## DispatchProxy

The default proxy factory is implemented using [System.Reflection.DispatchProxy](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy).
This is included in the .NET Standard 2.1 and DivertR therefore does not require dependencies on external libraries however it is limited to proxying interface types only.

> DispatchProxy is limited to **interface types** only
{: .note }


## [DivertR.DynamicProxy](https://github.com/devodo/DivertR/tree/main/src/DivertR.DynamicProxy)

DivertR is designed to be transparent so it does not alter the behaviour of the original system by relaying calls to root instances.
This is best achieved when proxying interface types only.

In some scenarios class proxies may be required and in this case [DivertR.DynamicProxy](https://github.com/devodo/DivertR/tree/main/src/DivertR.DynamicProxy) is an alternative proxy factory implementation that does support this based on [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/).

> When proxying classes, care should be taken as only calls to non-virtual members cannot be intercepted, this can cause inconsistent behaviour e.g. when relaying to root instances.
{: .note }
