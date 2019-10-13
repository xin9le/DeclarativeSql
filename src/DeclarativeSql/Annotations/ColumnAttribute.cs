using System;
using System.Data;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that represents a column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets the kind of database.
        /// </summary>
        public DbKind Database { get; }


        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Gets or sets the data type of the column.
        /// This setting is required when performing BulkInsert under Oracle environment.
        /// </summary>
        public DbType? Type { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        public ColumnAttribute(DbKind database, string name)
        {
            this.Database = database;
            this.Name = name;
        }
        #endregion
    }
}
