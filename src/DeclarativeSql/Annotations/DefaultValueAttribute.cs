using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that represents default value of column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public sealed class DefaultValueAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets the kind of database.
        /// </summary>
        public DbKind Database { get; }


        /// <summary>
        /// Gets the default value.
        /// </summary>
        public object Value { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="value"></param>
        public DefaultValueAttribute(DbKind database, object value)
        {
            this.Database = database;
            this.Value = value;
        }
        #endregion
    }
}
