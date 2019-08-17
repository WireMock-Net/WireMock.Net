using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Exceptions;
using WireMock.Validation;

namespace WireMock.Matchers
{
    internal class CSharpCodeMatcher : IObjectMatcher
    {
        private const string TemplateForIsMatchWithString = "{0} public class CodeHelper {{ public bool IsMatch(string it) {{ {1} }} }}";

        private const string TemplateForIsMatchWithJObject = "{0} public class CodeHelper {{ public bool IsMatch(dynamic it) {{ {1} }} }}";

        private readonly string[] _usings =
        {
            "System",
            "System.Linq",
            "System.Collections.Generic",
            "Microsoft.CSharp",
            "Newtonsoft.Json.Linq"
        };

        public MatchBehaviour MatchBehaviour { get; }

        private readonly string[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public CSharpCodeMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        public CSharpCodeMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
            _patterns = patterns;
        }

        public double IsMatch(string input)
        {
            return IsMatchInternal(input);
        }

        public double IsMatch(object input)
        {
            return IsMatchInternal(input);
        }

        public double IsMatchInternal(object input)
        {
            double match = MatchScores.Mismatch;

            if (input != null)
            {
                match = MatchScores.ToScore(_patterns.Select(pattern => IsMatch(input, pattern)));
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        private bool IsMatch(dynamic input, string pattern)
        {
            bool isMatchWithString = input is string;
            var inputValue = isMatchWithString ? input : JObject.FromObject(input);
            string source = GetSourceForIsMatchWithString(pattern, isMatchWithString);

#if NET451 || NET452
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

                object helper = compilerResults.CompiledAssembly.CreateInstance("CodeHelper");
                if (helper == null)
                {
                    throw new WireMockException("Unable to create instance from CodeHelper");
                }

                var methodInfo = helper.GetType().GetMethod("IsMatch");
                if (methodInfo == null)
                {
                    throw new WireMockException("Unable to find method 'IsMatch' in CodeHelper");
                }
                
                return (bool)methodInfo.Invoke(helper, new[] { inputValue });
            }
#elif NET46 || NET461
            try
            {
                dynamic script = CSScriptLibrary.CSScript.Evaluator.CompileCode(source).CreateObject("*");
                return (bool)script.IsMatch(inputValue);
            }
            catch
            {
                throw new WireMockException("Unable to create compile and execute code from WireMock.CodeHelper");
            }
#elif NETSTANDARD2_0
            try
            {
                var assembly = CSScriptLib.CSScript.Evaluator.CompileCode(source);
                dynamic script = csscript.GenericExtensions.CreateObject(assembly, "*");
                return (bool)script.IsMatch(inputValue);
            }
            catch (Exception ex)
            {
                throw new WireMockException("Unable to create compiler and execute code for WireMock.CodeHelper", ex);
            }
#else
            throw new NotSupportedException("The 'CSharpCodeMatcher' cannot be used in netstandard 1.3");
#endif
        }

        private string GetSourceForIsMatchWithString(string pattern, bool isMatchWithString)
        {
            string template = isMatchWithString ? TemplateForIsMatchWithString : TemplateForIsMatchWithJObject;
            return string.Format(template, string.Join(Environment.NewLine, _usings.Select(u => $"using {u};")), pattern);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "CSharpCodeMatcher";
    }
}