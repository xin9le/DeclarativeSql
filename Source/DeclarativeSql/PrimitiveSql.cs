using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;
using This = DeclarativeSql.PrimitiveSql;



namespace DeclarativeSql
{
    /// <summary>
    /// プリミティブなSQLの自動生成機能を提供します。
    /// </summary>
    public static class PrimitiveSql
    {
        #region Count
        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコード数をカウントするクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <returns>生成されたSQL</returns>
        public static string CreateCount<T>()
        {
            return This.CreateCount(typeof(T));
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコード数をカウントするクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateCount(Type type)
        {
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
        public static string CreateSelect<T>(params Expression<Func<T, object>>[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var propertyNames   = properties.Length == 0
                                ? new string[0]
                                : ExpressionHelper.GetMemberNames(properties).ToArray();
            return This.CreateSelect(typeof(T), propertyNames);
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="propertyNames">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateSelect(Type type, params string[] propertyNames)
        {
            if (type == null)           throw new ArgumentNullException(nameof(type));
            if (propertyNames == null)  throw new ArgumentNullException(nameof(propertyNames));

            var table   = TableMappingInfo.Create(type);
            var columns = propertyNames.Length == 0
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
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateInsert<T>(DbKind targetDatabase, bool useSequence = true, bool setIdentity = false)
        {
            return This.CreateInsert(targetDatabase, typeof(T), useSequence, setIdentity);
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateInsert(DbKind targetDatabase, Type type, bool useSequence = true, bool setIdentity = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var prefix  = targetDatabase.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsIdentity);
            var values  = columns.Select(x =>
                        {
                            if (useSequence)
                            if (x.Sequence != null)
                            switch (targetDatabase)
                            {
                                case DbKind.SqlServer:  return $"next value for {x.Sequence.FullName}";
                                case DbKind.Oracle:     return $"{x.Sequence.FullName}.nextval";
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
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="properties">プロパティ式のコレクション</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate<T>(DbKind targetDatabase, params Expression<Func<T, object>>[] properties)
        {
            return This.CreateUpdate<T>(targetDatabase, false, properties);
        }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="propertyNames">プロパティ名のコレクション</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate(DbKind targetDatabase, Type type, params string[] propertyNames)
        {
            return This.CreateUpdate(targetDatabase, type, false, propertyNames);
         }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="properties">プロパティ式のコレクション</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate<T>(DbKind targetDatabase, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var propertyNames = ExpressionHelper.GetMemberNames(properties).ToArray();
            return This.CreateUpdate(targetDatabase, typeof(T), setIdentity, propertyNames);
        }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="setIdentity">自動連番のID列に値を設定するかどうか</param>
        /// <param name="propertyNames">プロパティ名のコレクション</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate(DbKind targetDatabase, Type type, bool setIdentity, params string[] propertyNames)
        {
            if (type == null)           throw new ArgumentNullException(nameof(type));
            if (propertyNames == null)  throw new ArgumentNullException(nameof(propertyNames));

            var prefix  = targetDatabase.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsIdentity);
            if (propertyNames.Length != 0)
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
        public static string CreateDelete<T>()
        {
            return This.CreateDelete(typeof(T));
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateDelete(Type type)
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
        public static string CreateTruncate<T>()
        {
            return This.CreateTruncate(typeof(T));
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを切り捨てるクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateTruncate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"truncate table {table.FullName}";
        }
        #endregion
    }
}