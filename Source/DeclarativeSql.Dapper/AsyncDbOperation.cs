using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Helpers;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// 非同期的なデータベース操作を提供します。
    /// </summary>
    public static class AsyncDbOperation
    {
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>レコード数</returns>
        public static async Task<ulong> CountAsync<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateCount<T>();
            var result = await connection.QueryAsync<CountResult>(sql).ConfigureAwait(false);
            return result.Single().Count;
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <returns>レコード数</returns>
        public static async Task<ulong> CountAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));

            var count = PrimitiveSql.CreateCount<T>();
            var where = PredicateSql.From(connection.GetDbKind(), predicate);
            var builder = new StringBuilder();
            builder.AppendLine(count);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            var result = await connection.QueryAsync<CountResult>(builder.ToString(), where.Parameter).ConfigureAwait(false);
            return result.Single().Count;
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public static async Task<IReadOnlyList<T>> SelectAsync<T>(this IDbConnection connection, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var sql = PrimitiveSql.CreateSelect(properties);
            var result = await connection.QueryAsync<T>(sql).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public static async Task<IReadOnlyList<T>> SelectAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var select  = PrimitiveSql.CreateSelect(properties);
            var where   = PredicateSql.From(connection.GetDbKind(), predicate);
            var builder = new StringBuilder();
            builder.AppendLine(select);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            var result = await connection.QueryAsync<T>(builder.ToString(), where.Parameter).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static Task<int> InsertAsync<T>(this IDbConnection connection, T data, bool useSequence = true, bool setIdentity = false)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));

            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(connection.GetDbKind(), type, useSequence, setIdentity);
            return connection.ExecuteAsync(sql, data);
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, params Expression<Func<T, object>>[] properties)
        {
            return connection.UpdateAsync(data, false, properties);
        }


        /// <summary>
        /// 指定された情報でレコードを非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var sql = PrimitiveSql.CreateUpdate(connection.GetDbKind(), setIdentity, properties);
            return connection.ExecuteAsync(sql, data);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            return connection.UpdateAsync(data, predicate, false, properties);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var dbKind  = connection.GetDbKind();
            var update  = PrimitiveSql.CreateUpdate(dbKind, setIdentity, properties);
            var where   = PredicateSql.From(dbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return connection.ExecuteAsync(builder.ToString(), where.Parameter.Merge(data));
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>影響した行数</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateDelete<T>();
            return connection.ExecuteAsync(sql);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">削除条件</param>
        /// <returns>影響した行数</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));

            var delete  = PrimitiveSql.CreateDelete<T>();
            var where   = PredicateSql.From(connection.GetDbKind(), predicate);
            var builder = new StringBuilder();
            builder.AppendLine(delete);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return connection.ExecuteAsync(builder.ToString(), where.Parameter);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>-1</returns>
        public static Task<int> TruncateAsync<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateTruncate<T>();
            return connection.ExecuteAsync(sql);
        }
        #endregion
    }
}