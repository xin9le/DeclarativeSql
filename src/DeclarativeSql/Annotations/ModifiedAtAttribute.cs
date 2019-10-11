using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether is ModifiedAt column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ModifiedAtAttribute : Attribute
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
        public ModifiedAtAttribute(string defaultValue = null)
            => this.DefaultValue = defaultValue;
        #endregion
    }
}
