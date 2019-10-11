using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether is CreatedAt column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CreatedAtAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets the default value.
        /// </summary>
        public string DefaultValue { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="defaultValue"></param>
        public CreatedAtAttribute(string defaultValue = null)
            => this.DefaultValue = defaultValue;
        #endregion
    }
}
