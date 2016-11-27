using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Represents an attribute representing automatic number assignment.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoIncrementAttribute : Attribute
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