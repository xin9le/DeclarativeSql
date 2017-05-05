using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    using Constructor = Func<IDbConnection, IDbTransaction, DbProvider, int?, DbOperation>;
    using This = DbOperation;



    /// <summary>
    /// Provides the database operations for common part.
    /// </summary>
    internal class DbOperation
    {
        #region Properties
        /// <summary>
        /// Gets database connection.
        /// </summary>
        protected IDbConnection Connection { get; }


        /// <summary>
        /// Gets database transaction.
        /// </summary>
        protected IDbTransaction Transaction { get; }


        /// <summary>
        /// Gets database provider。
        /// </summary>
        protected DbProvider DbProvider { get; }


        /// <summary>
        /// Gets timeout。
        /// </summary>
        protected int? Timeout { get; }


        /// <summary>
        /// Get the cache for constructor delegate.
        /// </summary>
        private static IReadOnlyDictionary<DbKind, Constructor> Constructors { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Call when first access.
        /// </summary>
        static DbOperation()
        {
            This.Constructors
                = typeof(This)
                .GetTypeInfo()
                .Assembly
                .GetLoadableTypes()
                .Select(x => x.GetTypeInfo())
                .Where(x => x.IsSubclassOf(typeof(This)))
                .Where(x => !x.IsAbstract)
                .Where(x => x.IsClass)
                .Where(x => x.IsNotPublic)
                .Join
                (
                    Enum.GetValues(typeof(DbKind))
                    .Cast<DbKind>()
                    .Select(x => new
                    {
                        Name = Enum.GetName(typeof(DbKind), x),
                        Value = x,
                    }),
                    x => x.Name,
                    y => y.Name + "Operation",
                    (x, y) => new { Type = x, DbKind = y.Value }
                )
                .Select(x =>
                {
                    var types = new []{ typeof(IDbConnection), typeof(IDbTransaction), typeof(DbProvider), typeof(int?) };
                    var @params = types.Select(Expression.Parameter).ToArray();
                    //var ctor = x.Type.GetConstructor(types);
                    var ctor = x.Type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];  //--- workaround
                    var @new = Expression.New(ctor, @params);
                    var lambda = Expression.Lambda<Constructor>(@new, @params);
                    return new
                    {
                        DbKind = x.DbKind,
                        Constructor = lambda.Compile(),
                    };
                })
                .ToDictionary(x => x.DbKind, x => x.Constructor);
        }


        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected DbOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
        {
            this.Connection = connection;
            this.Transaction = transaction.Unwrap();
            this.DbProvider = provider;
            this.Timeout = timeout;
        }
        #endregion


        #region Generate
        /// <summary>
        /// データベース接続からデータベース操作のインスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>データベース操作</returns>
        public static This Create(IDbConnection connection, int? timeout)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            return This.Create(connection, null, timeout);
        }


        /// <summary>
        /// トランザクションからデータベース操作のインスタンスを生成します。
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>データベース操作</returns>
        public static This Create(IDbTransaction transaction, int? timeout)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return This.Create(transaction.Connection, transaction, timeout);
        }


        /// <summary>
        /// データベース接続とトランザクションからデータベース操作のインスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        /// <returns>データベース操作</returns>
        private static This Create(IDbConnection connection, IDbTransaction transaction, int? timeout)
        {
            var provider = DbProvider.ByConnectionTypeName[connection.GetType().FullName];
            return  This.Constructors.ContainsKey(provider.Kind)
                ?   This.Constructors[provider.Kind](connection, transaction, provider, timeout)
                :   new This(connection, transaction, provider, timeout);
        }
        #endregion


        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>レコード数</returns>
        public virtual ulong Count<T>()
        {
            var sql = this.DbProvider.Count<T>().ToString();
            return this.Connection.ExecuteScalar<ulong>(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <returns>レコード数</returns>
        public virtual ulong Count<T>(Expression<Func<T, bool>> predicate)
        {
            var count = this.DbProvider.Count<T>().Where(predicate).Build();
            return this.Connection.ExecuteScalar<ulong>(count.Statement, count.WhereParameters, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>レコード数</returns>
        public virtual Task<ulong> CountAsync<T>()
        {
            var sql = this.DbProvider.Count<T>().ToString();
            return this.Connection.ExecuteScalarAsync<ulong>(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルにおいて指定の条件に一致するレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <returns>レコード数</returns>
        public virtual Task<ulong> CountAsync<T>(Expression<Func<T, bool>> predicate)
        {
            var count = this.DbProvider.Count<T>().Where(predicate).Build();
            return this.Connection.ExecuteScalarAsync<ulong>(count.Statement, count.WhereParameters, this.Transaction, this.Timeout);
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual IReadOnlyList<T> Select<T>(Expression<Func<T, object>> properties)
        {
            var sql = this.DbProvider.Select(properties).ToString();
            return this.Connection.Query<T>(sql, null, this.Transaction, true, this.Timeout) as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual IReadOnlyList<T> Select<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties)
        {
            var select = this.DbProvider.Select(properties).Where(predicate).Build();
            return this.Connection.Query<T>(select.Statement, select.WhereParameters, this.Transaction, true, this.Timeout) as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual async Task<IReadOnlyList<T>> SelectAsync<T>(Expression<Func<T, object>> properties)
        {
            var sql = this.DbProvider.Select(properties).ToString();
            var result = await this.Connection.QueryAsync<T>(sql, null, this.Transaction, this.Timeout).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual async Task<IReadOnlyList<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties)
        {
            var select = this.DbProvider.Select(properties).Where(predicate).Build();
            var result = await this.Connection.QueryAsync<T>(select.Statement, select.WhereParameters, this.Transaction, this.Timeout).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual int Insert<T>(T data, bool useSequence, bool setIdentity)
        {
            var sql = this.DbProvider.Insert<T>(useSequence, setIdentity).ToString();
            return this.Connection.Execute(sql, data, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> InsertAsync<T>(T data, bool useSequence, bool setIdentity)
        {
            var sql = this.DbProvider.Insert<T>(useSequence, setIdentity).ToString();
            return this.Connection.ExecuteAsync(sql, data, this.Transaction, this.Timeout);
        }
        #endregion


        #region BulkInsert
        /// <summary>
        /// Inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public virtual int BulkInsert<T>(IEnumerable<T> data)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Asynchronously inserts the specified record into the table by bulk method.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Affected row count.</returns>
        public virtual Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            throw new NotSupportedException();
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// Inserts the specified record into the table and returns the automatic assignment ID.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Automatic assignment ID.</returns>
        public virtual long InsertAndGet<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var sql = this.CreateInsertAndGetSql<T>();
            var reader = this.Connection.QueryMultiple(sql, data, this.Transaction, this.Timeout);
            return (long)reader.Read().First().Id;
        }


        /// <summary>
        /// Asynchronously inserts the specified record into the table and returns the automatic assignment ID.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <param name="data">Inserting target data.</param>
        /// <returns>Automatic assignment ID.</returns>
        public virtual async Task<long> InsertAndGetAsync<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var sql = this.CreateInsertAndGetSql<T>();
            var reader = await this.Connection.QueryMultipleAsync(sql, data, this.Transaction, this.Timeout).ConfigureAwait(false);
            var results = await reader.ReadAsync().ConfigureAwait(false);
            return (long)results.First().Id;
        }


        /// <summary>
        /// Generates SQL to insert record and get the automatically assigned ID.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <returns>SQL</returns>
        protected virtual string CreateInsertAndGetSql<T>()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Diagnoses whether the InsertAndGet method can be executed.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <returns>Retruns true if executable.</returns>
        protected static void AssertInsertAndGet<T>()
        {
            if (typeof(T).IsCollection())
                throw new InvalidOperationException("Can insert single entity only.");

            var table = TableMappingInfo.Create<T>();
            var primary = table.Columns.Where(x => x.IsPrimaryKey).ToArray();
            if (primary.Length != 1)
                throw new InvalidOperationException("Primary key column should be only one.");

            var autoIncrement = table.Columns.Where(x => x.IsAutoIncrement).ToArray();
            var sequence      = table.Columns.Where(x => x.Sequence != null).ToArray();
            var idColumnCount = autoIncrement.Length + sequence.Length;
            if (idColumnCount != 1)
                throw new InvalidOperationException("Id column (auto increment or sequence) should be only one.");

            if (!primary[0].IsAutoIncrement && primary[0].Sequence == null)
                throw new InvalidOperationException("Id column should be primary key.");
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual int Update<T>(T data, Expression<Func<T, object>> properties, bool setIdentity)
        {
            var sql = this.DbProvider.Update(properties, setIdentity).ToString();
            return this.Connection.Execute(sql, data, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual int Update<T>(T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties, bool setIdentity)
        {
            var update = this.DbProvider.Update(properties, setIdentity).Where(predicate).Build();
            var param  = update.WhereParameters.Merge(data, properties);
            return this.Connection.Execute(update.Statement, param, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定された情報でレコードを非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> UpdateAsync<T>(T data, Expression<Func<T, object>> properties, bool setIdentity)
        {
            var sql = this.DbProvider.Update(properties, setIdentity).ToString();
            return this.Connection.ExecuteAsync(sql, data, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> UpdateAsync<T>(T data, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> properties, bool setIdentity)
        {
            var update = this.DbProvider.Update(properties, setIdentity).Where(predicate).Build();
            var param  = update.WhereParameters.Merge(data, properties);
            return this.Connection.ExecuteAsync(update.Statement, param, this.Transaction, this.Timeout);
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>影響した行数</returns>
        public virtual int Delete<T>()
        {
            var sql = this.DbProvider.Delete<T>().ToString();
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">削除条件</param>
        /// <returns>影響した行数</returns>
        public virtual int Delete<T>(Expression<Func<T, bool>> predicate)
        {
            var delete = this.DbProvider.Delete<T>().Where(predicate).Build();
            return this.Connection.Execute(delete.Statement, delete.WhereParameters, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>影響した行数</returns>
        public virtual Task<int> DeleteAsync<T>()
        {
            var sql = this.DbProvider.Delete<T>().ToString();
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">削除条件</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
        {
            var delete = this.DbProvider.Delete<T>().Where(predicate).Build();
            return this.Connection.ExecuteAsync(delete.Statement, delete.WhereParameters, this.Transaction, this.Timeout);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定されたテーブルを切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>-1</returns>
        public virtual int Truncate<T>()
        {
            var sql = this.DbProvider.Truncate<T>().ToString();
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>-1</returns>
        public virtual Task<int> TruncateAsync<T>()
        {
            var sql = this.DbProvider.Truncate<T>().ToString();
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }
        #endregion
    }
}