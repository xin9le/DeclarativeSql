using System;
using System.Diagnostics;
using System.Text;



namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents SQL for specified mapping table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISql<T>
    {
        /// <summary>
        /// Gets database provider.
        /// </summary>
        DbProvider DbProvider { get; }


        /// <summary>
        /// Builds query.
        /// </summary>
        /// <returns></returns>
        Query Build();
    }



    /// <summary>
    /// Represents SQL for specified mapping table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class Sql<T> : ISql<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider"></param>
        protected Sql(DbProvider provider)
            => this.DbProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        #endregion


        #region ISql implementations
        /// <summary>
        /// Gets database provider.
        /// </summary>
        public DbProvider DbProvider { get; }


        /// <summary>
        /// Builds query.
        /// </summary>
        /// <returns></returns>
        public Query Build()
        {
            var builder = new StringBuilder();
            var bindParameter = new BindParameter();
            this.Build(builder, bindParameter);
            return new Query(builder.ToString(), bindParameter);
        }
        #endregion


        #region abstract
        /// <summary>
        /// Builds query.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bindParameter"></param>
        internal abstract void Build(StringBuilder builder, BindParameter bindParameter);
        #endregion


        #region Debug
        private string DebuggerDisplay()
            => this.Build().Statement;
        #endregion
    }
}
