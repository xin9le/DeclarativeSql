using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// SQLのバインド変数の接頭辞を表す属性を表します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class BindParameterPrefixAttribute : Attribute
    {
        #region プロパティ
        /// <summary>
        /// 接頭辞を取得します。
        /// </summary>
        public char Prefix{ get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="prefix">接頭辞</param>
        public BindParameterPrefixAttribute(char prefix)
        {
            this.Prefix = prefix;
        }
        #endregion
    }
}