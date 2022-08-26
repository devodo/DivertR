# About

DivertR is similar to well known mocking frameworks like Moq or FakeItEasy but provides additional features for dynamically manipulating the dependency injection (DI) layer at runtime.
You can redirect dependency calls to test doubles, such as substitute instances, mocks or delegates, and then optionally relay them back to the original services.

Many developers are already enjoying the benefits of in-process component/integration testing using Microsoft's [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
which also lets you customise the DI configuration, e.g. to substitute test doubles, but this can only be done once (per TestServer instantiation).

DivertR was born out of the need to efficiently modify DI configurations between tests running against the same TestServer instance.
It has grown into a framework that facilitates testing of wired up systems, bringing a familiar unit/mocking testing style into the realm of component and integration testing,
by providing features to conveniently substitute dependency behaviour (including error conditions) and verify inputs and outputs from recorded call information.

# Interfaces Only

DivertR uses the .NET Standard [DispatchProxy](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy) to build proxies and this is limited to interface types only.
Although other proxy generators that support classes such as DynamicProxy can be used, calls to non-virtual members cannot be intercepted and this can cause inconsistent behaviour e.g. when wrapping root instances.
