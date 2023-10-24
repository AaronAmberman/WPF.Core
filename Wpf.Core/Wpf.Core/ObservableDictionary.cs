using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace WPF.Core
{
    /// <summary>A dictionary that reports changes.</summary>
    /// <typeparam name="TKey">The type of key,</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Constants

        private const string COUNT_PROPERTY = nameof(Count);
        private const string INDEXER_PROPERTY = "Item[]";
        private const string KEYS_PROPERTY = nameof(Keys);
        private const string VALUES_PROPERTY = nameof(Values);

        #endregion

        #region Fields

        protected Dictionary<TKey, TValue> backingDictionary;
        protected bool isBeingModified;

        #endregion

        #region Properties

        /// <summary>Gets or sets the value with the specified key.</summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value associated to the key.</returns>
        public TValue this[TKey key] 
        {
            get => backingDictionary[key];
            set
            {
                CheckReentry();
                CheckReadOnly();

                if (!backingDictionary.ContainsKey(key)) Add(key, value);
                else
                {
                    isBeingModified = true;

                    TValue oldValue = backingDictionary[key];

                    backingDictionary[key] = value;

                    OnPropertyChanged(INDEXER_PROPERTY);
                    OnPropertyChanged(VALUES_PROPERTY);

                    OnCollectionValueChanged(NotifyCollectionChangedAction.Replace,
                        new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, oldValue),
                        backingDictionary.Keys.ToList().IndexOf(key));

                    isBeingModified = false;
                }
            }
        }

        /// <summary>Gets the number of items in the collection.</summary>
        public int Count => backingDictionary.Count;

        /// <summary>Gets or sets whether or not the collection is read only.</summary>
        public bool IsReadOnly { get; set; }

        /// <summary>Gets the collection of keys.</summary>
        public ICollection<TKey> Keys => backingDictionary.Keys;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => backingDictionary.Keys;

        /// <summary>Gets the collection of values.</summary>
        public ICollection<TValue> Values => backingDictionary.Values;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => backingDictionary.Values;

        #endregion

        #region Events

        /// <summary>Occurs when a property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        public ObservableDictionary()
        {            
            backingDictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="capacity">The initial capacity of the dictionary.</param>
        public ObservableDictionary(int capacity)
        {
            backingDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="dictionary">The dictionary to add to this one.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            backingDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="collection">The collection to add to this dictionary.</param>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            backingDictionary = new Dictionary<TKey, TValue>(collection);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="comparer">The equality comparer to use for equality comparisons.</param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            backingDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="capacity">The initial capacity of the dictionary.</param>
        /// <param name="comparer">The equality comparer to use for equality comparisons.</param>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            backingDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="dictionary">The dictionary to add to this one.</param>
        /// <param name="comparer">The equality comparer to use for equality comparisons.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            backingDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.</summary>
        /// <param name="collection">The collection to add to this dictionary.</param>
        /// <param name="comparer">The equality comparer to use for equality comparisons.</param>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            backingDictionary = new Dictionary<TKey, TValue>(collection, comparer);
        }

        #endregion

        #region Methods

        /// <summary>Adds a key and a value to the dictionary.</summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void Add(TKey key, TValue value)
        {
            CheckReentry();
            CheckReadOnly();

            isBeingModified = true;

            backingDictionary.Add(key, value);

            OnPropertyChanged(COUNT_PROPERTY);
            OnPropertyChanged(INDEXER_PROPERTY);
            OnPropertyChanged(KEYS_PROPERTY);
            OnPropertyChanged(VALUES_PROPERTY);
            OnCollectionAdd(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            
            isBeingModified = false;
        }

        /// <summary>Adds a KeyValuePair to the dictionary.</summary>
        /// <param name="item">The KeyValuePair to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>Throws an <see cref="InvalidOperationException"/> if IsReadOnly true and a modification attempt is made.</summary>
        /// <exception cref="InvalidOperationException">Collection is read only and cannot be modified.</exception>
        protected void CheckReadOnly()
        {
            if (IsReadOnly)
                throw new InvalidOperationException("Collection is read only and cannot be modified.");
        }

        /// <summary>Throws an <see cref="InvalidOperationException"/> if a change attempt is made during a CollectionChanged event.</summary>
        /// <exception cref="InvalidOperationException">Cannot modify the collection during a collection changed event.</exception>
        protected void CheckReentry()
        {
            if (isBeingModified)
                throw new InvalidOperationException("Cannot modify the collection during a collection changed event.");
        }

        /// <summary>Clears the dictionary.</summary>
        public void Clear()
        {
            CheckReentry();
            CheckReadOnly();

            isBeingModified = true;

            backingDictionary.Clear();

            OnPropertyChanged(COUNT_PROPERTY);
            OnPropertyChanged(INDEXER_PROPERTY);
            OnPropertyChanged(KEYS_PROPERTY);
            OnPropertyChanged(VALUES_PROPERTY);
            OnCollectionReset(NotifyCollectionChangedAction.Reset);

            isBeingModified = false;
        }

        /// <summary>Determines whether or not the dictionary contains the item by using the equality comparer.</summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the item is found, otherwise false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return backingDictionary.Contains(item);
        }

        /// <summary>Determines if the dictionary contains the specified key.</summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>True if the key is found, otherwise false.</returns>
        public bool ContainsKey(TKey key)
        {
            return backingDictionary.ContainsKey(key);
        }

        /// <summary>Copies the items in the dictionary to the array at the specified index.</summary>
        /// <param name="array">The array to copy items to.</param>
        /// <param name="arrayIndex">The array index to start copying items at.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0 or greater than the number of items.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) 
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > backingDictionary.Count)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            
            for (int i = 0; i < backingDictionary.Count; i++)
            {
                TKey key = backingDictionary.Keys.ElementAt(i);

                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, backingDictionary[key]);
            }
        }

        /// <summary>Gets the enumerator for the dictionary.</summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return backingDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backingDictionary.GetEnumerator();
        }

        /// <summary>Call when needing to send the reset collection signal.</summary>
        /// <param name="action">Should be a Reset action.</param>
        protected virtual void OnCollectionReset(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        /// <summary>Call when adding an item or removing an item to send the appropriate signal.</summary>
        /// <param name="action">Add.</param>
        /// <param name="item">The item being added.</param>
        protected virtual void OnCollectionAdd(NotifyCollectionChangedAction action, object item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        /// <summary>Call when adding an item or removing an item to send the appropriate signal.</summary>
        /// <param name="action">Remove.</param>
        /// <param name="item">The item being removed.</param>
        /// <param name="index">The index of the item that was removed.</param>
        protected virtual void OnCollectionRemove(NotifyCollectionChangedAction action, object item, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>Call when updating an item with the this indexer.</summary>
        /// <param name="action">Should be Replace.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The item being replaced.</param>
        /// <param name="index">The index of the item that was updated.</param>
        protected virtual void OnCollectionValueChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>Call when a property changes value.</summary>
        /// <param name="property">The name of the property changing.</param>
        /// <exception cref="ArgumentNullException">Thrown if property is null, empty or consist of white-space characters only.</exception>
        protected virtual void OnPropertyChanged(string property)
        {
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentNullException(nameof(property));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>Removes an item from the dictionary.</summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>True if the item was removed, otherwise false.</returns>
        public bool Remove(TKey key)
        {
            CheckReentry();
            CheckReadOnly();

            isBeingModified = true;

            TValue value = backingDictionary[key];
            int index = backingDictionary.Keys.ToList().IndexOf(key);

            bool result = backingDictionary.Remove(key);

            OnPropertyChanged(COUNT_PROPERTY);
            OnPropertyChanged(INDEXER_PROPERTY);
            OnPropertyChanged(KEYS_PROPERTY);
            OnPropertyChanged(VALUES_PROPERTY);
            OnCollectionRemove(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value), index);

            isBeingModified = false;

            return result;
        }

        /// <summary>Removes an item from the dictionary.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed, otherwise false.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>Attempts to get the value with the specified key.</summary>
        /// <param name="key">The key to look for.</param>
        /// <param name="value">The out bound value.</param>
        /// <returns>True if the item was found, otherwise false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return backingDictionary.TryGetValue(key, out value);
        }

        #endregion
    }
}
