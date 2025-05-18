// Copyright © WireMock.Net

using System;
using AnyOfTypes;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class TypeLoaderTests
{
    public interface IDummyInterfaceNoImplementation
    {
    }

    public interface IDummyInterfaceWithImplementation
    {
    }

    public class DummyClass : IDummyInterfaceWithImplementation
    {
    }

    [Fact]
    public void LoadNewInstance()
    {
        // Act
        AnyOf<string, StringPattern> pattern = "x";
        var result = TypeLoader.LoadNewInstance<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, pattern);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void LoadNewInstanceByFullName()
    {
        // Act
        var result = TypeLoader.LoadNewInstanceByFullName<IDummyInterfaceWithImplementation>(typeof(DummyClass).FullName!);

        // Assert
        result.Should().BeOfType<DummyClass>();
    }

    [Fact]
    public void LoadStaticInstance()
    {
        // Act
        var result = TypeLoader.LoadStaticInstance<IMimeKitUtils>();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void LoadStaticInstanceByFullName()
    {
        // Act
        var result = TypeLoader.LoadStaticInstanceByFullName<IDummyInterfaceWithImplementation>(typeof(DummyClass).FullName!);

        // Assert
        result.Should().BeOfType<DummyClass>();
    }

    [Fact]
    public void LoadNewInstance_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => TypeLoader.LoadNewInstance<IDummyInterfaceNoImplementation>();

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Util.TypeLoaderTests+IDummyInterfaceNoImplementation'.");
    }

    [Fact]
    public void LoadNewInstanceByFullName_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => TypeLoader.LoadNewInstanceByFullName<IDummyInterfaceWithImplementation>("xyz");

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Util.TypeLoaderTests+IDummyInterfaceWithImplementation' and has FullName 'xyz'.");
    }
}