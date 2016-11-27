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
    /// Provides mapping information between columns and properties.
    /// </summary>
    public sealed class ColumnMappingInfo
    {
        #region Fields
        /// <summary>
        /// Holds the default type mapping information.
        /// </summary>
        private readonly static IReadOnlyDictionary<Type, DbType> typeMap = TypeMap.Default();
        #endregion


        #region Properties
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; private set; }


        /// <summary>
        /// Gets the property type.
        /// </summary>
        public Type PropertyType { get; private set; }


        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; private set; }


        /// <summary>
        /// Gets the column type.
        /// </summary>
        public DbType ColumnType { get; private set; }


        /// <summary>
        /// Gets whether it is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; private set; }


        /// <summary>
        /// Gets whether to allow null values.
        /// </summary>
        public bool IsNullable { get; private set; }


        /// <summary>
        /// Gets whether it is an automatic number assignment ID.
        /// </summary>
        public bool IsAutoIncrement { get; private set; }


        /// <summary>
        /// Gets sequence.
        /// </summary>
        public SequenceMappingInfo Sequence { get; private set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        private ColumnMappingInfo()
        { }
        #endregion


        #region Generates
        /// <summary>
        /// Creates instance from the specified property information.
        /// </summary>
        /// <param name="info">Property information</param>
        /// <returns>Mapping instance of column information</returns>
        public static This From(PropertyInfo info)
        {
            var isPrimary   = info.IsDefined<KeyAttribute>();
            var required    = info.IsDefined<RequiredAttribute>();
            var sequence    = info.GetCustomAttribute<SequenceAttribute>();
            var propType    = info.PropertyType.GetTypeInfo().IsEnum
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
                IsAutoIncrement = (This.GetDatabaseGeneratedOption(info) == DatabaseGeneratedOption.Identity) || info.IsDefined<AutoIncrementAttribute>(),
                Sequence        = (sequence == null) ? null : SequenceMappingInfo.From(sequence),
            };
        }
        #endregion


        #region Supports
        /// <summary>
        /// Gets the column name from the specified property.
        /// </summary>
        /// <param name="info">Property information</param>
        /// <returns>Column name</returns>
        private static string GetColumnName(PropertyInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var attribute = info.GetCustomAttribute<ColumnAttribute>(false);
            return attribute?.Name ?? info.Name;
        }


        /// <summary>
        /// Gets the value of <see cref="DataGeneratedOption"/> attached to the specified property.
        /// </summary>
        /// <param name="info">Property information</param>
        /// <returns>The value of <see cref="DataGeneratedOption"/></returns>
        /// <remarks>If the attribute is not given, <see cref="DataGeneratedOption.None"/> is returned.</remarks>
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