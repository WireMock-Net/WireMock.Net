using WireMock.Admin.Mappings;
using WireMock.Matchers.Request;
using WireMock.Server;

namespace WireMock;

/// <summary>
/// IMappingBuilder
/// </summary>
public interface IMappingBuilder
{
    /// <summary>
    /// The given.
    /// </summary>
    /// <param name="requestMatcher">The request matcher.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider Given(IRequestMatcher requestMatcher);

    /// <summary>
    /// Gets all the mappings as a list.
    /// </summary>
    /// <returns>A list from <see cref="MappingModel"/>s.</returns>
    MappingModel[] GetMappings();

    /// <summary>
    /// Convert all mappings to JSON.
    /// </summary>
    /// <returns>JSON</returns>
    string ToJson();

    /// <summary>
    /// Save all mappings as a single JSON to a file.
    /// </summary>
    /// <param name="path">The file to write to.</param>
    void SaveMappingsToFile(string path);

    /// <summary>
    /// Save all mappings as multiple JSON files (each file is 1 mapping).
    /// </summary>
    /// <param name="folder">The folder to write the files to.</param>
    void SaveMappingsToFiles(string folder);
}