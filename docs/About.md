# About

DivertR is similar to well known mocking frameworks like Moq or FakeItEasy but provides additional features for use with integration testing such as dynamically manipulating the dependency injection (DI) layer at runtime.
You can redirect calls to test doubles, such as substitute instances, mocks or delegates, and then optionally relay them back to the original services.

Many developers are already enjoying the benefits of in-process component/integration testing using Microsoft's [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
which also lets you customise the DI configuration, e.g. to substitute test doubles, but this can only be done once (per TestServer instantiation).

DivertR was born out of the need to efficiently modify DI configurations between tests running against the same TestServer instance.
It has grown into a framework that facilitates testing of wired up systems, bringing a familiar unit/mocking testing style into the realm of component and integration testing,
by providing features to conveniently substitute dependency behaviour (including error conditions) and verify inputs and outputs from recorded call information.

# DispatchProxy

The default DivertR proxy factory is implemented using [System.Reflection.DispatchProxy](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy). As this is part of the .NET Standard 2.1, DivertR therefore does not have any dependencies on external libraries however it is limited to proxying interface types only.

If class proxies are required, [DivertR.DynamicProxy](../src/DivertR.DynamicProxy/README.md) is an alternative proxy factory implemented using [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/) that does support classes.
However, when proxying classes, care should be taken as only calls to non-virtual members cannot be intercepted, this can cause inconsistent behaviour e.g. when wrapping root instances.
