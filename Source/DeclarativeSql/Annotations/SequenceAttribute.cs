using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// シーケンス名を表す属性を表します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SequenceAttribute : Attribute
    {
        #region プロパティ
        /// <summary>
        /// シーケンス名を取得します。
        /// </summary>
        public string Name{ get; }


        /// <summary>
        /// スキーマ名を取得または設定します。
        /// </summary>
        public string Schema{ get; set; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="name">シーケンス名</param>
        public SequenceAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }
        #endregion
    }
}