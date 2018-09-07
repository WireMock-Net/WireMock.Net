using System.Collections.Generic;
using System.Linq;

namespace WireMock.Util
{
    public class IndexableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// Gets the value associated with the specified index.
        /// </summary>
        /// <param name="index"> The index of the value to get.</param>
        /// <returns>The value associated with the specified index.</returns>
        public TValue this[int index]
        {
            get
            {
                // get the item for that index.
                if (index < 0 || index > Count)
                {
                    throw new KeyNotFoundException();
                }
                return Values.Cast<TValue>().ToArray()[index];
            }
        }
    }
}
