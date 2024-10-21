// Copyright © WireMock.Net

using System.Threading.Tasks;

namespace WireMock.Models;

/// <summary>
/// A class encapsulating logic for generating a digital signature.
/// </summary>
public class DigitalSignature : IDigitalSignature
{
    /// <inheritdoc/>
    public string HeaderName { get; set; } = "";

    /// <inheritdoc/>
    public PrivateKeyCallback PrivateKeyCallback { get; set; } = async () => await Task.FromResult("");

    /// <inheritdoc/>
    public DigitalSignatureCallback DigitalSignatureCallback { get; set; } = async (privateKey, bodyData) => await Task.FromResult("");
}
