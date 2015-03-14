using System;
using System.Data;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// usingの範囲でトランザクションを管理するための機能を提供します。
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        #region Methods
        /// <summary>
        /// 処理が正常に完了したかどうかを示します。
        /// </summary>
        void Complete();
        #endregion
    }



    /// <summary>
    /// トランザクション処理をusingスコープで簡単に行えるようにする機能を提供します。
    /// </summary>
    internal class TransactionScope : ITransactionScope
    {
        #region Fields
        /// <summary>
        /// ラップしているトランザクションを保持します。
        /// </summary>
        private readonly IDbTransaction transaction;


        /// <summary>
        /// 処理が正常に完了したかどうかを保持します。
        /// </summary>
        private bool isCompleted;
        #endregion


        #region Constructors / Destructors
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="transaction">管理対象のトランザクション</param>
        public TransactionScope(IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            this.transaction = transaction;
        }


        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        ~TransactionScope()
        {
            this.Dispose();
        }
        #endregion


        #region ITransactionScope members
        /// <summary>
        /// トランザクション処理が正常に完了したことを記録します。
        /// </summary>
        /// <remarks>このメソッドを呼び出した時点ではコミットは行われません。</remarks>
        public void Complete()
        {
            this.isCompleted = true;
        }
        #endregion


        #region IDisposable members
        /// <summary>
        /// 使用しているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            if (this.isCompleted) this.transaction.Commit();
            else                  this.transaction.Rollback();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}