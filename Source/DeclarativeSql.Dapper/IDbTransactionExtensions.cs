using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// トランザクションの拡張機能を提供します。
    /// </summary>
    public static class IDbTransactionExtensions
    {
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>レコード数</returns>
        public static ulong Count<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).Count<T>();
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>レコード数</returns>
        public static ulong Count<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).Count(predicate);
        }


        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>レコード数</returns>
        public static Task<ulong> CountAsync<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).CountAsync<T>();
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>レコード数</returns>
        public static Task<ulong> CountAsync<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).CountAsync(predicate);
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="properties">取得対象の列</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>取得したレコード</returns>
        public static IReadOnlyList<T> Select<T>(this IDbTransaction transaction, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).Select(properties);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>取得したレコード</returns>
        public static IReadOnlyList<T> Select<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).Select(predicate, properties);
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="properties">取得対象の列</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>取得したレコード</returns>
        public static Task<IReadOnlyList<T>> SelectAsync<T>(this IDbTransaction transaction, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).SelectAsync(properties);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>取得したレコード</returns>
        public static Task<IReadOnlyList<T>> SelectAsync<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).SelectAsync(predicate, properties);
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static int Insert<T>(this IDbTransaction transaction, T data, int? timeout = null, bool useSequence = true, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).Insert(data, useSequence, setIdentity);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static Task<int> InsertAsync<T>(this IDbTransaction transaction, T data, int? timeout = null, bool useSequence = true, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).InsertAsync(data, useSequence, setIdentity);
        }
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public static int BulkInsert<T>(this IDbTransaction transaction, IEnumerable<T> data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).BulkInsert(data);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static Task<int> BulkInsertAsync<T>(this IDbTransaction transaction, IEnumerable<T> data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).BulkInsertAsync(data);
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbTransaction transaction, T data, Expression<Func<T, object>> properties = null, int? timeout = null, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).Update(data, properties, setIdentity);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbTransaction transaction, T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).Update(data, predicate, properties, setIdentity);
        }


        /// <summary>
        /// 指定された情報でレコードを非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbTransaction transaction, T data, Expression<Func<T, object>> properties = null, int? timeout = null, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).UpdateAsync(data, properties, setIdentity);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbTransaction transaction, T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null, bool setIdentity = false)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).UpdateAsync(data, predicate, properties, setIdentity);
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static int Delete<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).Delete<T>();
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">削除条件</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static int Delete<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).Delete(predicate);
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static Task<int> DeleteAsync<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).DeleteAsync<T>();
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="predicate">削除条件</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static Task<int> DeleteAsync<T>(this IDbTransaction transaction, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (predicate == null)   throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(transaction, timeout).DeleteAsync(predicate);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定されたテーブルを切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>-1</returns>
        public static int Truncate<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).Truncate<T>();
        }


        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>-1</returns>
        public static Task<int> TruncateAsync<T>(this IDbTransaction transaction, int? timeout = null)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return DbOperation.Create(transaction, timeout).TruncateAsync<T>();
        }
        #endregion
    }
}