// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// RequestModelBuilder
/// </summary>
public partial class RequestModelBuilder
{
    /// <summary>
    /// UsingConnect: add HTTP Method matching on `CONNECT` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingConnect(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingDelete: add HTTP Method matching on `DELETE` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingGet: add HTTP Method matching on `GET` and matchBehaviour (optional).
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    /// IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
    public RequestModelBuilder UsingGet() => WithMethods("GET");

    /// <summary>
    /// UsingHead: Add HTTP Method matching on `HEAD` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPost: add HTTP Method matching on `POST` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPatch: add HTTP Method matching on `PATCH` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPut: add HTTP Method matching on `OPTIONS` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingOptions(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPut: add HTTP Method matching on `PUT` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingTrace: add HTTP Method matching on `TRACE` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    //public RequestModelBuilder UsingTrace(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingAnyMethod: add HTTP Method matching on any method.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingAnyMethod() => this;
}