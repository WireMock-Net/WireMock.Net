// Copyright © WireMock.Net

#if PROTOBUF
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBufJsonConverter;
using Stef.Validation;

namespace WireMock.Util;

internal class WireMockProtoFileResolver : IProtoFileResolver
{
    private readonly Dictionary<string, string> _files = new();

    public WireMockProtoFileResolver(IReadOnlyCollection<string> protoDefinitions)
    {
        if (Guard.NotNullOrEmpty(protoDefinitions).Count() > 1)
        {
            foreach (var extraProtoDefinition in protoDefinitions.Skip(1))
            {
                var firstNonEmptyLine = extraProtoDefinition.Split(['\r', '\n']).FirstOrDefault(l => !string.IsNullOrEmpty(l));
                if (firstNonEmptyLine != null)
                {
                    _files.Add(firstNonEmptyLine.TrimStart(['\r', '\n', '/', ' ']), extraProtoDefinition);
                }
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
}
#endif