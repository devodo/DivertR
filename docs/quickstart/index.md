---
layout: default
title: Quickstart
nav_order: 2
has_children: true
---

# Quickstart

## Installing

Install DivertR as a [NuGet package](https://www.nuget.org/packages/DivertR):

```sh
Install-Package DivertR
```

Or via the .NET command line interface:

```sh
dotnet add package DivertR
```

## Overview

DivertR is a proxy framework for creating and configuring test doubles like mocks.

1. The `Redirect` class is used to create proxies.
2. Configure proxy behaviour by adding `Vias` to the `Redirect`.
3. Delete `Vias` to `Reset` proxy behaviour between tests.

```csharp
[Fact]
public async Task GivenFooExistsInRepo_WhenGetFoo_ThenReturnsFoo_WithOk200()
{
    // ARRANGE
    var foo = new Foo
    {
        Id = Guid.NewGuid(),
        Name = "Foo123"
    };

    _diverter
        .Redirect<IFooRepository>() // Redirect IFooRepository calls 
        .To(x => x.GetFooAsync(foo.Id)) // matching this method and argument
        .Via(() => Task.FromResult(foo)); // via this delegate

    // ACT
    var response = await _fooClient.GetFooAsync(foo.Id);
    
    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    response.Content.Id.ShouldBe(foo.Id);
    response.Content.Name.ShouldBe(foo.Name);
}

[Fact]
public async Task GivenFooRepoException_WhenGetFoo_ThenReturns500InternalServerError()
{
    // ARRANGE
    _diverter
        .Redirect<IFooRepository>()
        .To(x => x.GetFooAsync(Is<Guid>.Any))
        .Via(() => throw new Exception());

    // ACT
    var response = await _fooClient.GetFooAsync(Guid.NewGuid());

    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
}
```

