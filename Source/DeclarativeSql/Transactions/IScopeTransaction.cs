using System;



namespace DeclarativeSql.Transactions
{
    /// <summary>
    /// スコープ管理可能なトランザクションとしての機能を提供します。
    /// </summary>
    public interface IScopeTransaction : IDisposable
    {
        #region Methods
        /// <summary>
        /// 処理が正常に完了したかどうかをマークします。
        /// </summary>
        void Complete();
        #endregion
    }
}