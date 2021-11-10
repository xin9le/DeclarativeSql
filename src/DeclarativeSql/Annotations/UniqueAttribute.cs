using System;

namespace DeclarativeSql.Annotations;



/// <summary>
/// Provides an attribute that represents a unique constraint.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class UniqueAttribute : Attribute
{
    #region Properties
    /// <summary>
    /// Gets the index.
    /// </summary>
    public ushort Index { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    public UniqueAttribute(ushort index)
        => this.Index = index;
    #endregion
}
