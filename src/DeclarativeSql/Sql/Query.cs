﻿namespace DeclarativeSql.Sql
{
    /// <summary>
    /// Represents a query statement and parameter pair.
    /// </summary>
    public readonly struct Query
    {
        #region Properties
        /// <summary>
        /// Gets SQL statement.
        /// </summary>
        public string Statement { get; }


        /// <summary>
        /// Gets bind parameter collection.
        /// This contains parameters that are generated by where clause.
        /// </summary>
        public BindParameter? BindParameter { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="bindParameter"></param>
        internal Query(string statement, BindParameter? bindParameter)
        {
            this.Statement = statement;
            this.BindParameter = bindParameter;
        }
        #endregion
    }
}
