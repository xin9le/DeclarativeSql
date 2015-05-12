using System;
using System.Collections.Generic;
using System.Data;
using This = DeclarativeSql.Mapping.TypeMap;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// CLR型とDB型とのマッピング情報を提供します。
    /// </summary>
    internal class TypeMap : Dictionary<Type, DbType>
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        private TypeMap()
        {}
        #endregion


        #region 取得
        /// <summary>
        /// 既定のマッピング情報を生成します。
        /// </summary>
        /// <returns>型マッピング情報</returns>
        public static This Default() => new This
        {
            [typeof(byte)]              = DbType.Byte,
            [typeof(sbyte)]             = DbType.SByte,
            [typeof(short)]             = DbType.Int16,
            [typeof(ushort)]            = DbType.UInt16,
            [typeof(int)]               = DbType.Int32,
            [typeof(uint)]              = DbType.UInt32,
            [typeof(long)]              = DbType.Int64,
            [typeof(ulong)]             = DbType.UInt64,
            [typeof(float)]             = DbType.Single,
            [typeof(double)]            = DbType.Double,
            [typeof(decimal)]           = DbType.Decimal,
            [typeof(bool)]              = DbType.Boolean,
            [typeof(string)]            = DbType.String,
            [typeof(char)]              = DbType.StringFixedLength,
            [typeof(Guid)]              = DbType.Guid,
            [typeof(DateTime)]          = DbType.DateTime,
            [typeof(DateTimeOffset)]    = DbType.DateTimeOffset,
            [typeof(TimeSpan)]          = DbType.Time,
            [typeof(byte[])]            = DbType.Binary,
            [typeof(byte?)]             = DbType.Byte,
            [typeof(sbyte?)]            = DbType.SByte,
            [typeof(short?)]            = DbType.Int16,
            [typeof(ushort?)]           = DbType.UInt16,
            [typeof(int?)]              = DbType.Int32,
            [typeof(uint?)]             = DbType.UInt32,
            [typeof(long?)]             = DbType.Int64,
            [typeof(ulong?)]            = DbType.UInt64,
            [typeof(float?)]            = DbType.Single,
            [typeof(double?)]           = DbType.Double,
            [typeof(decimal?)]          = DbType.Decimal,
            [typeof(bool?)]             = DbType.Boolean,
            [typeof(char?)]             = DbType.StringFixedLength,
            [typeof(Guid?)]             = DbType.Guid,
            [typeof(DateTime?)]         = DbType.DateTime,
            [typeof(DateTimeOffset?)]   = DbType.DateTimeOffset,
            [typeof(TimeSpan?)]         = DbType.Time,
            [typeof(object)]            = DbType.Object,
        };
        #endregion
    }
}