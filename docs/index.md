---
title: Home
layout: home
nav_order: 1
---

# DivertR

Welcome to DivertR! The .NET mocking framework for integration testing.

Why yet another mocking framework?

DivertR is similar to existing frameworks like [Moq](https://github.com/moq/moq4) but provides additional features for integration testing.

It works by updating the dependency injection container so that resolved services are replaced with testable proxies that wrap and transparently forward calls to the original services.
The proxies can be configured to mock and spy on calls and be reset to their initial transparent state dynamically within a running process.

In its initial and reset state, DivertR aims to leave the system behaviour unchanged including maintaining the lifetime and disposal of the original services.
This enables a style of testing where each test starts by resetting the running system to its original state and then mocks out specific parts as required.

This approach is very effective when used in combination with Microsoft's [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests).
Tests are efficient and fast as dependencies can be mocked and reconfigured dynamically between tests without requiring reinitialisation ([code example here](https://github.com/devodo/DivertR/tree/main/test/DivertR.WebAppTests)).

# Feature Summary

1. Test double proxy framework for mocking, faking, stubbing, spying, etc.
2. Proxies that wrap and forward to root (original) services so tests run against the integrated system and only mock and spy on specific parts as needed.
3. Dynamic update and reset of proxies in a running application enabling changes between tests without requiring restart and initialisation overhead.
4. A lightweight, fluent interface for configuring proxies to redirect calls to delegates or substitute instances.
5. Proxy delegate chaining and relay back to the original root target.
6. Simple plugging of proxy factories into the dependency injection container by decorating and wrapping existing registrations.
7. Recording and verification of calls.
8. Leveraging .NET ValueTuple types for specifying named and strongly typed call arguments.

# Quickstart

* See the [quickstart guide](quickstart/) to get started.
* Your [feedback or comments](https://github.com/devodo/DivertR/discussions/43) are welcome.

