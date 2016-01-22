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
        /// コレクションの各要素に対して指定されたアクションを実行します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">コレクション</param>
        /// <param name="action">実行する処理</param>
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
        /// 指定されたコレクションを指定の数ずつにまとめます。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">対象となるコレクション</param>
        /// <param name="count">まとめる数</param>
        /// <returns>まとめられたコレクション</returns>
        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> collection, int count)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (count <= 0)         throw new ArgumentOutOfRangeException(nameof(count));
            return collection.BufferCore(count);
        }


        /// <summary>
        /// Bufferメソッドの実処理を提供します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">対象となるコレクション</param>
        /// <param name="count">まとめる数</param>
        /// <returns>まとめられたコレクション</returns>
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
        /// 指定されたコレクションが空かどうかを確認します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">コレクション</param>
        /// <returns>空の場合true</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection) => !collection.Any();
        #endregion


        #region Materialize
        /// <summary>
        /// 指定されたコレクションが遅延状態の場合は実体化して返し、既に実体化されている場合はそれ自身を返します。
        /// </summary>
        /// <param name="collection">対象となるコレクション</param>
        /// <returns>実体化されたコレクション</returns>
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