using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DeclarativeSql.DbOperations;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides <see cref="IDbConnection"/> extension methods.
    /// </summary>
    public static class IDbConnectionExtensions
    {
        #region Count
        /// <summary>
        /// Gets the number of records in the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>Record count</returns>
        public static ulong Count<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).Count<T>();
        }


        /// <summary>
        /// Gets the number of records that match the specified condition in the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="timeout"></param>
        /// <returns>Record count</returns>
        public static ulong Count<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).Count(predicate);
        }


        /// <summary>
        /// Gets the number of records in the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>Record count</returns>
        public static Task<ulong> CountAsync<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).CountAsync<T>();
        }


        /// <summary>
        /// Gets the number of records that match the specified condition in the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="timeout"></param>
        /// <returns>Record count</returns>
        public static Task<ulong> CountAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).CountAsync(predicate);
        }
        #endregion


        #region Select
        /// <summary>
        /// Gets all records from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="properties"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection connection, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).Select(properties);
        }


        /// <summary>
        /// Gets records that match the specified condition from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).Select(predicate, properties);
        }


        /// <summary>
        /// Gets all records from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="properties"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<List<T>> SelectAsync<T>(this IDbConnection connection, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).SelectAsync(properties);
        }


        /// <summary>
        /// Gets records that match the specified condition from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Task<List<T>> SelectAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).SelectAsync(predicate, properties);
        }
        #endregion


        #region Insert
        /// <summary>
        /// Inserts the specified data into the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int Insert<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).Insert(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> InsertAsync<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertAsync(data, createdAt);
        }
        #endregion


        #region InsertMulti
        /// <summary>
        /// Inserts the specified data into the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int InsertMulti<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertMulti(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> InsertMultiAsync<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertMultiAsync(data, createdAt);
        }
        #endregion


        #region BulkInsert
        /// <summary>
        /// Inserts the specified data into the table using the bulk method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int BulkInsert<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).BulkInsert(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table using the bulk method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> BulkInsertAsync<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).BulkInsertAsync(data, createdAt);
        }
        #endregion


        #region InsertAndGetId
        /// <summary>
        /// Inserts the specified data into the table and returns the automatically incremented ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Auto incremented ID</returns>
        public static long InsertAndGetId<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertAndGetId(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table and returns the automatically incremented ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <returns>Auto incremented ID</returns>
        public static Task<long> InsertAndGetIdAsync<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertAndGetIdAsync(data, createdAt);
        }
        #endregion


        #region InsertIgnore
        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected row count</returns>
        public static int InsertIgnore<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertIgnore(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected row count</returns>
        public static Task<int> InsertIgnoreAsync<T>(this IDbConnection connection, T data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertIgnoreAsync(data, createdAt);
        }
        #endregion


        #region InsertIgnoreMulti
        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected row count</returns>
        public static int InsertIgnoreMulti<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertIgnoreMulti(data, createdAt);
        }


        /// <summary>
        /// Inserts the specified data into the table.
        /// Insertion processing is not performed when there is a collision with a unique constraint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="createdAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected row count</returns>
        public static Task<int> InsertIgnoreMultiAsync<T>(this IDbConnection connection, IEnumerable<T> data, ValuePriority createdAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).InsertIgnoreMultiAsync(data, createdAt);
        }
        #endregion


        #region Update
        /// <summary>
        /// Updates records with the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int Update<T>(this IDbConnection connection, T data, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).Update(data, properties, modifiedAt);
        }


        /// <summary>
        /// Updates records that match the specified conditions with the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int Update<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).Update(data, predicate, properties, modifiedAt);
        }


        /// <summary>
        /// Updates records with the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            return DbOperation.Create(connection, timeout).UpdateAsync(data, properties, modifiedAt);
        }


        /// <summary>
        /// Updates records that match the specified conditions with the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <param name="properties"></param>
        /// <param name="modifiedAt"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties = null, ValuePriority modifiedAt = default, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).UpdateAsync(data, predicate, properties, modifiedAt);
        }
        #endregion


        #region Delete
        /// <summary>
        /// Deletes all records from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int Delete<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).Delete<T>();
        }


        /// <summary>
        /// Deletes records that match the specified conditions from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static int Delete<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).Delete(predicate);
        }


        /// <summary>
        /// Deletes all records from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).DeleteAsync<T>();
        }


        /// <summary>
        /// Deletes records that match the specified conditions from the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="predicate"></param>
        /// <param name="timeout"></param>
        /// <returns>Effected rows count</returns>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> predicate, int? timeout = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return DbOperation.Create(connection, timeout).DeleteAsync(predicate);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// Truncates the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>-1</returns>
        public static int Truncate<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).Truncate<T>();
        }


        /// <summary>
        /// Truncates the specified table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="timeout"></param>
        /// <returns>-1</returns>
        public static Task<int> TruncateAsync<T>(this IDbConnection connection, int? timeout = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return DbOperation.Create(connection, timeout).TruncateAsync<T>();
        }
        #endregion
    }
}
