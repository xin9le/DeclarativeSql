using Cysharp.Text;



namespace DeclarativeSql.Sql.Statements
{
    /// <summary>
    /// Represents null statement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Null<T> : Statement<T>
    {
        #region Constructors
        /// <inheritdoc/>
        public Null()
        { }
        #endregion


        #region override
        /// <inheritdoc/>
        internal override void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        { }
        #endregion
    }
}
