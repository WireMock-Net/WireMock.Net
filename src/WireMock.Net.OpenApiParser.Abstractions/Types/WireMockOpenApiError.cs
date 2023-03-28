namespace WireMock.Net.OpenApiParser.Abstractions.Types;

/// <summary>
/// Error related to the Open API Document.
/// </summary>
public class WireMockOpenApiError
{
    /// <summary>
    /// Initializes the <see cref="WireMockOpenApiError"/> class.
    /// </summary>
    public WireMockOpenApiError(string pointer, string message)
    {
        Pointer = pointer;
        Message = message;
    }

    /// <summary>
    /// Message explaining the error.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Pointer to the location of the error.
    /// </summary>
    public string Pointer { get; set; }

    /// <summary>
    /// Gets the string representation of <see cref="WireMockOpenApiError"/>.
    /// </summary>
    public override string ToString()
    {
        return Message + (!string.IsNullOrEmpty(Pointer) ? " [" + Pointer + "]" : "");
    }
}