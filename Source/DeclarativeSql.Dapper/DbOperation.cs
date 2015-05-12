using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// データベース操作を提供します。
    /// </summary>
    public static class DbOperation
    {
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>レコード数</returns>
        public static ulong Count<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateCount<T>();
            return connection.Query<CountResult>(sql).Single().Count;
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <returns>レコード数</returns>
        public static ulong Count<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));

            var count = PrimitiveSql.CreateCount<T>();
            var where = PredicateSql.From(connection.GetDbKind(), predicate);
            var builder = new StringBuilder();
            builder.AppendLine(count);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return connection.Query<CountResult>(builder.ToString(), where.Parameter).Single().Count;
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public static IReadOnlyList<T> Select<T>(this IDbConnection connection, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var sql = PrimitiveSql.CreateSelect(properties);
            return connection.Query<T>(sql) as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public static IReadOnlyList<T> Select<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
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
            return connection.Query<T>(builder.ToString(), where.Parameter) as IReadOnlyList<T>;
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public static int Insert<T>(this IDbConnection connection, T data, bool useSequence = true, bool setIdentity = false)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));

            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(connection.GetDbKind(), type, useSequence, setIdentity);
            return connection.Execute(sql, data);
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbConnection connection, T data, params Expression<Func<T, object>>[] properties)
            => connection.Update(data, false, properties);


        /// <summary>
        /// 指定された情報でレコードを更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbConnection connection, T data, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var sql = PrimitiveSql.CreateUpdate(connection.GetDbKind(), setIdentity, properties);
            return connection.Execute(sql, data);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
            => connection.Update(data, predicate, false, properties);


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public static int Update<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null)       throw new ArgumentNullException(nameof(data));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var dbKind  = connection.GetDbKind();
            var update  = PrimitiveSql.CreateUpdate(dbKind, setIdentity, properties);
            var where   = PredicateSql.From(dbKind, predicate);
            var param   = where.Parameter.Merge(data, properties);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return connection.Execute(builder.ToString(), param);
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>影響した行数</returns>
        public static int Delete<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateDelete<T>();
            return connection.Execute(sql);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">削除条件</param>
        /// <returns>影響した行数</returns>
        public static int Delete<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));

            var delete  = PrimitiveSql.CreateDelete<T>();
            var where   = PredicateSql.From(connection.GetDbKind(), predicate);
            var builder = new StringBuilder();
            builder.AppendLine(delete);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return connection.Execute(builder.ToString(), where.Parameter);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定されたテーブルを切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>-1</returns>
        public static int Truncate<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var sql = PrimitiveSql.CreateTruncate<T>();
            return connection.Execute(sql);
        }
        #endregion
    }
}