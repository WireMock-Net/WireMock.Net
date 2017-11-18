using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WireMock.Util
{
    /// <summary>
    /// A special Collection that overrides methods of <see cref="ObservableCollection{T}"/> to make them thread safe
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <inheritdoc cref="ObservableCollection{T}" />
    public class ConcurentObservableCollection<T> : ObservableCollection<T>
    {
        private readonly object _lockObject = new object();

        /// <summary> 
        /// Initializes a new instance of the <see cref="T:WireMock.Util.ConcurentObservableCollection`1" /> class. 
        /// </summary> 
        public ConcurentObservableCollection() { }

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

        /// <inheritdoc cref="ObservableCollection{T}.ClearItems"/>
        protected override void ClearItems()
        {
            lock (_lockObject)
            {
                base.ClearItems();
            }
        }

        /// <inheritdoc cref="ObservableCollection{T}.RemoveItem"/>
        protected override void RemoveItem(int index)
        {
            lock (_lockObject)
            {
                base.RemoveItem(index);
            }
        }

        /// <inheritdoc cref="ObservableCollection{T}.InsertItem"/>
        protected override void InsertItem(int index, T item)
        {
            lock (_lockObject)
            {
                base.InsertItem(index, item);
            }
        }

        /// <inheritdoc cref="ObservableCollection{T}.SetItem"/>
        protected override void SetItem(int index, T item)
        {
            lock (_lockObject)
            {
                base.SetItem(index, item);
            }
        }

        /// <inheritdoc cref="ObservableCollection{T}.MoveItem"/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (_lockObject)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }
    }
}