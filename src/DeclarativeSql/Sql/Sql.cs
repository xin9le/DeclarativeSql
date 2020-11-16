using Cysharp.Text;
using DeclarativeSql.Mapping;



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
        /// <param name="table"></param>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        void Build(DbProvider dbProvider, TableInfo table, ref Utf16ValueStringBuilder builder, ref BindParameter? bindParameter);
    }
}
