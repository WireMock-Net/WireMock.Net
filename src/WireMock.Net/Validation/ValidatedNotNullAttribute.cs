using System;

namespace Stef.Validation
{
    /// <summary>
    /// To fix 'xxx' is null on at least one execution path. See also https://rules.sonarsource.com/csharp/RSPEC-3900.
    /// </summary>
    internal class ValidatedNotNullAttribute : Attribute
    {
    }
}
