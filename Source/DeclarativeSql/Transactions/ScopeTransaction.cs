using System;
using System.Data;



namespace DeclarativeSql.Transactions
{
    /// <summary>
    /// スコープ管理可能なデータベーストランザクション機能を提供します。
    /// </summary>
    public sealed class ScopeTransaction : IScopeTransaction, IDbTransaction
    {
        #region Fields
        /// <summary>
        /// ラップしている生のトランザクションを取得します。
        /// </summary>
        internal IDbTransaction Raw { get; }


        /// <summary>
        /// 処理が正常に完了したかどうかを取得または設定します。
        /// </summary>
        private bool IsCompleted { get; set; }
        #endregion


        #region Constructors / Destructors
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="transaction">管理対象のトランザクション</param>
        internal ScopeTransaction(IDbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            this.Raw = transaction;
        }


        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        ~ScopeTransaction()
        {
            this.Dispose();
        }
        #endregion


        #region ITransactionScope members
        /// <summary>
        /// トランザクション処理が正常に完了したことをマークします。
        /// </summary>
        /// <remarks>このメソッドを呼び出した時点ではコミットは行われません。</remarks>
        public void Complete() => this.IsCompleted = true;
        #endregion


        #region IDbTransaction members
        /// <summary>
        /// トランザクションを関連付けるデータベース接続オブジェクトを取得します。
        /// </summary>
        public IDbConnection Connection => this.Raw.Connection;


        /// <summary>
        /// このトランザクションの分離レベルを取得します。
        /// </summary>
        public IsolationLevel IsolationLevel => this.Raw.IsolationLevel;


        /// <summary>
        /// データベーストランザクションをコミットします。
        /// </summary>
        void IDbTransaction.Commit() => this.Raw.Commit();


        /// <summary>
        /// 保留中の状態からトランザクションをロールバックします。
        /// </summary>
        void IDbTransaction.Rollback() => this.Raw.Rollback();
        #endregion


        #region IDisposable members
        /// <summary>
        /// 使用しているリソースを解放します。
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