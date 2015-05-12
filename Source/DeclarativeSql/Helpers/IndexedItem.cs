namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 要素とインデックスのペアを表します。
    /// </summary>
    /// <typeparam name="T">格納する要素の型</typeparam>
    internal sealed class IndexedItem<T>
    {
        #region プロパティ
        /// <summary>
        /// 要素を取得します。
        /// </summary>
        public T Element { get; }


        /// <summary>
        /// インデックスを取得します。
        /// </summary>
        public int Index { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="element">要素</param>
        /// <param name="index">インデックス</param>
        internal IndexedItem(T element, int index)
        {
            this.Element = element;
            this.Index   = index;
        }
        #endregion
    }
}