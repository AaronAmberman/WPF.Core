using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Wpf.Core
{
    /// <summary>Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>Adds the elements of the specified enumerable to the end of the collection.</summary>
        /// <param name="items">The items to add to the end of the collection.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (!items.Any()) return;

            CheckReentrancy();

            foreach(T item in items)
            {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
        }

        /// <summary>Removes the specified elements from the collection.</summary>
        /// <param name="items">The items to remove from the collection.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (!items.Any()) return;

            CheckReentrancy();

            foreach (T item in items)
            {
                Items.Remove(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items.ToList()));
        }
    }
}
