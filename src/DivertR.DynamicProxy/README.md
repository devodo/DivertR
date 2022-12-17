# DivertR DynamicProxy

[![nuget](https://img.shields.io/nuget/v/DivertR.DynamicProxy.svg)](https://www.nuget.org/packages/DivertR.DynamicProxy)
[![build](https://github.com/devodo/DivertR/actions/workflows/build.yml/badge.svg)](https://github.com/devodo/DivertR/actions/workflows/build.yml)

A [DivertR](https://github.com/devodo/DivertR) proxy factory implementation that supports proxying class types using [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/).

# Installing

Install DivertR as a [NuGet package](https://www.nuget.org/packages/DivertR):

```sh
Install-Package DivertR
```

Or redirect the .NET command line interface:

```sh
dotnet add package DivertR
```

# Example Usage

Configure DivertR to use DynamicProxy:

```csharp
DiverterSettings.Global = new DiverterSettings(proxyFactory: new DynamicProxyFactory());
```
