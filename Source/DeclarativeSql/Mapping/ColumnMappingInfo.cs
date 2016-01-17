using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using DeclarativeSql.Annotations;
using DeclarativeSql.Helpers;
using This = DeclarativeSql.Mapping.ColumnMappingInfo;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// 列とプロパティとのマッピング情報を提供します。
    /// </summary>
    public sealed class ColumnMappingInfo
    {
        #region フィールド
        /// <summary>
        /// 既定の型マッピング情報を保持します。
        /// </summary>
        private readonly static IReadOnlyDictionary<Type, DbType> typeMap = TypeMap.Default();
        #endregion


        #region プロパティ
        /// <summary>
        /// プロパティ名を取得します。
        /// </summary>
        public string PropertyName { get; private set; }


        /// <summary>
        /// プロパティの型を取得します。
        /// </summary>
        public Type PropertyType { get; private set; }


        /// <summary>
        /// 列名を取得します。
        /// </summary>
        public string ColumnName { get; private set; }


        /// <summary>
        /// 列の型を取得します。
        /// </summary>
        public DbType ColumnType { get; private set; }


        /// <summary>
        /// 主キーかどうかを取得します。
        /// </summary>
        public bool IsPrimaryKey { get; private set; }


        /// <summary>
        /// Null値を許可しているかどうかを取得します。
        /// </summary>
        public bool IsNullable { get; private set; }


        /// <summary>
        /// 自動採番IDかどうかを取得します。
        /// </summary>
        public bool IsAutoIncrement { get; private set; }


        /// <summary>
        /// シーケンスを取得します。
        /// </summary>
        public SequenceMappingInfo Sequence { get; private set; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        private ColumnMappingInfo()
        { }
        #endregion


        #region 生成
        /// <summary>
        /// 指定されたプロパティ情報からインスタンスを生成します。
        /// </summary>
        /// <param name="info">プロパティ情報</param>
        /// <returns>列情報のマッピングインスタンス</returns>
        public static This From(PropertyInfo info)
        {
            var isPrimary   = info.Has<KeyAttribute>();
            var required    = info.Has<RequiredAttribute>();
            var sequence    = info.GetCustomAttribute<SequenceAttribute>();
            var propType    = info.PropertyType.IsEnum
                            ? Enum.GetUnderlyingType(info.PropertyType)
                            : info.PropertyType;
            return new This()
            {
                PropertyName    = info.Name,
                PropertyType    = info.PropertyType,
                ColumnName      = This.GetColumnName(info),
                ColumnType      = This.typeMap[propType],
                IsPrimaryKey    = isPrimary,
                IsNullable      = !(isPrimary || required),
                IsAutoIncrement = (This.GetDatabaseGeneratedOption(info) == DatabaseGeneratedOption.Identity) || info.Has<AutoIncrementAttribute>(),
                Sequence        = (sequence == null) ? null : SequenceMappingInfo.From(sequence),
            };
        }
        #endregion


        #region 補助
        /// <summary>
        /// 指定されたプロパティから列名を取得します。
        /// </summary>
        /// <param name="info">プロパティ情報</param>
        /// <returns>列名</returns>
        private static string GetColumnName(PropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var attribute = info.GetCustomAttribute<ColumnAttribute>(false);
            return attribute?.Name ?? info.Name;
        }


        /// <summary>
        /// 指定されたプロパティに付与されているDataGeneratedOptionの値を取得します。
        /// </summary>
        /// <param name="info">プロパティ情報</param>
        /// <returns>DataGeneratedOptionの値</returns>
        /// <remarks>属性が付与されていない場合はNoneが返ります。</remarks>
        private static DatabaseGeneratedOption GetDatabaseGeneratedOption(PropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var attribute = info.GetCustomAttribute<DatabaseGeneratedAttribute>(false);
            return attribute?.DatabaseGeneratedOption ?? DatabaseGeneratedOption.None;
        }
        #endregion
    }
}