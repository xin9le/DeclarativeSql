using System;



namespace DeclarativeSql.Transactions
{
    /// <summary>
    /// Provides scope manageable transactions interface.
    /// </summary>
    public interface IScopeTransaction : IDisposable
    {
        #region Methods
        /// <summary>
        /// Mark whether the process has completed successfully.
        /// </summary>
        void Complete();
        #endregion
    }
}
