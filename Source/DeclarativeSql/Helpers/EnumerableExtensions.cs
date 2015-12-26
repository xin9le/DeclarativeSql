using System;
using System.Collections;
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


        #region Do
        /// <summary>
        /// コレクションの各要素に対して指定されたアクションを実行し、次に繋げます。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">コレクション</param>
        /// <param name="action">実行する処理</param>
        /// <returns>コレクション</returns>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null)     throw new ArgumentNullException(nameof(action));
            return collection.DoCore(action);
        }


        /// <summary>
        /// Doメソッドの実処理を提供します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="collection">コレクション</param>
        /// <param name="action">実行する処理</param>
        /// <returns>コレクション</returns>
        private static IEnumerable<T> DoCore<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
                yield return item;
            }
        }
        #endregion


        #region Convert
        /// <summary>
        /// 指定のコンバータを利用して非ジェネリックコレクションをジェネリックコレクションに変換します。
        /// </summary>
        /// <typeparam name="T">変換後の要素の型</typeparam>
        /// <param name="collection">変換元コレクション</param>
        /// <param name="converter">型コンバータ</param>
        /// <returns>変換されたコレクション</returns>
        public static IEnumerable<T> Convert<T>(this IEnumerable collection, Func<object, T> converter)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (converter == null)  throw new ArgumentNullException(nameof(converter));
            return collection.ConvertCore(converter);
        }


        /// <summary>
        /// Convertメソッドの実処理を提供します。
        /// </summary>
        /// <typeparam name="T">変換後の要素の型</typeparam>
        /// <param name="collection">変換元コレクション</param>
        /// <param name="converter">型コンバータ</param>
        /// <returns>変換されたコレクション</returns>
        private static IEnumerable<T> ConvertCore<T>(this IEnumerable collection, Func<object, T> converter)
        {
            foreach (var item in collection)
                yield return converter(item);
        }
        #endregion


        #region Concat
        /// <summary>
        /// 指定されたコレクションに要素を連結します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <param name="first">接続先コレクション</param>
        /// <param name="second">末尾に追加する要素</param>
        /// <returns>連結されたコレクション</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, params T[] second)
        {
            if (first == null)  throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            return Enumerable.Concat(first, second);
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


        #region Indexed
        /// <summary>
        /// 要素とインデックスをペアとしたコレクションに変換します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="collection">変換前コレクション</param>
        /// <returns>要素とインデックスのペアを持つコレクション</returns>
        public static IEnumerable<IndexedItem<T>> Indexed<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            return collection.Select((x, i) => new IndexedItem<T>(x, i));
        }
        #endregion


        #region Distinct
        /// <summary>
        /// 指定の比較セレクターを用いて指定のコレクションから重複を削除します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <typeparam name="TKey">比較する値の型</typeparam>
        /// <param name="collection">重複削除の対象コレクション</param>
        /// <param name="selector">比較セレクター</param>
        /// <returns>重複が取り除かれたコレクション</returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> selector)
            => collection.Distinct(new CompareSelector<T, TKey>(selector));
        #endregion


        #region Except
        /// <summary>
        /// 指定された比較セレクターを使用して値を比較することにより、2 つのシーケンスの差集合を生成します。
        /// </summary>
        /// <typeparam name="T">コレクション要素の型</typeparam>
        /// <typeparam name="TKey">比較する値の型</typeparam>
        /// <param name="first">secondには含まれていないが、返される要素を含むコレクション</param>
        /// <param name="second">最初のシーケンスにも含まれ、返されたシーケンスからは削除される要素を含むコレクション</param>
        /// <param name="selector">比較セレクター</param>
        /// <returns>差集合</returns>
        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> selector)
            => first.Except(second, new CompareSelector<T, TKey>(selector));
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
    }
}