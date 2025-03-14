// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stef.Validation;
using WireMock.Models;
using WireMock.Settings;

namespace WireMock.Util;

/// <summary>
/// Some helper methods for Proto Definitions.
/// </summary>
public static class ProtoDefinitionHelper
{
    /// <summary>
    /// Builds a list of proto definitions from a directory.
    /// And adds a comment with the relative path to each <c>.proto</c> file so it can be used by the WireMockProtoFileResolver.
    /// </summary>
    /// <param name="directory">The directory to start from.</param>
    public static ProtoDefinitionData FromDirectory(string directory)
    {
        Guard.NotNullOrEmpty(directory);

        var dictionary = new Dictionary<string, string>();
        var filePaths = Directory.EnumerateFiles(directory, "*.proto", SearchOption.AllDirectories);

        foreach (var filePath in filePaths)
        {
            var relativePath = PathUtils.GetRelativePath(directory, filePath);
            var comment = $"// {relativePath}";
            var content = File.ReadAllText(filePath);

            // Only add the comment if it's not already there.
            var modifiedContent = !content.StartsWith(comment) ? $"{comment}\n{content}" : content;
            var key = Path.GetFileNameWithoutExtension(filePath);
            dictionary.Add(key, modifiedContent);
        }

        return new ProtoDefinitionData(dictionary);
    }

    internal static IdOrTexts GetIdOrTexts(WireMockServerSettings settings, params string[] protoDefinitionOrId)
    {
        switch (protoDefinitionOrId.Length)
        {
            case 1:
                var idOrText = protoDefinitionOrId[0];
                if (settings.ProtoDefinitions?.TryGetValue(idOrText, out var protoDefinitions) == true)
                {
                    return new(idOrText, protoDefinitions);
                }

                return new(null, protoDefinitionOrId);

            default:
                return new(null, protoDefinitionOrId);
        }
    }
}

public class ProtoDefinitionData
{
    private readonly IDictionary<string, string> _dictionary;

    internal ProtoDefinitionData(IDictionary<string, string> dictionary)
    {
        _dictionary = dictionary;
    }

    public IReadOnlyList<string> ToList(string mainProtoFilename)
    {
        Guard.NotNullOrEmpty(mainProtoFilename);

        if (!_dictionary.TryGetValue(mainProtoFilename, out var mainProtoDefinition))
        {
            throw new KeyNotFoundException($"The ProtoDefinition with filename '{mainProtoFilename}' was not found.");
        }

        var list = new List<string> { mainProtoDefinition };
        list.AddRange(_dictionary.Where(kvp => kvp.Key != mainProtoFilename).Select(kvp => kvp.Value));
        return list;
    }
}