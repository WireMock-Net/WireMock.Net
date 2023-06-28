#if GRAPHQL
using System;
using FluentAssertions;
using HotChocolate.Language;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class GraphQLMatcherTests
{
    private const string TestSchema = @"
  input MessageInput {
    content: String
    author: String
  }

  type Message {
    id: ID!
    content: String
    author: String
  }

  type Mutation {
    createMessage(input: MessageInput): Message
    updateMessage(id: ID!, input: MessageInput): Message
  }

  type Query {
   greeting:String
   students:[Student]
   studentById(id:ID!):Student
  }

  type Student {
   id:ID!
   firstName:String
   lastName:String
   fullName:String 
  }";

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectQuery_IsMatch()
    {
        // Arrange
        var input = @"
{
  students {
    fullName
    id
  }
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectMutation_IsMatch()
    {
        // Arrange
        var input = @"mutation {
  createMessage(input: {
    author: ""stef"",
    content: ""test 123"",
  }) {
    id
  }
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_IncorrectQuery_IsMismatch()
    {
        // Arrange
        var input = @"
{
  students {
    fullName
    id
    abc
  }
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Mismatch);
    }

    [Fact]
    public void GraphQLMatcher_For_InvalidSchema_ThrowsSyntaxException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new GraphQLMatcher("in va lid");

        // Assert
        action.Should().Throw<SyntaxException>();
    }
}
#endif