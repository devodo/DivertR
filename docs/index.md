---
title: Home
layout: home
nav_order: 1
---

# DivertR

Welcome to DivertR! The .Net mocking framework for integration testing.

DivertR provides standard mocking capability similar to other well known frameworks like [Moq](https://github.com/moq/moq4) - but why yet another .Net mocking framework?

It is specifically designed for testing wired-up systems and enables a style of testing where each test starts with the system in its original, fully integrated state and then mocks out specific parts as required.

This is achieved by replacing dependencies with proxies that wrap and transparently forward calls to the originals.
The proxies are then manipulated and reset between tests and are able to e.g. mock behaviour and spy on interactions.

DivertR proxies can be manipulated dynamically at run-time within a running process.
Tests are therefore efficient and fast as the system under test can be reconfigured dynamically between tests without requiring restart.

# Feature Summary

1. Test double proxy framework for mocking, faking, stubbing, spying, etc.
2. Proxies that wrap and forward to root (original) instances so tests run against the integrated system whilst modifying and spying on specific parts as needed.
3. A lightweight, fluent interface for configuring proxies to redirect calls to delegates or substitute instances.
4. Dynamic update and reset of proxies in a running application enabling changes between tests without requiring restart and initialisation overhead.
5. Leveraging .NET ValueTuple types for specifying named and strongly typed call arguments that can be passed and reused e.g. in call verifications.
6. Method call interception and diversion with optional relay back to the original target.
7. Simple plugging of proxy factories into the dependency injection container by decorating and wrapping existing registrations.
8. Recording and verifying proxy calls.

# Quickstart

* See the [quickstart guide](quickstart/) to get started.
* Your [feedback or comments](https://github.com/devodo/DivertR/discussions/43) are welcome.

