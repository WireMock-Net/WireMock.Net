using System.Collections.Generic;
using System.Linq;

namespace WireMock.Util
{
    /// <summary>
    /// A special List which overrides the ToString() to return first value.
    /// </summary>
    /// <typeparam name="T">The generic type</typeparam>
    /// <seealso cref="List{T}" />
    public class WireMockList<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
        /// </summary>
        public WireMockList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public WireMockList(params T[] collection) : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WireMockList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public WireMockList(IEnumerable<T> collection) : base(collection)
        {
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this != null && this.Any())
                return this.First().ToString();

            return base.ToString();
        }
    }
}