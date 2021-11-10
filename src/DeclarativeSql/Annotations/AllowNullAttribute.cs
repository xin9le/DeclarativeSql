using System;

namespace DeclarativeSql.Annotations;



/// <summary>
/// Provides an attribute that indicates whether to allow null.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class AllowNullAttribute : Attribute
{
    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    public AllowNullAttribute()
    { }
    #endregion
}
