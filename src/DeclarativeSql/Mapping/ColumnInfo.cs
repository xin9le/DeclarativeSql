using System;
using System.Data;
using System.Linq;
using System.Reflection;
using DeclarativeSql.Annotations;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// Provides column mapping information.
    /// </summary>
    internal sealed class ColumnInfo
    {
        #region Properties
        /// <summary>
        /// Gets the name of member.
        /// </summary>
        public string MemberName { get; }


        /// <summary>
        /// Gets the type of member.
        /// </summary>
        public Type MemberType { get; }


        /// <summary>
        /// Gets the name of column.
        /// </summary>
        public string ColumnName { get; }


        /// <summary>
        /// Gets the type of column.
        /// </summary>
        public DbType? ColumnType { get; }


        /// <summary>
        /// Gets the default value.
        /// </summary>
        public object DefaultValue { get; }


        /// <summary>
        /// Gets whether primary key.
        /// </summary>
        public bool IsPrimaryKey { get; }


        /// <summary>
        /// Gets whether auto increment numbering.
        /// </summary>
        public bool IsAutoIncrement { get; }


        /// <summary>
        /// Gets whether allowing null.
        /// </summary>
        public bool AllowNull { get; }


        /// <summary>
        /// Gets whether unique constraint.
        /// </summary>
        public bool IsUnique
            => this.UniqueIndex.HasValue;


        /// <summary>
        /// Gets the index of unique constraint.
        /// </summary>
        public int? UniqueIndex { get; }


        /// <summary>
        /// Gets whether CreatedAt column.
        /// </summary>
        public bool IsCreatedAt { get; }


        /// <summary>
        /// Gets whether ModifiedAt column.
        /// </summary>
        public bool IsModifiedAt { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="property"></param>
        internal ColumnInfo(DbKind database, PropertyInfo property)
            : this(database, property, property.PropertyType)
        {}


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="field"></param>
        internal ColumnInfo(DbKind database, FieldInfo field)
            : this(database, field, field.FieldType)
        {}


        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="member"></param>
        private ColumnInfo(DbKind database, MemberInfo member, Type memberType)
        {
            var column = member.GetCustomAttributes<ColumnAttribute>(true).FirstOrDefault(x => x.Database == database);
            var @default = member.GetCustomAttributes<DefaultValueAttribute>(true).FirstOrDefault(x => x.Database == database);
            var unique = member.GetCustomAttribute<UniqueAttribute>(true);

            this.MemberName = member.Name;
            this.MemberType = memberType;
            this.ColumnName = column?.Name ?? member.Name;
            this.ColumnType = column?.Type;
            this.DefaultValue = @default?.Value;
            this.IsPrimaryKey = Attribute.IsDefined(member, typeof(PrimaryKeyAttribute));
            this.IsAutoIncrement = Attribute.IsDefined(member, typeof(AutoIncrementAttribute));
            this.AllowNull = Attribute.IsDefined(member, typeof(AllowNullAttribute));
            this.UniqueIndex = this.IsPrimaryKey ? -1 : (int?)unique?.Index;
            this.IsCreatedAt = Attribute.IsDefined(member, typeof(CreatedAtAttribute));
            this.IsModifiedAt = Attribute.IsDefined(member, typeof(ModifiedAtAttribute));
        }
        #endregion
    }
}
