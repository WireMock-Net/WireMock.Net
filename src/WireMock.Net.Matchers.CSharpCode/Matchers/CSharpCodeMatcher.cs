using System;
using System.Linq;
using System.Reflection;
using System.Text;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// CSharpCode / CS-Script Matcher
/// </summary>
/// <inheritdoc cref="ICSharpCodeMatcher"/>
internal class CSharpCodeMatcher : ICSharpCodeMatcher
{
    private const string TemplateForIsMatchWithString = "public class CodeHelper {{ public bool IsMatch(string it) {{ {0} }} }}";

    private const string TemplateForIsMatchWithDynamic = "public class CodeHelper {{ public bool IsMatch(dynamic it) {{ {0} }} }}";

    private readonly string[] _usings =
    {
        "System",
        "System.Linq",
        "System.Collections.Generic",
        "Microsoft.CSharp",
        "Newtonsoft.Json.Linq"
    };

    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IMatcher.ThrowException"/>
    public bool ThrowException { get; }

    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public CSharpCodeMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public CSharpCodeMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator = MatchOperator.Or, params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        ThrowException = false;
        MatchOperator = matchOperator;
    }

    public double IsMatch(string input)
    {
        return IsMatchInternal(input);
    }

    public double IsMatch(object? input)
    {
        return IsMatchInternal(input);
    }

    public double IsMatchInternal(object? input)
    {
        double match = MatchScores.Mismatch;

        if (input != null)
        {
            match = MatchScores.ToScore(_patterns.Select(pattern => IsMatch(input, pattern.GetPattern())).ToArray(), MatchOperator);
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, match);
    }

    private bool IsMatch(dynamic input, string pattern)
    {
        bool isMatchWithString = input is string;
        var inputValue = isMatchWithString ? input : JObject.FromObject(input);
        string source = GetSourceForIsMatchWithString(pattern, isMatchWithString);

        object? result;

#if (NET451 || NET452)
        var compilerParams = new System.CodeDom.Compiler.CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            ReferencedAssemblies =
            {
                "System.dll",
                "System.Core.dll",
                "Microsoft.CSharp.dll",
                "Newtonsoft.Json.dll"
            }
        };

        using (var codeProvider = new Microsoft.CSharp.CSharpCodeProvider())
        {
            var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParams, source);

            if (compilerResults.Errors.Count != 0)
            {
                var errors = from System.CodeDom.Compiler.CompilerError er in compilerResults.Errors select er.ToString();
                throw new WireMockException(string.Join(", ", errors));
            }

            var helper = compilerResults.CompiledAssembly?.CreateInstance("CodeHelper");
            if (helper == null)
            {
                throw new WireMockException("CSharpCodeMatcher: Unable to create instance from WireMock.CodeHelper");
            }

            var methodInfo = helper.GetType().GetMethod("IsMatch");
            if (methodInfo == null)
            {
                throw new WireMockException("CSharpCodeMatcher: Unable to find method 'IsMatch' in WireMock.CodeHelper");
            }

            try
            {
                result = methodInfo.Invoke(helper, new[] { inputValue });
            }
            catch (Exception ex)
            {
                throw new WireMockException("CSharpCodeMatcher: Unable to call method 'IsMatch' in WireMock.CodeHelper", ex);
            }
        }
#elif (NET46 || NET461)
            dynamic script;
            try
            {
                script = CSScriptLibrary.CSScript.Evaluator.CompileCode(source).CreateObject("*");
            }
            catch (Exception ex)
            {
                throw new WireMockException("CSharpCodeMatcher: Unable to create compiler for WireMock.CodeHelper", ex);
            }
            
            try
            {
                result = script.IsMatch(inputValue);
            }
            catch (Exception ex)
            {
                throw new WireMockException("CSharpCodeMatcher: Problem calling method 'IsMatch' in WireMock.CodeHelper", ex);
            }

#elif (NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1 || NET5_0 || NET6_0)
            Assembly assembly;
            try
            {
                assembly = CSScriptLib.CSScript.Evaluator.CompileCode(source);
            }
            catch (Exception ex)
            {
                throw new WireMockException($"CSharpCodeMatcher: Unable to compile code `{source}` for WireMock.CodeHelper", ex);
            }

            dynamic script;
            try
            {
                script = CSScripting.ReflectionExtensions.CreateObject(assembly, "*");
            }
            catch (Exception ex)
            {
                throw new WireMockException("CSharpCodeMatcher: Unable to create object from assembly", ex);
            }

            try
            {
                result = script.IsMatch(inputValue);
            }
            catch (Exception ex)
            {
                throw new WireMockException("CSharpCodeMatcher: Problem calling method 'IsMatch' in WireMock.CodeHelper", ex);
            }
#else
            throw new NotSupportedException("The 'CSharpCodeMatcher' cannot be used in netstandard 1.3");
#endif
        try
        {
            return (bool)result;
        }
        catch
        {
            throw new WireMockException($"Unable to cast result '{result}' to bool");
        }
    }

    private string GetSourceForIsMatchWithString(string pattern, bool isMatchWithString)
    {
        string template = isMatchWithString ? TemplateForIsMatchWithString : TemplateForIsMatchWithDynamic;

        var stringBuilder = new StringBuilder();
        foreach (string @using in _usings)
        {
            stringBuilder.AppendLine($"using {@using};");
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat(template, pattern);

        return stringBuilder.ToString();
    }

    /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "CSharpCodeMatcher";
}