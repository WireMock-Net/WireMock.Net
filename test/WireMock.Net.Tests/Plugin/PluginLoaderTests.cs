using System;
using AnyOfTypes;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Plugin;
using Xunit;

namespace WireMock.Net.Tests.Plugin;

public class PluginLoaderTests
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
        var result = PluginLoader.Load<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, pattern);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Load_ByInterfaceAndFullName()
    {
        // Act
        var result = PluginLoader.LoadByFullName<IDummyInterfaceWithImplementation>(typeof(DummyClass).FullName);

        // Assert
        result.Should().BeOfType<DummyClass>();
    }

    [Fact]
    public void Load_ByInterface_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => PluginLoader.Load<IDummyInterfaceNoImplementation>();

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Plugin.PluginLoaderTests+IDummyInterfaceNoImplementation'.");
    }

    [Fact]
    public void Load_ByInterfaceAndFullName_ButNoImplementationFoundForInterface_ThrowsException()
    {
        // Act
        Action a = () => PluginLoader.LoadByFullName<IDummyInterfaceWithImplementation>("xyz");

        // Assert
        a.Should().Throw<DllNotFoundException>().WithMessage("No dll found which implements Interface 'WireMock.Net.Tests.Plugin.PluginLoaderTests+IDummyInterfaceWithImplementation' and has FullName 'xyz'.");
    }
}