// Copyright © WireMock.Net

using System;
using System.IO;
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

    public interface IDummyInterfaceWithImplementationUsedForStaticTest
    {
    }

    public class DummyClass1UsedForStaticTest : IDummyInterfaceWithImplementationUsedForStaticTest
    {
        public DummyClass1UsedForStaticTest(Counter counter)
        {
            counter.AddOne();
        }
    }

    public class DummyClass2UsedForStaticTest : IDummyInterfaceWithImplementationUsedForStaticTest
    {
        public DummyClass2UsedForStaticTest(Counter counter)
        {
            counter.AddOne();
        }
    }

    public class Counter
    {
        public int Value { get; private set; }

        public void AddOne()
        {
            Value++;
        }
    }

    [Fact]
    public void LoadNewInstance()
    {
        var current = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(Path.GetTempPath());

            // Act
            AnyOf<string, StringPattern> pattern = "x";
            var result = TypeLoader.LoadNewInstance<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, pattern);

            // Assert
            result.Should().NotBeNull();
        }
        finally
        {
            Directory.SetCurrentDirectory(current);
        }
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
    public void LoadStaticInstance_ShouldOnlyCreateInstanceOnce()
    {
        // Arrange
        var counter = new Counter();

        // Act
        var result = TypeLoader.LoadStaticInstance<IDummyInterfaceWithImplementationUsedForStaticTest>(counter);
        TypeLoader.LoadStaticInstance<IDummyInterfaceWithImplementationUsedForStaticTest>(counter);

        // Assert
        result.Should().BeOfType<DummyClass1UsedForStaticTest>();
        counter.Value.Should().Be(1);
    }

    [Fact]
    public void LoadStaticInstanceByFullName_ShouldOnlyCreateInstanceOnce()
    {
        // Arrange
        var counter = new Counter();
        var fullName = typeof(DummyClass2UsedForStaticTest).FullName!;

        // Act
        var result = TypeLoader.LoadStaticInstanceByFullName<IDummyInterfaceWithImplementationUsedForStaticTest>(fullName, counter);
        TypeLoader.LoadStaticInstanceByFullName<IDummyInterfaceWithImplementationUsedForStaticTest>(fullName, counter);

        // Assert
        result.Should().BeOfType<DummyClass2UsedForStaticTest>();
        counter.Value.Should().Be(1);
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