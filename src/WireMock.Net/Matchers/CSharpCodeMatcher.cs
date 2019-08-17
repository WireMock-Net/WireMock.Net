using System.Linq;
using JetBrains.Annotations;
using WireMock.Exceptions;
using WireMock.Validation;

namespace WireMock.Matchers
{
    internal class CSharpCodeMatcher : IStringMatcher
    {
        protected const string Framework = "{0}; namespace {1} {{ public class CodeHelper {{ public bool IsMatch(string it) {{ {1} }} }} }}";

        private readonly string[] _usings =
        {
            "System",
            "System.Linq",
            "System.Collections.Generic"
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
            double match = MatchScores.Mismatch;

            if (input != null)
            {
                match = MatchScores.ToScore(_patterns.Select(p => IsMatch(input, p)));
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        private bool IsMatch(string input, string pattern)
        {
            string source = GetSource(pattern);

#if NET451 || NET452
            var compilerParams = new System.CodeDom.Compiler.CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                ReferencedAssemblies =
                {
                    "System.dll"
                }
            };

            using (var codeProvider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParams, source);

                if (compilerResults.Errors.Count != 0)
                {
                    var errors = from object er in compilerResults.Errors select er.ToString();
                    throw new WireMockException(string.Join(", ", errors));
                }

                object helper = compilerResults.CompiledAssembly.CreateInstance("WireMock.CodeHelper");
                if (helper == null)
                {
                    throw new WireMockException("Unable to create instance from WireMock.CodeHelper");
                }

                var methodInfo = helper.GetType().GetMethod("IsMatch");
                if (methodInfo == null)
                {
                    throw new WireMockException("Unable to find method 'IsMatch' in WireMock.CodeHelper");
                }

                return (bool)methodInfo.Invoke(helper, new object[] { input });
            }
#elif NET46 || NET461
            try
            {
                dynamic script = CSScriptLibrary.CSScript.Evaluator.CompileCode(source).CreateObject("*");
                return (bool)script.IsMatch(input);
            }
            catch
            {
                throw new WireMockException("Unable to create compile and execute code from WireMock.CodeHelper");
            }
#else
            try
            {
                var assembly = CSScriptLib.CSScript.Evaluator.CompileCode(source);
                dynamic script = csscript.GenericExtensions.CreateObject(assembly, "*");
                return (bool)script.IsMatch(input);
            }
            catch
            {
                throw new WireMockException("Unable to create compile and execute code from WireMock.CodeHelper");
            }
#endif
        }

        private string GetSource(string pattern)
        {
            return string.Format(Framework, string.Join(";", _usings.Distinct().Select(u => "using " + u)), pattern);
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