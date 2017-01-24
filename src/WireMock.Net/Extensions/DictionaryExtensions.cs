using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace WireMock.Extensions
{
    /// <summary>
    /// Dictionary Extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts IDictionary to an ExpandObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public static dynamic ToExpandoObject<T>(this IDictionary<string, T> dictionary)
        {
            dynamic expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>)expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                var value = kvp.Value as IDictionary<string, object>;
                if (value != null)
                {
                    var expandoValue = value.ToExpandoObject();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        var objects = item as IDictionary<string, object>;
                        if (objects != null)
                        {
                            var expandoItem = objects.ToExpandoObject();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp.Key, kvp.Value);
                }
            }

            return expando;
        }
    }
}