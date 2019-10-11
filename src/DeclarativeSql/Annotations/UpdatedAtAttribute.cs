using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether is UpdatedAt column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class UpdatedAtAttribute : Attribute
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
        public UpdatedAtAttribute(string defaultValue = null)
            => this.DefaultValue = defaultValue;
        #endregion
    }
}
