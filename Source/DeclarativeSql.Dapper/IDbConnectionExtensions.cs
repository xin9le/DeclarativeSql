using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// データベース接続の拡張機能を提供します。
    /// </summary>
    public static class IDbConnectionExtensions
    {
        #region 同期
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
            return DbOperation.Create(connection).Count<T>();
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
            return DbOperation.Create(connection).Count(predicate);
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
            return DbOperation.Create(connection).Select(properties);
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
            return DbOperation.Create(connection).Select(predicate, properties);
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
            return DbOperation.Create(connection).Insert(data, useSequence, setIdentity);
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
            return DbOperation.Create(connection).Update(data, setIdentity, properties);
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
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            return DbOperation.Create(connection).Update(data, predicate, setIdentity, properties);
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
            return DbOperation.Create(connection).Delete<T>();
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
            return DbOperation.Create(connection).Delete(predicate);
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
            return DbOperation.Create(connection).Truncate<T>();
        }
        #endregion
        #endregion


        #region 非同期
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <returns>レコード数</returns>
        public static Task<ulong> CountAsync<T>(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection).CountAsync<T>();
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <returns>レコード数</returns>
        public static Task<ulong> CountAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection).CountAsync(predicate);
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
        public static Task<IReadOnlyList<T>> SelectAsync<T>(this IDbConnection connection, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            return DbOperation.Create(connection).SelectAsync(properties);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="connection">データベース接続</param>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public static Task<IReadOnlyList<T>> SelectAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            return DbOperation.Create(connection).SelectAsync(predicate, properties);
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
            return DbOperation.Create(connection).InsertAsync(data, useSequence, setIdentity);
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
            => connection.UpdateAsync(data, false, properties);


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
            return DbOperation.Create(connection).UpdateAsync(data, setIdentity, properties);
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
            => connection.UpdateAsync(data, predicate, false, properties);


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
            if (predicate == null)  throw new ArgumentNullException(nameof(predicate));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            return DbOperation.Create(connection).UpdateAsync(data, predicate, setIdentity, properties);
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
            return DbOperation.Create(connection).DeleteAsync<T>();
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
            return DbOperation.Create(connection).DeleteAsync(predicate);
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
            return DbOperation.Create(connection).TruncateAsync<T>();
        }
        #endregion
        #endregion
    }
}