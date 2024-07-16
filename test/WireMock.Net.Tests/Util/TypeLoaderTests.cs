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
    public void Load_ByInterface()
    {
        // Act
        AnyOf<string, StringPattern> pattern = "x";
        var result = TypeLoader.Load<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, pattern);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Load_ByInterfaceAndFullName()
    {
        // Act
        var result = TypeLoader.LoadByFullName<IDummyInterfaceWithImplementation>(typeof(DummyClass).FullName!);

        // Assert
        result.Should().BeOfType<DummyClass>();
    }

    [Fact]
    public void Load_ByInterface_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => TypeLoader.Load<IDummyInterfaceNoImplementation>();

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Util.TypeLoaderTests+IDummyInterfaceNoImplementation'.");
    }

    [Fact]
    public void Load_ByInterfaceAndFullName_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => TypeLoader.LoadByFullName<IDummyInterfaceWithImplementation>("xyz");

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Util.TypeLoaderTests+IDummyInterfaceWithImplementation' and has FullName 'xyz'.");
    }
}