#if USE_CSSCRIPT
#if NET46
using CSScriptLibrary;
#else
using csscript;
using CSScriptLib;
#endif
using JetBrains.Annotations;

namespace WireMock.Matchers
{
    internal class CSScriptMatcher : BaseScriptMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSScriptMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public CSScriptMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSScriptMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        public CSScriptMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] patterns) : base(matchBehaviour, patterns)
        {
        }

        protected override bool IsMatch(string input, string pattern)
        {
            string source = GetSource(pattern);

            dynamic script = CSScript.Evaluator.CompileCode(source).CreateObject("*");
            return (bool)script.IsMatch(pattern);
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public override string Name => "CSScriptMatcher";
    }
}
#endif