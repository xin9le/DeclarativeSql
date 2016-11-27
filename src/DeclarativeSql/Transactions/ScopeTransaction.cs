using System;
using System.Data;



namespace DeclarativeSql.Transactions
{
    /// <summary>
    /// Provides scope manageable transactions.
    /// </summary>
    public sealed class ScopeTransaction : IScopeTransaction, IDbTransaction
    {
        #region Fields
        /// <summary>
        /// Gets wrapped raw transaction.
        /// </summary>
        internal IDbTransaction Raw { get; }


        /// <summary>
        /// Gets or sets whether processing has completed successfully.
        /// </summary>
        private bool IsCompleted { get; set; }
        #endregion


        #region Constructors / Destructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="transaction">Target transaction</param>
        internal ScopeTransaction(IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            this.Raw = transaction;
        }


        /// <summary>
        /// Destroys instance.
        /// </summary>
        ~ScopeTransaction()
        {
            this.Dispose();
        }
        #endregion


        #region ITransactionScope members
        /// <summary>
        /// Mark that transaction processing completed successfully.
        /// </summary>
        /// <remarks>When this method is called, no commit is done.</remarks>
        public void Complete() => this.IsCompleted = true;
        #endregion


        #region IDbTransaction members
        /// <summary>
        /// Gets the database connection object that associates the transaction.
        /// </summary>
        public IDbConnection Connection => this.Raw.Connection;


        /// <summary>
        /// Gets the isolation level of this transaction.
        /// </summary>
        public IsolationLevel IsolationLevel => this.Raw.IsolationLevel;


        /// <summary>
        /// Commit the database transaction.
        /// </summary>
        void IDbTransaction.Commit() => this.Raw.Commit();


        /// <summary>
        /// Roll back the transaction from the pending state.
        /// </summary>
        void IDbTransaction.Rollback() => this.Raw.Rollback();
        #endregion


        #region IDisposable members
        /// <summary>
        /// Releases the used resources.
        /// </summary>
        public void Dispose()
        {
            if (this.IsCompleted) this.Raw.Commit();
            else                  this.Raw.Rollback();
            this.Raw.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}