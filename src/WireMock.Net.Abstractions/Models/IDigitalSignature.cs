// Copyright © WireMock.Net

using System.Threading.Tasks;
using WireMock.Util;

namespace WireMock.Models;

/// <summary>
/// A delegate encapsulating logic for fetching or generating a private key.
/// </summary>
/// <returns>An async task containing the value of the private key.</returns>
public delegate Task<string> PrivateKeyCallback();

/// <summary>
/// A delegate encapsulating logic for fetching or generating a digital signature.
/// </summary>
/// <param name="privateKey">The value of the private key returned by <see cref="PrivateKeyCallback"/>.</param>
/// <param name="bodyData">The body data associated with the request.</param>
/// <returns>An async task containing the value of the digital signature.</returns>
public delegate Task<string> DigitalSignatureCallback(string privateKey, IBodyData? bodyData);

/// <summary>
/// An interface encapsulating logic for generating a digital signature.
/// </summary>
public interface IDigitalSignature
{
    /// <summary>
    /// The header name to use for the digital signature.
    /// </summary>
    string HeaderName { get; set; }

    /// <summary>
    /// The callback used to fetch or generate the private key.
    /// </summary>
    PrivateKeyCallback PrivateKeyCallback { get; set; }

    /// <summary>
    /// The callback used to fetch or generate the digital signature.
    /// </summary>
    DigitalSignatureCallback DigitalSignatureCallback { get; set; }
}