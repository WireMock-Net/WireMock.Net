// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
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
    /// Builds a dictionary of ProtoDefinitions from a directory.
    /// - The key will be the filename without extension.
    /// - The value will be the ProtoDefinition with an extra comment with the relative path to each <c>.proto</c> file so it can be used by the WireMockProtoFileResolver.
    /// </summary>
    /// <param name="directory">The directory to start from.</param>
    public static ProtoDefinitionData FromDirectory(string directory)
    {
        Guard.NotNullOrEmpty(directory);

        var dictionary = new Dictionary<string, string>();
        var filePaths = Directory.EnumerateFiles(directory, "*.proto", SearchOption.AllDirectories);

        foreach (var filePath in filePaths)
        {
            // Get the relative path to the directory (note that this will be OS specific).
            var relativePath = PathUtils.GetRelativePath(directory, filePath);

            // Build comment and get content from file.
            var comment = $"// {relativePath}";
            var content = File.ReadAllText(filePath);

            // Only add the comment if it's not already defined.
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