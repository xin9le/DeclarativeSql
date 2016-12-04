using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Transactions;



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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static int BulkInsert<T>(this IDbTransaction transaction, IEnumerable<T> data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).BulkInsert(data);
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>影響した行数</returns>
        public static Task<int> BulkInsertAsync<T>(this IDbTransaction transaction, IEnumerable<T> data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).BulkInsertAsync(data);
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// 指定されたレコードをテーブルに挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>自動採番ID</returns>
        public static long InsertAndGet<T>(this IDbTransaction transaction, T data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).InsertAndGet(data);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="transaction">トランザクション</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>自動採番ID</returns>
        public static Task<long> InsertAndGetAsync<T>(this IDbTransaction transaction, T data, int? timeout = null)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (data == null)        throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(transaction, timeout).InsertAndGetAsync(data);
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
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


        #region Dapper methods import
        /// <summary>
        /// Execute parameterized SQL
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>Number of rows affected</returns>
        public static int Execute(this IDbTransaction transaction, string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Execute(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a command asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<int> ExecuteAsync(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteAsync(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL and return an System.Data.IDataReader
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>An System.Data.IDataReader that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper,
        /// for example, used to fill a System.Data.DataTable or System.Data.DataSet.
        /// </remarks>
        public static IDataReader ExecuteReader(this IDbTransaction transaction, string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteReader(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL and return an System.Data.IDataReader
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>An System.Data.IDataReader that can be used to iterate over the results of the SQL query.</returns>
        /// <remarks>
        /// This is typically used when the results of a query are not processed by Dapper,
        /// for example, used to fill a System.Data.DataTable or System.Data.DataSet.
        /// </remarks>
        public static Task<IDataReader> ExecuteReaderAsync(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteReaderAsync(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>The first cell selected</returns>
        public static object ExecuteScalar(this IDbTransaction transaction, string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteScalar(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>The first cell selected</returns>
        public static T ExecuteScalar<T>(this IDbTransaction transaction, string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteScalar<T>(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>The first cell selected</returns>
        public static Task<object> ExecuteScalarAsync(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteScalarAsync(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>The first cell selected</returns>
        public static Task<T> ExecuteScalarAsync<T>(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.ExecuteScalarAsync<T>(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Return a list of dynamic objects, reader is closed after the call
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary<string,object></remarks>
        public static IEnumerable<dynamic> Query(this IDbTransaction transaction, string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, param, transaction, buffered, commandTimeout, commandType);
        }


        /// <summary>
        /// Executes a query, returning the data typed as per the Type suggested
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="type"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is
        /// queried then the data from the first column in assumed, otherwise an instance
        /// is created per row, and a direct column-name===member-name mapping is assumed
        /// (case insensitive).
        /// </returns>
        public static IEnumerable<object> Query(this IDbTransaction transaction, Type type, string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, type, sql, param, transaction, buffered, commandTimeout, commandType);
        }


        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is
        /// queried then the data from the first column in assumed, otherwise an instance
        /// is created per row, and a direct column-name===member-name mapping is assumed
        /// (case insensitive).
        /// </returns>
        /// <remarks>
        /// the dynamic param may seem a bit odd, but this works around a major usability
        /// issue in vs, if it is Object vs completion gets annoying. Eg type new [space]
        /// get new object
        /// </remarks>
        public static IEnumerable<T> Query<T>(this IDbTransaction transaction, string sql, object param = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query<T>(transaction.Connection, sql, param, transaction, buffered, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with arbitrary input parameters
        /// </summary>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="types">array of types in the recordset</param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TReturn>(this IDbTransaction transaction, string sql, Type[] types, Func<object[], TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset</typeparam>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 4 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TSeventh"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.Query(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary<string,object></remarks>
        public static Task<IEnumerable<dynamic>> QueryAsync(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="type"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<object>> QueryAsync(this IDbTransaction transaction, Type type, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, type, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a query asynchronously using .NET 4.5 Task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync<T>(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with arbitrary input parameters
        /// </summary>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="types">array of types in the recordset</param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this IDbTransaction transaction, string sql, Type[] types, Func<object[], TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst">The first type in the recordset</typeparam>
        /// <typeparam name="TSecond">The second type in the recordset</typeparam>
        /// <typeparam name="TReturn">The return type</typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Maps a query to objects
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn">The Field we should split and read the second object from (default: id)</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 4 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 5 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 6 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Perform a multi mapping query with 7 input parameters
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TSeventh"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <param name="buffered"></param>
        /// <param name="splitOn"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbTransaction transaction, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, dynamic param = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryAsync(transaction.Connection, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static SqlMapper.GridReader QueryMultiple(this IDbTransaction transaction, string sql, object param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryMultiple(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }


        /// <summary>
        /// Execute a command that returns multiple result sets, and access each in turn
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static Task<SqlMapper.GridReader> QueryMultipleAsync(this IDbTransaction transaction, string sql, dynamic param = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction = transaction.Unwrap();
            return SqlMapper.QueryMultipleAsync(transaction.Connection, sql, param, transaction, commandTimeout, commandType);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// 指定されたトランザクションに内包されたトランザクションを取得します。
        /// 内包されていない場合は自身を返します。
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <returns>内包されていたトランザクション</returns>
        internal static IDbTransaction Unwrap(this IDbTransaction transaction)
            => (transaction as ScopeTransaction)?.Raw ?? transaction;
        #endregion
    }
}