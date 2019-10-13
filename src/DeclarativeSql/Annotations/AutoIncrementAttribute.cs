using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Provides an attribute that indicates whether to perform automatic numbering.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class AutoIncrementAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        public AutoIncrementAttribute()
        {}
        #endregion
    }
}
