using WireMock.Matchers;

namespace WireMock.Settings;

public class ProxySaveMappingSetting<T>
{
    public MatchBehaviour MatchBehaviour { get; } = MatchBehaviour.AcceptOnMatch;

    public T Value { get; }

    public ProxySaveMappingSetting(T value, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Value = value;
        MatchBehaviour = matchBehaviour;
    }

    public static implicit operator ProxySaveMappingSetting<T>(T value) => new(value);

    public static implicit operator T(ProxySaveMappingSetting<T> @this) => @this.Value;
}