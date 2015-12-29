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