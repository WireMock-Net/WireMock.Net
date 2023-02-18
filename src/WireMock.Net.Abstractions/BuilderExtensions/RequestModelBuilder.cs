// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// RequestModelBuilder
/// </summary>
public partial class RequestModelBuilder
{
    /// <summary>
    /// UsingConnect: add HTTP Method matching on `CONNECT`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingConnect() => WithMethods("CONNECT");

    /// <summary>
    /// UsingDelete: add HTTP Method matching on `DELETE`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingDelete() => WithMethods("DELETE");

    /// <summary>
    /// UsingGet: add HTTP Method matching on `GET`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingGet() => WithMethods("GET");

    /// <summary>
    /// UsingHead: Add HTTP Method matching on `HEAD`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingHead() => WithMethods("HEAD");

    /// <summary>
    /// UsingPost: add HTTP Method matching on `POST`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPost() => WithMethods("POST");

    /// <summary>
    /// UsingPatch: add HTTP Method matching on `PATCH`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPatch() => WithMethods("PATCH");

    /// <summary>
    /// UsingPut: add HTTP Method matching on `OPTIONS`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingOptions() => WithMethods("OPTIONS");

    /// <summary>
    /// UsingPut: add HTTP Method matching on `PUT`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPut() => WithMethods("PUT");

    /// <summary>
    /// UsingTrace: add HTTP Method matching on `TRACE`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingTrace() => WithMethods("TRACE");

    /// <summary>
    /// UsingAnyMethod: add HTTP Method matching on any method.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingAnyMethod() => this;
}