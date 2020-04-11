using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MySavesAtPortia.Serialization
{
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly List<KeyValuePair<TKey, TValue>> items = new List<KeyValuePair<TKey, TValue>>();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return items.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (items.Any(t => t.Key.Equals(item.Key)))
                throw new ArgumentException("Key already added", nameof(item));
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return items.Any(t => t.Key.Equals(item.Key) && t.Value.Equals(item.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return items.Remove(item);
        }

        public int Count => items.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            return items.Any(t => t.Key.Equals(key));
        }

        public bool Remove(TKey key)
        {
            return items.RemoveAll(t => t.Key.Equals(key)) > 0;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var idx = items.FindIndex(t => t.Key.Equals(key));
            if (idx == -1)
            {
                value = default(TValue);
                return false;
            }
            value = items[idx].Value;
            return true;
        }

        public TValue this[TKey key]
        {
            get => items.First(t => t.Key.Equals(key)).Value;
            set
            {
                var index = items.FindIndex(t => t.Key.Equals(key));
                if (index < 0)
                    throw new ArgumentException($"Key not found: '{key}'");
                items[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public ICollection<TKey> Keys => items.Select(t => t.Key).ToList();
        public ICollection<TValue> Values => items.Select(t => t.Value).ToList();
    }
}