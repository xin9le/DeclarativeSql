namespace DeclarativeSql
{
    /// <summary>
    /// Represents value priority.
    /// </summary>
    public enum ValuePriority
    {
        /// <summary>
        /// Use default value if specified, then fallback to property value.
        /// </summary>
        Default = 0,


        /// <summary>
        /// Use property value.
        /// </summary>
        Property,
    }
}
