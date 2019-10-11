namespace DeclarativeSql
{
    /// <summary>
    /// Represents value priority.
    /// </summary>
    public enum ValuePriority
    {
        /// <summary>
        /// Use attribute value if specified, then fallback to property value.
        /// </summary>
        Attribute = 0,


        /// <summary>
        /// Use property value.
        /// </summary>
        Property,
    }
}
