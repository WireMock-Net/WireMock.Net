#if GRAPHQL
using System;
using FluentAssertions;
using GraphQLParser.Exceptions;
using WireMock.Matchers;
using WireMock.Models;
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
        var input = "{\"query\":\"{\\r\\n students {\\r\\n fullName\\r\\n id\\r\\n }\\r\\n}\"}";

        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(TestSchema);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_CorrectGraphQLQuery_IsMatch()
    {
        // Arrange
        var input = @"{
	""query"": ""query ($sid: ID!)\r\n{\r\n studentById(id: $sid) {\r\n fullName\r\n id\r\n }\r\n}"",
	""variables"": {
		""sid"": ""1""
	}
}";
        // Act
        var matcher = new GraphQLMatcher(TestSchema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Perfect);

        matcher.GetPatterns().Should().Contain(TestSchema);
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
    public void GraphQLMatcher_For_ValidSchemaAsStringPattern_And_CorrectQuery_IsMatch()
    {
        // Arrange
        var input = "{\"query\":\"{\\r\\n students {\\r\\n fullName\\r\\n id\\r\\n }\\r\\n}\"}";
        var schema = new StringPattern
        {
            Pattern = TestSchema
        };

        // Act
        var matcher = new GraphQLMatcher(schema);
        var result = matcher.IsMatch(input);

        // Assert
        result.Should().Be(MatchScores.Perfect);
    }

    [Fact]
    public void GraphQLMatcher_For_ValidSchema_And_IncorrectQueryWithError_WithThrowExceptionTrue_ThrowsException()
    {
        // Arrange
        var input = "{\"query\":\"{\\r\\n studentsX {\\r\\n fullName\\r\\n X\\r\\n }\\r\\n}\"}";

        // Act
        var matcher = new GraphQLMatcher(TestSchema, MatchBehaviour.AcceptOnMatch, true);
        Action action = () => matcher.IsMatch(input);

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void GraphQLMatcher_For_InvalidSchema_ThrowsGraphQLSyntaxErrorException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new GraphQLMatcher("in va lid");

        // Assert
        action.Should().Throw<GraphQLSyntaxErrorException>();
    }
}
#endif