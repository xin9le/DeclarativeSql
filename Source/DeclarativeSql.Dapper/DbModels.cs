namespace DeclarativeSql.Dapper
{
    /// <summary>
    /// count文の結果をマッピングするためのモデルを表します。
    /// </summary>
    internal class CountResult
    {
        /// <summary>
        /// レコード数を取得または設定します。
        /// </summary>
        public ulong Count { get; set; }
    }
}