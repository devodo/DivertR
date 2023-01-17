---
title: Home
layout: home
nav_order: 1
---

# DivertR

Welcome to DivertR! The .NET mocking framework for integration testing.

Why yet another mocking framework?

DivertR provides standard mocking capability similar to other well known frameworks like [Moq](https://github.com/moq/moq4)
but it goes further, building on the shoulders of giants and amazing tools like WebApplicationFactory to enable a new way of testing wired-up, integrated systems.

This is achieved by replacing existing dependencies with proxy test doubles that forward calls to the originals.
These proxies can be manipulated and reset dynamically within a running process e.g. to mock behaviour and spy on interactions.
In their initial and reset state the proxies transparently relay calls, leaving the original system behaviour unchanged.
This enables a style of testing where each test starts by resetting the running system to its original state and then mocks out specific parts as required.

Tests are efficient and fast as the system under test can have its external dependencies mocked and (assuming it is stateless) be reconfigured dynamically between tests without requiring restart.

As a specific example, DivertR has been used effectively when combined with Microsoft's WebApplicationFactory (TestServer) for testing web applications.
A [code sample](https://github.com/devodo/DivertR/tree/main/test/DivertR.WebAppTests) of this usage is provided.

# Feature Summary

1. Test double proxy framework for mocking, faking, stubbing, spying, etc.
2. Proxies that wrap and forward to root (original) instances so tests run against the integrated system whilst modifying and spying on specific parts as needed.
3. A lightweight, fluent interface for configuring proxies to redirect calls to delegates or substitute instances.
4. Dynamic update and reset of proxies in a running application enabling changes between tests without requiring restart and initialisation overhead.
5. Leveraging .NET ValueTuple types for specifying named and strongly typed call arguments that can be passed and reused e.g. in call verifications.
6. Method call interception and diversion with optional relay back to the original root target.
7. Simple plugging of proxy factories into the dependency injection container by decorating and wrapping existing registrations.
8. Recording and verifying proxy calls.

# Quickstart

* See the [quickstart guide](quickstart/) to get started.
* Your [feedback or comments](https://github.com/devodo/DivertR/discussions/43) are welcome.

