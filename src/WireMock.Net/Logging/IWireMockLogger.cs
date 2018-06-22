using JetBrains.Annotations;

namespace WireMock.Logging
{
    /// <summary>
    /// IWireMockLogger interface
    /// </summary>
    [PublicAPI]
    public interface IWireMockLogger
    {
        /// <summary>
        /// Writes the message at the Debug level using the specified parameters. 
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [StringFormatMethod("formatString")]
        void Debug([NotNull] string formatString, [NotNull] params object[] args);

        /// <summary>
        /// Writes the message at the Info level using the specified parameters. 
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [StringFormatMethod("formatString")]
        void Info([NotNull] string formatString, [NotNull] params object[] args);

        /// <summary>
        /// Writes the message at the Warning level using the specified parameters. 
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [StringFormatMethod("formatString")]
        void Warn([NotNull] string formatString, [NotNull] params object[] args);

        /// <summary>
        /// Writes the message at the Error level using the specified parameters. 
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [StringFormatMethod("formatString")]
        void Error([NotNull] string formatString, [NotNull] params object[] args);

        /// <summary>
        /// Writes the message at the Debug level using the specified parameters. 
        /// </summary>
        /// <param name="message">The message.</param>
        [PublicAPI]
        void Debug([NotNull] string message);

        /// <summary>
        /// Writes the message at the Info level using the specified parameters. 
        /// </summary>
        /// <param name="message">The message.</param>
        [PublicAPI]
        void Info([NotNull] string message);

        /// <summary>
        /// Writes the message at the Warning level using the specified parameters. 
        /// </summary>
        /// <param name="message">The message.</param>
        [PublicAPI]
        void Warn([NotNull] string message);

        /// <summary>
        /// Writes the message at the Error level using the specified parameters. 
        /// </summary>
        /// <param name="message">The message.</param>
        [PublicAPI]
        void Error([NotNull] string message);
    }
}