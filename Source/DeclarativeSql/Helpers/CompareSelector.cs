using System;
using System.Collections.Generic;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// デリゲートで外部から等値比較方法を指定する、比較セレクターとしての機能を提供ます。
    /// </summary>
    /// <typeparam name="T">オブジェクトの型</typeparam>
    /// <typeparam name="TKey">比較する値の型</typeparam>
    internal sealed class CompareSelector<T, TKey> : IEqualityComparer<T>
    {
        #region フィールド
        /// <summary>
        /// 比較するためのセレクターを保持します。
        /// </summary>
        private Func<T, TKey> selector = null;
        #endregion


        #region コンストラクタ
        /// <summary>
        /// 指定のセレクターからインスタンスを生成します。
        /// </summary>
        /// <param name="selector">比較用セレクター</param>
        public CompareSelector(Func<T, TKey> selector)
        {
            this.selector = selector;
        }
        #endregion


        #region IEqualityComparer<T>の実装
        /// <summary>
        /// 指定したオブジェクトが等しいかどうかを判断します。
        /// </summary>
        /// <param name="x">比較対象のT型の第1オブジェクト</param>
        /// <param name="y">比較対象のT型の第2オブジェクト</param>
        /// <returns>等しいかどうか</returns>
        public bool Equals(T x, T y)
        {
            if (this.selector(x) == null)
            if (this.selector(y) == null)   return true;
            if (this.selector(x) == null)   return false;
            if (this.selector(y) == null)   return false;
            return this.selector(x).Equals(this.selector(y));
        }


        /// <summary>
        /// 指定したオブジェクトのハッシュコードを返します。
        /// </summary>
        /// <param name="obj">ハッシュコードが返される対象のオブジェクト</param>
        /// <returns>ハッシュコード</returns>
        public int GetHashCode(T obj)
            =>  this.selector(obj) == null
                ? default(int)
                : this.selector(obj).GetHashCode();
        #endregion
    }
}