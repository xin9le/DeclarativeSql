using System;
using DeclarativeSql.Annotations;
using This = DeclarativeSql.Mapping.SequenceMappingInfo;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// シーケンスのマッピング情報を提供します。
    /// </summary>
    public sealed class SequenceMappingInfo
    {
        #region プロパティ
        /// <summary>
        /// スキーマ名を取得します。
        /// </summary>
        public string Schema { get; }


        /// <summary>
        /// シーケンス名を取得します。
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// スキーマ名とシーケンス名を結合したフルネームを取得します。
        /// </summary>
        public string FullName => string.IsNullOrWhiteSpace(this.Schema)
                                ? this.Name
                                : "\{this.Schema}.\{this.Name}";
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="schema">スキーマ名</param>
        /// <param name="name">シーケンス名</param>
        internal SequenceMappingInfo(string schema, string name)
        {
            this.Schema = schema;
            this.Name = name;
        }
        #endregion


        #region 生成
        /// <summary>
        /// シーケンス属性からインスタンスを生成します。
        /// </summary>
        /// <param name="attribute">シーケンス属性</param>
        /// <returns>生成されたインスタンス</returns>
        internal static This From(SequenceAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            return new This(attribute.Schema, attribute.Name);
        }
        #endregion
    }
}