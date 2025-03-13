// Copyright © WireMock.Net

#if PROTOBUF
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ProtoBufJsonConverter;
using Stef.Validation;

namespace WireMock.Util;

/// <summary>
/// This resolver is used to resolve the extra ProtoDefinition files.
/// It assumes that:
/// - the first ProtoDefinition file is the main ProtoDefinition file.
/// - the first commented line of each extra ProtoDefinition file is the filename which is used in the import of the other ProtoDefinition file(s).
/// </summary>
internal class WireMockProtoFileResolver : IProtoFileResolver
{
    private readonly Dictionary<string, string> _files = new();

    public WireMockProtoFileResolver(IReadOnlyCollection<string> protoDefinitions)
    {
        if (Guard.NotNullOrEmpty(protoDefinitions).Count() <= 1)
        {
            return;
        }

        foreach (var extraProtoDefinition in protoDefinitions.Skip(1))
        {
            var firstNonEmptyLine = extraProtoDefinition.Split(['\r', '\n']).FirstOrDefault(l => !string.IsNullOrEmpty(l));
            if (firstNonEmptyLine != null && TryGetValidFileName(firstNonEmptyLine.TrimStart(['/', ' ']), out var validFileName))
            {
                _files.Add(validFileName, extraProtoDefinition);
            }
        }
    }

    public bool Exists(string path)
    {
        return _files.ContainsKey(path);
    }

    public TextReader OpenText(string path)
    {
        if (_files.TryGetValue(path, out var extraProtoDefinition))
        {
            return new StringReader(extraProtoDefinition);
        }

        throw new FileNotFoundException($"The ProtoDefinition '{path}' was not found.");
    }

    private static bool TryGetValidFileName(string fileName, [NotNullWhen(true)] out string? validFileName)
    {
        if (!fileName.Any(c => Path.GetInvalidPathChars().Contains(c)))
        {
            validFileName = fileName;
            return true;
        }

        validFileName = null;
        return false;
    }
}
#endif