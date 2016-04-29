﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
            var sql = PrimitiveSql.CreateCount<T>(this.DbKind);
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
            var count = PrimitiveSql.CreateCount<T>(this.DbKind);
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
            var sql = PrimitiveSql.CreateCount<T>(this.DbKind);
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
            var count = PrimitiveSql.CreateCount<T>(this.DbKind);
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
            var sql = PrimitiveSql.CreateSelect(this.DbKind, properties);
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
            var select  = PrimitiveSql.CreateSelect(this.DbKind, properties);
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
            var sql = PrimitiveSql.CreateSelect(this.DbKind, properties);
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
            var select  = PrimitiveSql.CreateSelect(this.DbKind, properties);
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
            this.AssertInsertAndGet<T>();
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
            this.AssertInsertAndGet<T>();
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
        public virtual void AssertInsertAndGet<T>()
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
            var sql = PrimitiveSql.CreateDelete<T>(this.DbKind);
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
            var delete  = PrimitiveSql.CreateDelete<T>(this.DbKind);
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
            var sql = PrimitiveSql.CreateDelete<T>(this.DbKind);
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
            var delete  = PrimitiveSql.CreateDelete<T>(this.DbKind);
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
            var sql = PrimitiveSql.CreateTruncate<T>(this.DbKind);
            return this.Connection.Execute(sql, null, this.Transaction, this.Timeout);
        }


        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>-1</returns>
        public virtual Task<int> TruncateAsync<T>()
        {
            var sql = PrimitiveSql.CreateTruncate<T>(this.DbKind);
            return this.Connection.ExecuteAsync(sql, null, this.Transaction, this.Timeout);
        }
        #endregion
    }
}