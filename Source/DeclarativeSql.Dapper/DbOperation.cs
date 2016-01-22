using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;
using This = DeclarativeSql.Dapper.DbOperation;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// データベース操作を提供します。
    /// </summary>
    internal class DbOperation
    {
        #region プロパティ
        /// <summary>
        /// データベース接続を取得します。
        /// </summary>
        protected IDbConnection Connection { get; }


        /// <summary>
        /// トランザクションを取得します。
        /// </summary>
        protected IDbTransaction Transaction { get; }


        /// <summary>
        /// データベースの種類を取得します。
        /// </summary>
        protected DbKind DbKind { get; }


        /// <summary>
        /// タイムアウト時間を取得します。
        /// </summary>
        protected int? Timeout { get; }


        /// <summary>
        /// コンストラクタのデリゲートのキャッシュを取得します。
        /// </summary>
        private static IReadOnlyDictionary<DbKind, Func<IDbConnection, IDbTransaction, int?, This>> Constructors { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// この型に最初にアクセスされたときに呼び出されます。
        /// </summary>
        static DbOperation()
        {
            This.Constructors
                = Assembly.GetExecutingAssembly()
                .GetTypes()
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
                    var type1 = typeof(IDbConnection);
                    var type2 = typeof(IDbTransaction);
                    var type3 = typeof(int?);
                    var param1 = Expression.Parameter(type1, "p1");
                    var param2 = Expression.Parameter(type2, "p2");
                    var param3 = Expression.Parameter(type3, "p3");
                    var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    var args = new [] { type1, type2, type3 };
                    var ctor = x.Type.GetConstructor(flags, null, CallingConventions.Standard, args, null);
                    var @new = Expression.New(ctor, param1, param2, param3);
                    var lambda = Expression.Lambda<Func<IDbConnection, IDbTransaction, int?, This>>(@new, param1, param2, param3);
                    return new
                    {
                        DbKind = x.DbKind,
                        Constructor = lambda.Compile(),
                    };
                })
                .ToDictionary(x => x.DbKind, x => x.Constructor);
        }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected DbOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
        {
            this.Connection = connection;
            this.Transaction = transaction.Unwrap();
            this.DbKind = connection.GetDbKind();
            this.Timeout = timeout;
        }
        #endregion


        #region 生成
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
            var dbKind = connection.GetDbKind();
            return  This.Constructors.ContainsKey(dbKind)
                ?   This.Constructors[dbKind](connection, transaction, timeout)
                :   new This(connection, transaction, timeout);
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
            var sql = PrimitiveSql.CreateCount<T>();
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
            var count = PrimitiveSql.CreateCount<T>();
            var where = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(count);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.ExecuteScalar<ulong>(builder.ToString(), where.Parameter, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>レコード数</returns>
        public virtual Task<ulong> CountAsync<T>()
        {
            var sql = PrimitiveSql.CreateCount<T>();
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
            var count = PrimitiveSql.CreateCount<T>();
            var where = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(count);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.ExecuteScalarAsync<ulong>(builder.ToString(), where.Parameter, this.Transaction, this.Timeout);
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
            var sql = PrimitiveSql.CreateSelect(properties);
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
            var select  = PrimitiveSql.CreateSelect(properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(select);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.Query<T>(builder.ToString(), where.Parameter, this.Transaction, true, this.Timeout) as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual async Task<IReadOnlyList<T>> SelectAsync<T>(Expression<Func<T, object>> properties)
        {
            var sql = PrimitiveSql.CreateSelect(properties);
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
            var select  = PrimitiveSql.CreateSelect(properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(select);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            var result = await this.Connection.QueryAsync<T>(builder.ToString(), where.Parameter, this.Transaction, this.Timeout).ConfigureAwait(false);
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
            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(this.DbKind, type, useSequence, setIdentity);
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
            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(this.DbKind, type, useSequence, setIdentity);
            return this.Connection.ExecuteAsync(sql, data, this.Transaction, this.Timeout);
        }
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public virtual int BulkInsert<T>(IEnumerable<T> data)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            throw new NotSupportedException();
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// 指定されたレコードをテーブルに挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public virtual long InsertAndGet<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var sql = this.CreateInsertAndGetSql<T>();
            var reader = this.Connection.QueryMultiple(sql, data, this.Transaction, this.Timeout);
            return (long)reader.Read().First().Id;
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public virtual async Task<long> InsertAndGetAsync<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var sql = this.CreateInsertAndGetSql<T>();
            var reader = await this.Connection.QueryMultipleAsync(sql, data, this.Transaction, this.Timeout).ConfigureAwait(false);
            var results = await reader.ReadAsync().ConfigureAwait(false);
            return (long)results.First().Id;
        }


        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected virtual string CreateInsertAndGetSql<T>()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// InsertAndGet メソッドを実行可能かどうかを診断します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>実行可能かどうか</returns>
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
            var sql = PrimitiveSql.CreateUpdate(this.DbKind, properties, setIdentity);
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
            var update  = PrimitiveSql.CreateUpdate(this.DbKind, properties, setIdentity);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var param   = where.Parameter.Merge(data, properties);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.Execute(builder.ToString(), param, this.Transaction, this.Timeout);
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
            var sql = PrimitiveSql.CreateUpdate(this.DbKind, properties, setIdentity);
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
            var update  = PrimitiveSql.CreateUpdate(this.DbKind, properties, setIdentity);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var param   = where.Parameter.Merge(data, properties);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.ExecuteAsync(builder.ToString(), param, this.Transaction, this.Timeout);
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
            var sql = PrimitiveSql.CreateDelete<T>();
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
            var delete  = PrimitiveSql.CreateDelete<T>();
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(delete);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.Execute(builder.ToString(), where.Parameter, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>影響した行数</returns>
        public virtual Task<int> DeleteAsync<T>()
        {
            var sql = PrimitiveSql.CreateDelete<T>();
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
            var delete  = PrimitiveSql.CreateDelete<T>();
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(delete);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.ExecuteAsync(builder.ToString(), where.Parameter, this.Transaction, this.Timeout);
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
            var sql = PrimitiveSql.CreateTruncate<T>();
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>-1</returns>
        public virtual Task<int> TruncateAsync<T>()
        {
            var sql = PrimitiveSql.CreateTruncate<T>();
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }
        #endregion
    }



    /// <summary>
    /// Sql Serverデータベースに対する操作を提供します。
    /// </summary>
    internal class SqlServerOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected SqlServerOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            using (var executor = this.CreateBulkExecutor())
            {
                var param = this.SetupBulkInsert(executor, data);
                executor.WriteToServer(param);
                return param.Rows.Count;
            }
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override async Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            using (var executor = this.CreateBulkExecutor())
            {
                var param = this.SetupBulkInsert(executor, data);
                await executor.WriteToServerAsync(param).ConfigureAwait(false);
                return param.Rows.Count;
            }
        }


        /// <summary>
        /// バルク処理の実行機能を生成します。
        /// </summary>
        /// <returns>インスタンス</returns>
        private SqlBulkCopy CreateBulkExecutor() => new SqlBulkCopy(this.Connection as SqlConnection, SqlBulkCopyOptions.Default, this.Transaction as SqlTransaction);


        /// <summary>
        /// バルク方式での挿入処理の準備を行います。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="executor">バルク処理実行機能</param>
        /// <param name="data">挿入する生データ</param>
        /// <returns>挿入するデータ</returns>
        private DataTable SetupBulkInsert<T>(SqlBulkCopy executor, IEnumerable<T> data)
        {
            //--- タイムアウト
            if (this.Timeout.HasValue)
                executor.BulkCopyTimeout = this.Timeout.Value;
            executor.BulkCopyTimeout = 1;
            
            //--- 対象テーブル名
            var info = TableMappingInfo.Create<T>();
            executor.DestinationTableName = info.FullName;

            //--- 列のマップ
            var table = new DataTable();
            var getters = new List<Func<T, object>>();
            foreach (var x in info.Columns)
            {
                executor.ColumnMappings.Add(x.PropertyName, x.ColumnName);
                table.Columns.Add(x.PropertyName, x.PropertyType);
                getters.Add(AccessorCache<T>.LookupGet(x.PropertyName));
            }

            //--- データ生成
            foreach (var x in data)
            {
                var row = table.NewRow();
                for (int i = 0; i < getters.Count; i++)
                    row[i] = getters[i](x);
                table.Rows.Add(row);
            }
            return table;
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select cast(scope_identity() as bigint) as Id;";
        #endregion
    }



    /// <summary>
    /// SQL Server Compactデータベースに対する操作を提供します。
    /// </summary>
    internal class SqlServerCeOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected SqlServerCeOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select @@IDENTITY as Id;";
        #endregion
    }



    /// <summary>
    /// Oracleデータベースに対する操作を提供します。
    /// </summary>
    internal class OracleOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected OracleOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
            => this.CreateBulkInsertCommand(data).ExecuteNonQuery();


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
            => this.CreateBulkInsertCommand(data).ExecuteNonQueryAsync();


        /// <summary>
        /// バルク方式で指定のデータを挿入するためのコマンドを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>コマンド</returns>
        private DbCommand CreateBulkInsertCommand<T>(IEnumerable<T> data)
        {
            //--- 実体化
            data = data.Materialize();

            //--- build DbCommand
            var factory = DbProvider.GetFactory(this.DbKind);
            dynamic command = factory.CreateCommand();
            command.Connection = (dynamic)this.Connection;
            command.CommandText = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
            command.BindByName = true;
            command.ArrayBindCount = data.Count();
            if (this.Timeout.HasValue)
                command.CommandTimeout = this.Timeout.Value;

            //--- bind params
            foreach (var x in TableMappingInfo.Create<T>().Columns)
            {
                var getter = AccessorCache<T>.LookupGet(x.PropertyName);
                dynamic parameter = factory.CreateParameter();
                parameter.ParameterName = x.PropertyName;
                parameter.DbType = x.ColumnType;
                parameter.Value = data.Select(y => getter(y)).ToArray();
                command.Parameters.Add(parameter);
            }
            return command;
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// 指定されたレコードをテーブルに挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public override long InsertAndGet<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var param = this.CreateInsertAndGetParameter(data);
            if (param.Item1.ExecuteNonQuery() != 1)
                throw new SystemException("Affected row count is not 1.");
            return Convert.ToInt64(param.Item2.Value);
        }


        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入し、自動採番IDを返します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>自動採番ID</returns>
        public override async Task<long> InsertAndGetAsync<T>(T data)
        {
            This.AssertInsertAndGet<T>();
            var param = this.CreateInsertAndGetParameter(data);
            var result = await param.Item1.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (result != 1)
                throw new SystemException("Affected row count is not 1.");
            return Convert.ToInt64(param.Item2.Value);
        }


        /// <summary>
        /// InsertAndGetするためのパラメーターを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>パラメーター</returns>
        private Tuple<DbCommand, DbParameter> CreateInsertAndGetParameter<T>(T data)
        {
            //--- command
            var factory = DbProvider.GetFactory(this.DbKind);
            dynamic command = factory.CreateCommand();
            command.BindByName = true;
            command.Connection = (dynamic)this.Connection;
            if (this.Timeout.HasValue)
                command.CommandTimeout = this.Timeout.Value;

            //--- parameters
            DbParameter output = null;
            foreach (var x in TableMappingInfo.Create<T>().Columns)
            {
                dynamic parameter = factory.CreateParameter();
                parameter.ParameterName = x.PropertyName;
                parameter.DbType = x.ColumnType;
                if (x.IsPrimaryKey)
                {
                    parameter.Direction = ParameterDirection.Output;
                    output = parameter;
                    command.CommandText =
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)}
returning {x.ColumnName} into :{x.PropertyName}";
                }
                else
                {
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = AccessorCache<T>.LookupGet(x.PropertyName)(data);
                }
                command.Parameters.Add(parameter);
            }

            //--- ok
            return Tuple.Create((DbCommand)command, output);
        }
        #endregion
    }


    /// <summary>
    /// UnmanagedなODP.NETによるOracleデータベースに対する操作を提供します。
    /// </summary>
    internal class UnmanagedOracleOperation : OracleOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected UnmanagedOracleOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion
    }


    /// <summary>
    /// MySqlデータベースに対する操作を提供します。
    /// </summary>
    internal class MySqlOperation : DbOperation
    {
        #region プロパティ
        /// <summary>
        /// 指定された文字列をエスケープするデリゲートを取得します。
        /// </summary>
        private static Func<string, string> Escape { get; } = (Func<string, string>)Delegate.CreateDelegate
        (
            typeof(Func<string, string>),
            AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetType("MySql.Data.MySqlClient.MySqlHelper"))
                .First(x => x != null)
                .GetRuntimeMethod("EscapeString", new []{ typeof(string) })
        );
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected MySqlOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            var sql = this.CreateBulkInsertSql(data);
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定したデータからバルクインサート用のSQL文を生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>SQL文</returns>
        private string CreateBulkInsertSql<T>(IEnumerable<T> data)
        {
            var prefix  = this.DbKind.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create<T>();
            var columnNames = table.Columns.Select(x => "    " + x.ColumnName);
            var builder = new StringBuilder();
            builder.AppendLine($"insert into {table.FullName}");
            builder.AppendLine("(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
            builder.AppendLine(")");
            builder.Append("values");

            var getters = table.Columns.Select(c => AccessorCache<T>.LookupGet(c.PropertyName)).ToArray();
            foreach (var x in data)
            {
                builder.AppendLine();
                builder.Append("(");
                var values = getters.Select(f => ToSqlLiteral(f(x)));
                builder.Append(string.Join(", ", values));
                builder.Append("),");
            }
            builder.Length--;  //--- 最後の「,」を削除

            return builder.ToString();
        }


        /// <summary>
        /// 指定された値をSQL用の文字に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>SQL用の文字列</returns>
        private static string ToSqlLiteral(object value)
        {
            if (value == null)      return "NULL";
            if (value is string)    return $"'{Escape(value.ToString())}'";
            if (value is bool)      return Convert.ToInt32(value).ToString();
            if (value is Enum)      return ((Enum)value).ToString("d");
            if (value is DateTime)  return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            if (value is TimeSpan)  return $"'{((TimeSpan)value).ToString("HH:mm:ss")}'";
            if (value is Guid)      return $"'{value.ToString()}'";
            return Escape(value.ToString());
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select last_insert_id() as Id;";
        #endregion
    }



    /// <summary>
    /// PostgreSqlデータベースに対する操作を提供します。
    /// </summary>
    internal class PostgreSqlOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected PostgreSqlOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
        {
            var sequence = TableMappingInfo.Create<T>().Columns.First(x => x.IsPrimaryKey).Sequence;
            return
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select currval({sequence.FullName}) as Id;";
        }
        #endregion
    }



    /// <summary>
    /// SQLiteデータベースに対する操作を提供します。
    /// </summary>
    internal class SQLiteOperation : DbOperation
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <param name="transaction">トランザクション</param>
        /// <param name="timeout">タイムアウト時間</param>
        protected SQLiteOperation(IDbConnection connection, IDbTransaction transaction, int? timeout)
            : base(connection, transaction, timeout)
        {}
        #endregion


        #region BulkInsert
        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override int BulkInsert<T>(IEnumerable<T> data)
        {
            //--- 挿入処理本体
            Func<IEnumerable<T>, IDbTransaction, int> insert = (collection, transaction) =>
            {
                var result = 0;
                var sql = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
                foreach (var x in collection)
                {
                    var value = this.Connection.Execute(sql, x, transaction, this.Timeout);
                    Interlocked.Add(ref result, value);
                }
                return result;
            };

            //--- トランザクションが外部から指定されている場合はそれを利用
            if (this.Transaction != null)
                return insert(data, this.Transaction);

            //--- トランザクションが外部から指定されていない場合は新規に作成
            //--- SQLiteにおけるバルクインサートの魔法
            using (var transaction = this.Connection.StartTransaction())
            {
                var result = insert(data, transaction.Raw);
                transaction.Complete();
                return result;
            }
        }


        /// <summary>
        /// 指定されたレコードをバルク方式でテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <returns>影響した行数</returns>
        public override async Task<int> BulkInsertAsync<T>(IEnumerable<T> data)
        {
            //--- 挿入処理本体
            Func<IEnumerable<T>, IDbTransaction, Task<int>> insert = async (collection, transaction) =>
            {
                var result = 0;
                var sql = PrimitiveSql.CreateInsert<T>(this.DbKind, false, true);
                foreach (var x in collection)
                {
                    var value = await this.Connection.ExecuteAsync(sql, x, transaction, this.Timeout).ConfigureAwait(false);
                    Interlocked.Add(ref result, value);
                }
                return result;
            };

            //--- トランザクションが外部から指定されている場合はそれを利用
            if (this.Transaction != null)
                return await insert(data, this.Transaction).ConfigureAwait(false);

            //--- トランザクションが外部から指定されていない場合は新規に作成
            //--- SQLiteにおけるバルクインサートの魔法
            using (var transaction = this.Connection.StartTransaction())
            {
                var result = await insert(data, transaction.Raw).ConfigureAwait(false);
                transaction.Complete();
                return result;
            }
        }
        #endregion


        #region InsertAndGet
        /// <summary>
        /// レコードを挿入し、そのレコードに自動採番されたIDを取得するSQLを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>SQL文</returns>
        protected override string CreateInsertAndGetSql<T>()
            =>
$@"{PrimitiveSql.CreateInsert<T>(this.DbKind)};
select last_insert_rowid() as Id;";
        #endregion
    }
}