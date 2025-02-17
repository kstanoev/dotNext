﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNext.Collections.Generic
{
    /// <summary>
    /// Represents various extensions for types <see cref="Dictionary{TKey, TValue}"/>
    /// and <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class Dictionary
    {
        private static class Indexer<D, K, V>
            where D : class, IEnumerable<KeyValuePair<K, V>>
        {
            internal static readonly Func<D, K, V> Getter;
            internal static readonly Action<D, K, V> Setter;

            static Indexer()
            {
                foreach (var member in typeof(D).GetDefaultMembers())
                    if (member is PropertyInfo indexer)
                    {
                        Getter = indexer.GetMethod.CreateDelegate<Func<D, K, V>>();
                        Setter = indexer.SetMethod?.CreateDelegate<Action<D, K, V>>();
                        return;
                    }
            }
        }

        /// <summary>
        /// Provides strongly-typed access to dictionary indexer.
        /// </summary>
        /// <typeparam name="K">Type of keys in the dictionary.</typeparam>
        /// <typeparam name="V">Type of values in the dictionary.</typeparam>
		public static class Indexer<K, V>
        {
            /// <summary>
            /// Represents read-only dictionary indexer.
            /// </summary>
			public static Func<IReadOnlyDictionary<K, V>, K, V> ReadOnly => Indexer<IReadOnlyDictionary<K, V>, K, V>.Getter;

            /// <summary>
            /// Represents dictionary value getter.
            /// </summary>
			public static Func<IDictionary<K, V>, K, V> Getter => Indexer<IDictionary<K, V>, K, V>.Getter;

            /// <summary>
            /// Represents dictionary value setter.
            /// </summary>
			public static Action<IDictionary<K, V>, K, V> Setter => Indexer<IDictionary<K, V>, K, V>.Setter;
        }

        /// <summary>
        /// Returns <see cref="IReadOnlyDictionary{TKey, TValue}.get_Item"/> as
        /// delegate attached to the dictionary instance.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <param name="dictionary">Read-only dictionary instance.</param>
        /// <returns>A delegate representing dictionary indexer.</returns>
        public static Func<K, V> IndexerGetter<K, V>(this IReadOnlyDictionary<K, V> dictionary)
            => Indexer<K, V>.ReadOnly.Method.CreateDelegate<Func<K, V>>(dictionary);

        /// <summary>
        /// Returns <see cref="IDictionary{TKey, TValue}.get_Item"/> as
        /// delegate attached to the dictionary instance.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <param name="dictionary">Mutable dictionary instance.</param>
        /// <returns>A delegate representing dictionary indexer.</returns>
        public static Func<K, V> IndexerGetter<K, V>(this IDictionary<K, V> dictionary)
            => Indexer<K, V>.Getter.Method.CreateDelegate<Func<K, V>>(dictionary);

        /// <summary>
        /// Returns <see cref="IDictionary{TKey, TValue}.set_Item"/> as
        /// delegate attached to the dictionary instance.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <param name="dictionary">Mutable dictionary instance.</param>
        /// <returns>A delegate representing dictionary indexer.</returns>
        public static Action<K, V> IndexerSetter<K, V>(this IDictionary<K, V> dictionary)
            => Indexer<K, V>.Setter.Method.CreateDelegate<Action<K, V>>(dictionary);

        /// <summary>
        /// Deconstruct key/value pair.
        /// </summary>
        /// <typeparam name="K">Type of key.</typeparam>
        /// <typeparam name="V">Type of value.</typeparam>
        /// <param name="pair">A pair to decompose.</param>
        /// <param name="key">Deconstructed key.</param>
        /// <param name="value">Deconstructed value.</param>
        public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary if the key does not exist.
        /// </summary>
        /// <typeparam name="K">The key type of the dictionary.</typeparam>
        /// <typeparam name="V">The value type of the dictionary.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key of the key-value pair.</param>
        /// <param name="value">The value of the key-value pair.</param>
        /// <returns>
        /// The corresponding value in the dictionary if <paramref name="key"/> already exists, 
        /// or <paramref name="value"/>.
        /// </returns>
        public static V GetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.TryGetValue(key, out var temp))
                value = temp;
            else
                dictionary.Add(key, value);
            return value;
        }

        /// <summary>
        /// Generates a value and adds the key-value pair to the dictionary if the key does not
        /// exist.
        /// </summary>
        /// <typeparam name="K">The key type of the dictionary.</typeparam>
        /// <typeparam name="V">The value type of the dictionary.</typeparam>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="key">The key of the key-value pair.</param>
        /// <param name="valueFactory">
        /// The function used to generate the value from the key.
        /// </param>
        /// <returns>
        /// The corresponding value in the dictionary if <paramref name="key"/> already exists, 
        /// or the value generated by <paramref name="valueFactory"/>.
        /// </returns>
        public static V GetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key, Func<K, V> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;
            else
            {
                value = valueFactory(key);
                dictionary.Add(key, value);
                return value;
            }
        }

        /// <summary>
        /// Applies specific action to each dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="action"></param>
		public static void ForEach<K, V>(this IDictionary<K, V> dictionary, Action<K, V> action)
        {
            foreach (var (key, value) in dictionary)
                action(key, value);
        }

        /// <summary>
        /// Gets dictionary value by key if it exists or
        /// invoke <paramref name="defaultValue"/> and
        /// return its result as a default value.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <param name="dictionary">A dictionary to read from.</param>
        /// <param name="key">A key associated with the value.</param>
        /// <param name="defaultValue">A delegate to be invoked if key doesn't exist in the dictionary.</param>
        /// <returns>The value associated with the key or returned by the delegate.</returns>
		public static V GetOrInvoke<K, V>(this IDictionary<K, V> dictionary, K key, Func<V> defaultValue)
            => dictionary.TryGetValue(key, out var value) ? value : defaultValue();

        /// <summary>
        /// Gets dictionary value associated with the key
        /// and convert that value using passed converter.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <typeparam name="T">Type of value conversion result.</typeparam>
        /// <param name="dictionary">A dictionary to read from.</param>
        /// <param name="key">A key associated with value.</param>
        /// <param name="mapper">Value converter.</param>
        /// <returns>Converted value associated with the key.</returns>
        public static Optional<T> ConvertValue<K, V, T>(this IDictionary<K, V> dictionary, K key, Converter<V, T> mapper)
            => dictionary.TryGetValue(key, out var value) ? mapper(value) : Optional<T>.Empty;

        /// <summary>
        /// Gets dictionary value associated with the key
        /// and convert that value using passed converter.
        /// </summary>
        /// <typeparam name="K">Type of dictionary keys.</typeparam>
        /// <typeparam name="V">Type of dictionary values.</typeparam>
        /// <typeparam name="T">Type of value conversion result.</typeparam>
        /// <param name="dictionary">A dictionary to read from.</param>
        /// <param name="key">A key associated with value.</param>
        /// <param name="mapper">Value converter.</param>
        /// <param name="value">Converted value associated with the key.</param>
        /// <returns><see langword="true"/>, if key exists in the dictionary; otherwise, <see langword="false"/>.</returns>
		public static bool ConvertValue<K, V, T>(this IDictionary<K, V> dictionary, K key, Converter<V, T> mapper, out T value)
            => dictionary.ConvertValue(key, mapper).TryGet(out value);

        /// <summary>
        /// Obtains read-only view of the dictionary.
        /// </summary>
        /// <remarks>
        /// Any changes in the dictionary will be visible from read-only view.
        /// </remarks>
        /// <typeparam name="K">Type of keys.</typeparam>
        /// <typeparam name="V">Type of values.</typeparam>
        /// <param name="dictionary">A dictionary.</param>
        /// <returns>Read-only view of the dictionary.</returns>
        public static ReadOnlyDictionaryView<K, V> AsReadOnlyView<K, V>(this IDictionary<K, V> dictionary)
            => new ReadOnlyDictionaryView<K, V>(dictionary);

        /// <summary>
        /// Applies lazy conversion for each dictionary value.
        /// </summary>
        /// <typeparam name="K">Type of keys.</typeparam>
        /// <typeparam name="V">Type of values.</typeparam>
        /// <typeparam name="T">Type of mapped values.</typeparam>
        /// <param name="dictionary">A dictionary to be mapped.</param>
        /// <param name="mapper">Mapping function.</param>
        /// <returns>Read-only view of the dictionary where each value is converted in lazy manner.</returns>
        public static ReadOnlyDictionaryView<K, V, T> Convert<K, V, T>(this IReadOnlyDictionary<K, V> dictionary, Converter<V, T> mapper)
            => new ReadOnlyDictionaryView<K, V, T>(dictionary, mapper);
    }
}
