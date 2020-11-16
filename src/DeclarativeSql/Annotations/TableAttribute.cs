using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that represents a table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public sealed class TableAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets the kind of database.
        /// </summary>
        public DbKind Database { get; }


        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        public string? Schema { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        public TableAttribute(DbKind database, string name)
        {
            this.Database = database;
            this.Name = name;
        }
        #endregion
    }
}
