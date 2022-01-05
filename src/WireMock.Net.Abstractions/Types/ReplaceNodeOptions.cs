using System;

namespace WireMock.Types
{
    /// <summary>
    /// Flags to use when replace a JSON node using the Transformer.
    /// </summary>
    [Flags]
    public enum ReplaceNodeOptions
    {
        /// <summary>
        /// Default
        /// </summary>
        None = 0

        ///// <summary>
        ///// Replace boolean string value to a real boolean value. (This is used by default to maintain backward compatibility.)
        ///// </summary>
        //Bool = 0b00000001,

        ///// <summary>
        ///// Replace integer string value to a real integer value.
        ///// </summary>
        //Integer = 0b00000010,

        ///// <summary>
        ///// Replace long string value to a real long value.
        ///// </summary>
        //Long = 0b00000100,

        ///// <summary>
        ///// Replace all string values to a real values.
        ///// </summary>
        //All = Bool | Integer | Long
    }
}