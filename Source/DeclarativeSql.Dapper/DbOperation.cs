using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DeclarativeSql.Helpers;



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
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        protected DbOperation(IDbConnection connection)
        {
            this.Connection = connection;
            this.DbKind = connection.GetDbKind();
        }


        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        protected DbOperation(IDbTransaction transaction)
        {
            this.Connection = transaction.Connection;
            this.Transaction = transaction;
            this.DbKind = transaction.Connection.GetDbKind();
        }
        #endregion


        #region 生成
        /// <summary>
        /// データベース接続からデータベース操作のインスタンスを生成します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <returns>データベース操作</returns>
        public static DbOperation Create(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            //--- todo : create DbOperation for specified database if need customization.
            return new DbOperation(connection);
        }


        /// <summary>
        /// トランザクションからデータベース操作のインスタンスを生成します。
        /// </summary>
        /// <param name="transaction">トランザクション</param>
        /// <returns>データベース操作</returns>
        public static DbOperation Create(IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            //--- todo : create DbOperation for specified database if need customization.
            return new DbOperation(transaction);
        }
        #endregion


        #region 同期
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>レコード数</returns>
        public virtual ulong Count<T>()
        {
            var sql = PrimitiveSql.CreateCount<T>();
            return this.Connection.ExecuteScalar<ulong>(sql, null, this.Transaction);
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
            return this.Connection.ExecuteScalar<ulong>(builder.ToString(), where.Parameter, this.Transaction);
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual IReadOnlyList<T> Select<T>(Expression<Func<T, object>>[] properties)
        {
            var sql = PrimitiveSql.CreateSelect(properties);
            return this.Connection.Query<T>(sql, null, this.Transaction) as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual IReadOnlyList<T> Select<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] properties)
        {
            var select  = PrimitiveSql.CreateSelect(properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(select);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.Query<T>(builder.ToString(), where.Parameter, this.Transaction) as IReadOnlyList<T>;
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual int Insert<T>(T data, bool useSequence, bool setIdentity)
        {
            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(this.DbKind, type, useSequence, setIdentity);
            return this.Connection.Execute(sql, data, this.Transaction);
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public virtual int Update<T>(T data, bool setIdentity, Expression<Func<T, object>>[] properties)
        {
            var sql = PrimitiveSql.CreateUpdate(this.DbKind, setIdentity, properties);
            return this.Connection.Execute(sql, data, this.Transaction);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public virtual int Update<T>(T data, Expression<Func<T, bool>> predicate, bool setIdentity, Expression<Func<T, object>>[] properties)
        {
            var update  = PrimitiveSql.CreateUpdate(this.DbKind, setIdentity, properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var param   = where.Parameter.Merge(data, properties);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.Execute(builder.ToString(), param, this.Transaction);
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
            return this.Connection.Execute(sql, null, this.Transaction);
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
            return this.Connection.Execute(builder.ToString(), where.Parameter, this.Transaction);
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
            return this.Connection.Execute(sql, null, this.Transaction);
        }
        #endregion
        #endregion


        #region 非同期
        #region Count
        /// <summary>
        /// 指定されたテーブルのレコード数を非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>レコード数</returns>
        public virtual Task<ulong> CountAsync<T>()
        {
            var sql = PrimitiveSql.CreateCount<T>();
            return this.Connection.ExecuteScalarAsync<ulong>(sql, null, this.Transaction);
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
            return this.Connection.ExecuteScalarAsync<ulong>(builder.ToString(), where.Parameter, this.Transaction);
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual async Task<IReadOnlyList<T>> SelectAsync<T>(Expression<Func<T, object>>[] properties)
        {
            var sql = PrimitiveSql.CreateSelect(properties);
            var result = await this.Connection.QueryAsync<T>(sql, null, this.Transaction).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }


        /// <summary>
        /// 指定されたテーブルから指定の条件に一致するレコードを非同期的に取得します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="predicate">抽出条件</param>
        /// <param name="properties">取得対象の列</param>
        /// <returns>取得したレコード</returns>
        public virtual async Task<IReadOnlyList<T>> SelectAsync<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] properties)
        {
            var select  = PrimitiveSql.CreateSelect(properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var builder = new StringBuilder();
            builder.AppendLine(select);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            var result = await this.Connection.QueryAsync<T>(builder.ToString(), where.Parameter, this.Transaction).ConfigureAwait(false);
            return result as IReadOnlyList<T>;
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定されたレコードをテーブルに非同期的に挿入します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <param name="data">挿入するデータ</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> InsertAsync<T>(T data, bool useSequence, bool setIdentity)
        {
            var type = TypeHelper.GetElementType<T>() ?? typeof(T);
            var sql = PrimitiveSql.CreateInsert(this.DbKind, type, useSequence, setIdentity);
            return this.Connection.ExecuteAsync(sql, data, this.Transaction);
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された情報でレコードを非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> UpdateAsync<T>(T data, bool setIdentity, Expression<Func<T, object>>[] properties)
        {
            var sql = PrimitiveSql.CreateUpdate(this.DbKind, setIdentity, properties);
            return this.Connection.ExecuteAsync(sql, data, this.Transaction);
        }


        /// <summary>
        /// 指定の条件に一致するレコードを指定された情報で非同期的に更新します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="data">更新するデータ</param>
        /// <param name="predicate">更新条件</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">更新する列にマッピングされるプロパティ式のコレクション</param>
        /// <returns>影響した行数</returns>
        public virtual Task<int> UpdateAsync<T>(T data, Expression<Func<T, bool>> predicate, bool setIdentity, Expression<Func<T, object>>[] properties)
        {
            var update  = PrimitiveSql.CreateUpdate(this.DbKind, setIdentity, properties);
            var where   = PredicateSql.From(this.DbKind, predicate);
            var param   = where.Parameter.Merge(data, properties);
            var builder = new StringBuilder();
            builder.AppendLine(update);
            builder.AppendLine(nameof(where));
            builder.Append($"    {where.Statement}");
            return this.Connection.ExecuteAsync(builder.ToString(), param, this.Transaction);
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定されたテーブルからすべてのレコードを非同期的に削除します。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>影響した行数</returns>
        public virtual Task<int> DeleteAsync<T>()
        {
            var sql = PrimitiveSql.CreateDelete<T>();
            return this.Connection.ExecuteAsync(sql, null, this.Transaction);
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
            return this.Connection.ExecuteAsync(builder.ToString(), where.Parameter, this.Transaction);
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定されたテーブルを非同期的に切り捨てます。
        /// </summary>
        /// <typeparam name="T">テーブルにマッピングされた型</typeparam>
        /// <returns>-1</returns>
        public virtual Task<int> TruncateAsync<T>()
        {
            var sql = PrimitiveSql.CreateTruncate<T>();
            return this.Connection.ExecuteAsync(sql, null, this.Transaction);
        }
        #endregion
        #endregion
    }
}