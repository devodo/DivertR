---
layout: default
title: About
nav_order: 4
---

# About

DivertR is similar to other well known mocking frameworks but provides additional features for use with integration testing such as dynamically manipulating the dependency injection (DI) layer at runtime.
You can redirect calls to test doubles, such as substitute instances, mocks or delegates, and then optionally relay them back to the original services.

Many developers are already enjoying the benefits of in-process component/integration testing using Microsoft's [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
which also lets you customise the DI configuration, e.g. to substitute test doubles, but this can only be done once (per TestServer instantiation).

DivertR was born out of the need to efficiently run tests against integrated systems.
It has grown into a framework that facilitates testing of wired up systems, bringing a familiar unit/mocking testing style into the realm of component and integration testing,
by providing features to conveniently substitute dependency behaviour (including error conditions) and verify inputs and outputs from recorded call information.

