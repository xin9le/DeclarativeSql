using System.Data;
using System.Linq;
using DeclarativeSql.Mapping;



namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// Provides the database operations for PostgreSQL.
    /// </summary>
    internal class PostgreSqlOperation : DbOperation
    {
        #region Constructors
        /// <summary>
        /// Creates instanse.
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="transaction">Database transaction</param>
        /// <param name="provider">Database provider</param>
        /// <param name="timeout">Timeout</param>
        protected PostgreSqlOperation(IDbConnection connection, IDbTransaction transaction, DbProvider provider, int? timeout)
            : base(connection, transaction, provider, timeout)
        {}
        #endregion


        #region InsertAndGet
        /// <summary>
        /// Generates SQL to insert record and get the automatically assigned ID.
        /// </summary>
        /// <typeparam name="T">Mapped type to table.</typeparam>
        /// <returns>SQL</returns>
        protected override string CreateInsertAndGetSql<T>()
        {
            var sequence = TableMappingInfo.Create<T>().Columns.First(x => x.IsPrimaryKey).Sequence;
            return
$@"{this.DbProvider.Sql.CreateInsert<T>()};
select currval({sequence.FullName(this.DbProvider.KeywordBrackets)}) as Id;";
        }
        #endregion
    }
}