using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether is ModifiedAt column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ModifiedAtAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        public ModifiedAtAttribute()
        { }
        #endregion
    }
}
