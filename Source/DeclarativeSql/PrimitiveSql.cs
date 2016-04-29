﻿using System;
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
        /// <param name="targetDatabase">対象データベース</param>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <returns>生成されたSQL</returns>
        public static string CreateCount<T>(DbKind targetDatabase) => This.CreateCount(targetDatabase, typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコード数をカウントするクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateCount(DbKind targetDatabase, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"select count(*) as Count from {table.FullName(targetDatabase)}";
        }
        #endregion


        #region Select
        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateSelect<T>(DbKind targetDatabase, Expression<Func<T, object>> properties = null)
        {
            var propertyNames   = properties == null
                                ? null
                                : ExpressionHelper.GetMemberNames(properties);
            return This.CreateSelect(targetDatabase, typeof(T), propertyNames);
        }

        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="propertyNames">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateSelect(DbKind targetDatabase, Type type, IEnumerable<string> propertyNames = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (propertyNames == null) propertyNames = Enumerable.Empty<string>();

            var table = TableMappingInfo.Create(type);
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
            builder.Append($"from {table.FullName(targetDatabase)}");
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
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateInsert<T>(DbKind targetDatabase, bool useSequence = true, bool setIdentity = false)
            => This.CreateInsert(targetDatabase, typeof(T), useSequence, setIdentity);


        /// <summary>
        /// 指定された型情報から対象となるテーブルにレコードを挿入するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="useSequence">シーケンスを利用するかどうか</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateInsert(DbKind targetDatabase, Type type, bool useSequence = true, bool setIdentity = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var prefix  = targetDatabase.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
            var values  = columns.Select(x =>
                        {
                            if (useSequence)
                            if (x.Sequence != null)
                            switch (targetDatabase)
                            {
                                case DbKind.SqlServer:  return $"next value for {x.Sequence.FullName}";
                                case DbKind.Oracle:     return $"{x.Sequence.FullName}.nextval";
                                case DbKind.PostgreSql: return $"nextval('{x.Sequence.FullName}')";
                           }
                            return $"{prefix}{x.PropertyName}";
                        })
                        .Select(x => "    " + x);
            var columnNames = columns.Select(x => "    " + x.ColumnName);
            var builder = new StringBuilder();
            builder.AppendLine($"insert into {table.FullName(targetDatabase)}");
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
        /// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate<T>(DbKind targetDatabase, Expression<Func<T, object>> properties = null, bool setIdentity = false)
        {
            var propertyNames   = properties == null
                                ? null
                                : ExpressionHelper.GetMemberNames(properties);
            return This.CreateUpdate(targetDatabase, typeof(T), propertyNames, setIdentity);
        }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="propertyNames">プロパティ名のコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateUpdate(DbKind targetDatabase, Type type, IEnumerable<string> propertyNames = null, bool setIdentity = false)
        {
            if (type == null)           throw new ArgumentNullException(nameof(type));
            if (propertyNames == null)  propertyNames = Enumerable.Empty<string>();

            var prefix  = targetDatabase.GetBindParameterPrefix();
            var table   = TableMappingInfo.Create(type);
            var columns = table.Columns.Where(x => setIdentity ? true : !x.IsAutoIncrement);
            if (propertyNames.Any())
                columns = columns.Join(propertyNames, x => x.PropertyName, y => y, (x, y) => x);
            var setters = columns.Select(x => $"    {x.ColumnName} = {prefix}{x.PropertyName}");
            var builder = new StringBuilder();
            builder.AppendLine($"update {table.FullName(targetDatabase)}");
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
        /// <param name="targetDatabase">対象データベース</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateDelete<T>(DbKind targetDatabase) => This.CreateDelete(targetDatabase, typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを削除するクエリを生成します。
        /// </summary>
        /// <param name="type">テーブルの型</param>
        /// <param name="targetDatabase">対象データベース</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateDelete(DbKind targetDatabase, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"delete from {table.FullName(targetDatabase)}";
        }
        #endregion


        #region Truncate
        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを切り捨てるクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象データベース</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateTruncate<T>(DbKind targetDatabase) => This.CreateTruncate(targetDatabase, typeof(T));


        /// <summary>
        /// 指定された型情報から対象となるテーブルのすべてのレコードを切り捨てるクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <returns>生成されたSQL</returns>
        public static string CreateTruncate(DbKind targetDatabase, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var table = TableMappingInfo.Create(type);
            return $"truncate table {table.FullName(targetDatabase)}";
        }
        #endregion


        #region Obsolete
#pragma warning disable 0618
        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="properties">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateSelect<T>(Expression<Func<T, object>> properties = null) を利用してください。")]
        public static string CreateSelect<T>(params Expression<Func<T, object>>[] properties)
        {
            var propertyNames   = properties == null || properties.Length == 0
                                ? null
                                : ExpressionHelper.GetMemberNames(properties);
            return This.CreateSelect(DbKind.Unknown, typeof(T), propertyNames);
        }


        /// <summary>
        /// 指定された型情報から対象となるテーブルのレコードを取得するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="propertyNames">抽出する列にマッピングされるプロパティのコレクション。指定がない場合はすべての列を抽出対象とします。</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateSelect(Type type, IEnumerable<string> propertyNames = null) を利用してください。")]
        public static string CreateSelect(Type type, params string[] propertyNames)
            => This.CreateSelect(DbKind.Unknown, type, (IEnumerable<string>)propertyNames);


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="properties">プロパティ式のコレクション</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateUpdate<T>(DbKind targetDatabase, Expression<Func<T, object>> properties = null, bool setIdentity = false) を利用してください。")]
        public static string CreateUpdate<T>(DbKind targetDatabase, params Expression<Func<T, object>>[] properties)
            => This.CreateUpdate<T>(targetDatabase, false, properties);


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="propertyNames">プロパティ名のコレクション</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateUpdate(DbKind targetDatabase, Type type, IEnumerable<string> propertyNames = null, bool setIdentity = false) を利用してください。")]
        public static string CreateUpdate(DbKind targetDatabase, Type type, params string[] propertyNames)
            => This.CreateUpdate(targetDatabase, type, propertyNames, false);


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <param name="properties">プロパティ式のコレクション</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateUpdate<T>(DbKind targetDatabase, Expression<Func<T, object>> properties = null, bool setIdentity = false) を利用してください。")]
        public static string CreateUpdate<T>(DbKind targetDatabase, bool setIdentity, params Expression<Func<T, object>>[] properties)
        {
            var propertyNames   = properties == null || properties.Length == 0
                                ? null
                                : ExpressionHelper.GetMemberNames(properties);
            return This.CreateUpdate(targetDatabase, typeof(T), propertyNames, setIdentity);
        }


        /// <summary>
        /// 指定された型情報から、対象となるテーブルのレコードを指定されたプロパティにマッピングされている列に絞って更新するクエリを生成します。
        /// </summary>
        /// <param name="targetDatabase">対象データベース</param>
        /// <param name="type">テーブルの型</param>
        /// <param name="setIdentity">自動採番のID列に値を設定するかどうか</param>
        /// <param name="propertyNames">プロパティ名のコレクション</param>
        /// <returns>生成されたSQL</returns>
        [Obsolete("CreateUpdate(DbKind targetDatabase, Type type, IEnumerable<string> propertyNames = null, bool setIdentity = false) を利用してください。")]
        public static string CreateUpdate(DbKind targetDatabase, Type type, bool setIdentity, params string[] propertyNames)
            => This.CreateUpdate(targetDatabase, type, propertyNames, setIdentity);
#pragma warning restore 0618
        #endregion
    }
}