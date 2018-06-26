using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class JsonObjectMatcherTests
    {
        [Fact]
        public void JsonObjectMatcher_GetName()
        {
            // Assign
            object obj = 1;

            // Act
            var matcher = new JsonObjectMatcher(obj);
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("JsonObjectMatcher");
        }

        [Fact]
        public void JsonObjectMatcher_IsMatch_ByteArray()
        {
            // Assign
            object checkValue = new byte[] { 1, 2 };

            // Act
            var matcher = new JsonObjectMatcher(new byte[] { 1, 2 });
            double result = matcher.IsMatch(checkValue);

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void JsonObjectMatcher_IsMatch_AcceptOnMatch()
        {
            // Assign
            object obj = new { x = 500, s = "s" };

            // Act
            var matcher = new JsonObjectMatcher(obj);
            double result = matcher.IsMatch(new { x = 500, s = "s" });

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void JsonObjectMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            object obj = new { x = 500, s = "s" };

            // Act
            var matcher = new JsonObjectMatcher(MatchBehaviour.RejectOnMatch, obj);
            double result = matcher.IsMatch(new { x = 500, s = "s" });

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }

        [Fact]
        public void JsonObjectMatcher_IsMatch_UsesValueEquality()
        {
            // Assign
            // Two instances that are not equal because they are not reference-equal.            
            var sample = new Thing { Name = "Test", Value = 42 };
            var value = new Thing { Name = "Test", Value = 42 };
            Check.That(value).IsNotEqualTo(sample); 

            // Act
            var matcher = new JsonObjectMatcher(value);
            double result = matcher.IsMatch(sample);

            // Assert
            Check.That(result).IsEqualTo(1.0); //They should match because JsonObjectMatcher uses value equality.            
        }


        // used for testing JsonObjectMatcher's use of value equality
        // (Reminder: Anonymous classes use value equality, but other objects by default use reference equality.)
        public class Thing
        {
            public string Name { get; set; }
            public int Value { get; set; }
        } 
    }
}