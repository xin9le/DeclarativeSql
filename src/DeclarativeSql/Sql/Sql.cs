using Cysharp.Text;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents SQL.
    /// </summary>
    internal interface ISql
    {
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter);
    }
}
