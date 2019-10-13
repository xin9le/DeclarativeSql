using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether is CreatedAt column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class CreatedAtAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        public CreatedAtAttribute()
        { }
        #endregion
    }
}
