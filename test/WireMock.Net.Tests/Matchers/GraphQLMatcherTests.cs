#if !(NET451 || NET452 || NET46 || NET461 || NETSTANDARD1_3)
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NFluent;
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
    public void GraphQLMatcher_For_ValidSchema_And_CorrectQuery_IsMatch_Positive()
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
    public void GraphQLMatcher_For_ValidSchema_And_CorrectMutation_IsMatch_Positive()
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
    public void GraphQLMatcher_For_ValidSchema_And_IncorrectQuery_IsMatch_Positive()
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
    public void GraphQLMatcher_For_InvalidSchema_And_CorrectQuery_IsMatch_Negative()
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
        var matcher = new GraphQLMatcher("in va lid");
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Mismatch);
    }
}
#endif