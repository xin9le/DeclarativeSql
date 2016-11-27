using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides primitive sql generation functions.
    /// </summary>
    public class SqlGenerator
    {
        #region Properties
        /// <summary>
        /// Gets database provider.
        /// </summary>
        private DbProvider DbProvider { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="dbProvider">Database provider</param>
        internal SqlGenerator(DbProvider dbProvider)
        {
            this.DbProvider = dbProvider;
        }
        #endregion


        #region Count
        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコード数をカウントするクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <returns>生成されたSQL</returns>
        public string CreateCount<T>() => this.CreateCount(typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコード数をカウントするクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public string CreateCount(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"select count(*) as Count from {table.FullName}";
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        public string CreateSelect<T>(Expression<Func<T, object>> properties = null)
        {
            var propertyNames = properties == null
                              ? null
                              : ExpressionHelper.GetMemberNames(properties);
            return this.CreateSelect(typeof(T), propertyNames);
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="propertyNames">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        public string CreateSelect(Type type, IEnumerable<string> propertyNames = null)
        {
            if (type == null)           throw new ArgumentNullException(nameof(type));
            if (propertyNames == null)  propertyNames = Enumerable.Empty<string>();

            var table   = TableMappingInfo.Create(type);
            var columns = propertyNames.IsEmpty()
                        ? table.Columns
                        : table.Columns.Join
                        (
                            propertyNames,
                            x => x.PropertyName,
                            y => y,
                            (x, y) => x
                        );
            var columnNames = columns.Select(x => $"    {x.ColumnName} as {x.PropertyName}");
            var builder = new StringBuilder();
            builder.AppendLine("select");
            builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
            builder.Append($"from {table.FullName}");
            return builder.ToString();
        }
        #endregion


        #region Insert
        /// <summary>
        /// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public string CreateInsert<T>(bool useSequence = true, bool setIdentity = false)
            => this.CreateInsert(typeof(T), useSequence, setIdentity);


        /// <summary>
        /// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public string CreateInsert(Type type, bool useSequence = true, bool setIdentity = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var prefix  = this.DbProvider.BindParameterPrefix;
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
            var values  = columns.Select(x =>
                        {
                            if (useSequence)
                            if (x.Sequence != null)
                            switch (this.DbProvider.Kind)
                            {
                                case DbKind.SqlServer:  return $"next value for {x.Sequence.FullName}";
                              //case DbKind.Oracle:     return $"{x.Sequence.FullName}.nextval";
                              //case DbKind.PostgreSql: return $"nextval('{x.Sequence.FullName}')";
                            }
                            return $"{prefix}{x.PropertyName}";
                        })
                        .Select(x => "    " + x);
            var columnNames = columns.Select(x => "    " + x.ColumnName);
            var builder = new StringBuilder();
            builder.AppendLine($"insert into {table.FullName}");
            builder.AppendLine("(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", columnNames));
            builder.AppendLine(")");
            builder.AppendLine("values");
            builder.AppendLine("(");
            builder.AppendLine(string.Join($",{Environment.NewLine}", values));
            builder.Append(")");
            return builder.ToString();
        }
        #endregion


        #region Update
        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public string CreateUpdate<T>(Expression<Func<T, object>> properties = null, bool setIdentity = false)
        {
            var propertyNames = properties == null
                              ? null
                              : ExpressionHelper.GetMemberNames(properties);
            return this.CreateUpdate(typeof(T), propertyNames, setIdentity);
        }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <param name="propertyNames">プロパティ名のコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public string CreateUpdate(Type type, IEnumerable<string> propertyNames = null, bool setIdentity = false)
        {
            if (type == null)           throw new ArgumentNullException(nameof(type));
            if (propertyNames == null)  propertyNames = Enumerable.Empty<string>();

            var prefix  = this.DbProvider.BindParameterPrefix;
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
            if (propertyNames.Any())
                columns = columns.Join(propertyNames, x => x.PropertyName, y => y, (x, y) => x);
            var setters = columns.Select(x => $"    {x.ColumnName} = {prefix}{x.PropertyName}");
            var builder = new StringBuilder();
            builder.AppendLine($"update {table.FullName}");
            builder.AppendLine("set");
            builder.Append(string.Join($",{Environment.NewLine}", setters));
            return builder.ToString();
        }
        #endregion


        #region Delete
        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <returns>生成されたSQL</returns>
        public string CreateDelete<T>() => this.CreateDelete(typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public string CreateDelete(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"delete from {table.FullName}";
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを切り捨てるクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <returns>生成されたSQL</returns>
        public string CreateTruncate<T>() => this.CreateTruncate(typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを切り捨てるクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public string CreateTruncate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"truncate table {table.FullName}";
        }
        #endregion
    }
}