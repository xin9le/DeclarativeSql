using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DeclarativeSql.Annotations;
using This = DeclarativeSql.DbKindExtensions;



namespace DeclarativeSql
{
    /// <summary>
    /// DbKindの拡張機能を提供します。
    /// </summary>
    public static class DbKindExtensions
    {
        #region 内部クラス
        /// <summary>
        /// 事前読み込み用の設定を表します。
        /// </summary>
        private class Setting
        {
            #region プロパティ
            /// <summary>
            /// データベースの種類を取得または設定します。
            /// </summary>
            public DbKind DbKind { get; set; }


            /// <summary>
            /// データプロバイダー名を取得または設定します。
            /// </summary>
            public string ProviderName { get; set; }


            /// <summary>
            /// DbConnectionの型名を取得または設定します。
            /// </summary>
            public string DbConnectionTypeName { get; set; }


            /// <summary>
            /// バインド変数の接頭辞を取得または設定します。
            /// </summary>
            public char? BindParameterPrefix { get; set; }
            #endregion
        }
        #endregion


        #region フィールド
        /// <summary>
        /// 設定を保持します。
        /// </summary>
        private static readonly IDictionary<DbKind, Setting> cache = new Dictionary<DbKind, Setting>();
        #endregion


        #region コンストラクタ
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DbKindExtensions()
        {
            This.Prefetch();
        }
        #endregion


        #region 事前読み込み
        /// <summary>
        /// 設定をキャッシュします。
        /// </summary>
        private static void Prefetch()
        {
            var enumType = typeof(DbKind);
            foreach (DbKind value in Enum.GetValues(enumType))
            {
                var name    = Enum.GetName(enumType, value);
                var field   = enumType.GetRuntimeField(name);
                var providerNameAttribute   = field.GetCustomAttribute<ProviderNameAttribute>();
                var dbConnectionAttribute   = field.GetCustomAttribute<DbConnectionAttribute>();
                var prefixAttribute         = field.GetCustomAttribute<BindParameterPrefixAttribute>();
                This.cache.Add(value, new Setting()
                {
                    DbKind                  = value,
                    ProviderName            = providerNameAttribute?.ProviderName,
                    DbConnectionTypeName    = dbConnectionAttribute?.TypeName,
                    BindParameterPrefix     = prefixAttribute?.Prefix,
                });
            }
        }
        #endregion


        #region 拡張メソッド
        /// <summary>
        /// ProviderInvariantName属性からデータソースのデータプロバイダー名を取得します。
        /// </summary>
        /// <param name="kind">データベースの種類</param>
        /// <returns>データプロバイダー名</returns>
        public static string GetProviderName(this DbKind kind) => This.cache[kind].ProviderName;


        /// <summary>
        /// DbConnection属性からDbConnectionの型名を取得します。
        /// </summary>
        /// <param name="kind">データベースの種類</param>
        /// <returns>DbConnectionの型名</returns>
        public static string GetDbConnectionTypeName(this DbKind kind)
        {
            var setting = This.cache[kind];
            if (setting.ProviderName == null)           return null;
            if (setting.DbConnectionTypeName == null)   return null;
            return $"{setting.ProviderName}.{setting.DbConnectionTypeName}";
        }


        /// <summary>
        /// バインド変数の接頭辞を取得します。
        /// </summary>
        /// <param name="kind">データベースの種類</param>
        /// <returns>バインド変数の接頭辞</returns>
        internal static char GetBindParameterPrefix(this DbKind kind)
        {
            var prefix = This.cache[kind].BindParameterPrefix;
            if (!prefix.HasValue)
                throw new NotSupportedException();
            return prefix.Value;
        }


        /// <summary>
        /// 指定されたDbConnectionが対象としているデータベースの種類を取得します。
        /// </summary>
        /// <param name="connection">データベース接続</param>
        /// <returns>データベースの種類</returns>
        public static DbKind GetDbKind(this IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var fullName = connection.GetType().FullName;
            return  This.cache.Values
                    .Select(x => x.DbKind)
                    .FirstOrDefault(x => x.GetDbConnectionTypeName() == fullName);
        }
        #endregion
    }
}