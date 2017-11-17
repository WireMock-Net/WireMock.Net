using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WireMock.Utils
{
    /// <summary>
    /// A special Collection that overrides methods of <see cref="ObservableCollection{T}"/> to make them thread safe
    /// </summary>
    /// <typeparam name="T">The generic type</typeparam>
    /// <seealso cref="ObservableCollection{T}" />
    public class ConcurentObservableCollection<T> : ObservableCollection<T>
    {
        private readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurentObservableCollection{T}"/> class.
        /// </summary>
        public ConcurentObservableCollection() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurentObservableCollection{T}"/> class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public ConcurentObservableCollection(List<T> list) : base(list) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurentObservableCollection{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public ConcurentObservableCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            lock (_lockObject)
            {
                base.ClearItems();
            }
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            lock (_lockObject)
            {
                base.RemoveItem(index);
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            lock (_lockObject)
            {
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            lock (_lockObject)
            {
                base.SetItem(index, item);
            }
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (_lockObject)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }
    }
}