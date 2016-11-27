using System;
using System.Collections.Generic;
using System.Linq;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// System.Collection.Generic.IEnumerable&lt;T&gt;の拡張機能を提供します。
    /// </summary>
    internal static class EnumerableExtensions
    {
        #region ForEach
        /// <summary>
        /// Executes the specified action on each element of the collection.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="action">Process to be executed</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null)     throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
                action(item);
        }
        #endregion


        #region Buffer
        /// <summary>
        /// Concatenate the specified collections by the specified number.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="count">Buffering count</param>
        /// <returns>Buffered collection</returns>
        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> collection, int count)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (count <= 0)         throw new ArgumentOutOfRangeException(nameof(count));
            return collection.BufferCore(count);
        }


        /// <summary>
        /// Provide core function of Buffer method.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <param name="count">Buffering count</param>
        /// <returns>Buffered collection</returns>
        private static IEnumerable<IEnumerable<T>> BufferCore<T>(this IEnumerable<T> collection, int count)
        {
            var result = new List<T>(count);
            foreach (var item in collection)
            {
                result.Add(item);
                if (result.Count == count)
                {
                    yield return result;
                    result = new List<T>(count);
                }
            }
            if (result.Count != 0)
                yield return result.ToArray();
        }
        #endregion


        #region IsEmpty
        /// <summary>
        /// Checks if the specified collection is empty.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Target collection</param>
        /// <returns>True if empty</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection) => !collection.Any();
        #endregion


        #region Materialize
        /// <summary>
        /// If the specified collection is in a delayed state, it is instantiated and returned, and if it is already materialized it returns itself.
        /// </summary>
        /// <param name="collection">Target collection</param>
        /// <returns>Materialized collection</returns>
        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            return  collection is ICollection<T>         ? collection
                :   collection is IReadOnlyCollection<T> ? collection
                :   collection.ToArray();
        }
        #endregion
    }
}