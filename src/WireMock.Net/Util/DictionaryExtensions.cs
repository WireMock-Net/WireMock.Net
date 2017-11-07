using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Util
{
    /// <summary>
    /// Some IDictionary Extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Loops the dictionary and executes the specified action.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to loop (can be null).</param>
        /// <param name="action">The action.</param>
        public static void Loop<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, [NotNull] Action<TKey, TValue> action)
        {
            Check.NotNull(action, nameof(action));

            if (dictionary != null)
            {
                foreach (var entry in dictionary)
                {
                    action(entry.Key, entry.Value);
                }
            }
        }
    }
}