namespace WireMock.Models;

public readonly struct IdOrText
{
    public string? Id { get; }

    public string Text { get; }

    public string Val => Id ?? Text;

    public IdOrText(string? id, string text)
    {
        Id = id;
        Text = text;
    }
}