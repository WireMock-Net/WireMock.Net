// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
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
    /// And adds a comment with the relative path to each <c>.proto</c> file so it can be used by the <see cref="WireMockProtoFileResolver"/>
    /// </summary>
    /// <param name="directory">The directory to start from.</param>
    public static IReadOnlyList<string> FromDirectory(string directory)
    {
        var modifiedFiles = new List<string>();
        var files = Directory.EnumerateFiles(directory, "*.proto", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relativePath = PathUtils.GetRelativePath(directory, file);
            var comment = $"// {relativePath}";
            var content = File.ReadAllText(file);

            // Only add the comment if it's not already there.
            var modifiedContent = !content.StartsWith(comment) ? $"{comment}\n{content}" : content;

            modifiedFiles.Add(modifiedContent);
        }

        return modifiedFiles;
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