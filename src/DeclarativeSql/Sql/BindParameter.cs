using System;
using System.Collections;
using System.Collections.Generic;
using FastMember;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents bind parameters.
    /// </summary>
    public sealed class BindParameter : IDictionary<string, object>, IReadOnlyDictionary<string, object>
    {
        #region Properties
        /// <summary>
        /// Gets the key/value store that held inside.
        /// </summary>
        private IDictionary<string, object> Inner { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        public BindParameter()
            : this(new Dictionary<string, object>())
        {}


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="source"></param>
        public BindParameter(IDictionary<string, object> source)
            => this.Inner = source ?? throw new ArgumentNullException(nameof(source));
        #endregion


        #region IDictionary<TKey, TValue> implementations
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get => this.Inner[key];
            set => this.Inner[key] = value;
        }


        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the keys of the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ICollection<string> IDictionary<string, object>.Keys
            => this.Inner.Keys;


        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values of the <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        ICollection<object> IDictionary<string, object>.Values
            => this.Inner.Values;


        /// <summary>
        /// Gets the number of elements contained in the <see cref="ICollection{T}"/>.
        /// </summary>
        public int Count
            => this.Inner.Count;


        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
            => this.Inner.IsReadOnly;


        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
            => this.Inner.Add(key, value);


        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            => this.Inner.Add(item);


        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        public void Clear()
            => this.Inner.Clear();


        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            => this.Inner.Contains(item);


        /// <summary>
        /// Determines whether the <see cref="IDictionary{TKey, TValue}"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
            => this.Inner.ContainsKey(key);


        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular <seealso cref="Array"/> index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            => this.Inner.CopyTo(array, arrayIndex);


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => this.Inner.GetEnumerator();


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => this.Inner.GetEnumerator();


        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
            => this.Inner.Remove(key);


        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            => this.Inner.Remove(item);


        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
            => this.Inner.TryGetValue(key, out value);
        #endregion


        #region IReadOnlyDictionary<TKey, TValue> implementations
        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        public IEnumerable<string> Keys
            => this.Inner.Keys;


        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        public IEnumerable<object> Values
            => this.Inner.Values;
        #endregion


        #region Create
        /// <summary>
        /// Creates an instance from the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BindParameter From<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var result = new BindParameter();
            var members = TypeAccessor.Create(typeof(T)).GetMembers();
            var accessor = ObjectAccessor.Create(obj);
            for (var i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.CanRead)
                    result.Add(member.Name, accessor[member.Name]);
            }
            return result;
        }


        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns></returns>
        public BindParameter Clone()
        {
            IDictionary<string, object> result = new BindParameter();
            foreach (var x in this)
                result.Add(x);
            return (BindParameter)result;
        }
        #endregion


        #region Update
        /// <summary>
        /// Updates the value of the source property with the matching key name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public void Update<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var accessor = ObjectAccessor.Create(source);
            var members = TypeAccessor.Create(typeof(T)).GetMembers();
            for (var i = 0; i < members.Count; i++)
            {
                var member = members[i];
                if (member.CanRead && this.ContainsKey(member.Name))  // if exists same name
                    this[member.Name] = accessor[member.Name];
            }
        }


        /// <summary>
        /// Merges the specified values.
        /// </summary>
        /// <param name="kvs"></param>
        public void Merge(IEnumerable<KeyValuePair<string, object>> kvs)
        {
            if (kvs == null)
                throw new ArgumentNullException(nameof(kvs));

            foreach (var x in kvs)
                this.Add(x.Key, x.Value);
        }
        #endregion
    }
}
