using System;

namespace DeclarativeSql.Annotations;



/// <summary>
/// Provides an attribute that represents a primary key.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class PrimaryKeyAttribute : Attribute
{
    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    public PrimaryKeyAttribute()
    { }
    #endregion
}
