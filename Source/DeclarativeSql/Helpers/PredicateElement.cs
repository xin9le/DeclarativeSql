using System;
using System.Linq.Expressions;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 条件式を分解した要素を表します。
    /// </summary>
    public sealed class PredicateElement
    {
        #region プロパティ
        /// <summary>
        /// 演算子を取得します。
        /// </summary>
        public ExpressionType Operator { get; }


        /// <summary>
        /// 左辺のインスタンスの型を取得します。
        /// </summary>
        public Type Type { get; }


        /// <summary>
        /// 左辺のプロパティ名を取得します。
        /// </summary>
        public string PropertyName { get; }


        /// <summary>
        /// 右辺の値を取得します。
        /// </summary>
        public object Value { get; }


        /// <summary>
        /// 左辺の条件式要素を取得します。
        /// </summary>
        /// <remarks>左辺がさらに式になっている場合に利用します。</remarks>
        public PredicateElement Left { get; internal set; }


        /// <summary>
        /// 右辺の条件式要素を取得します。
        /// </summary>
        /// <remarks>右辺がさらに式になっている場合に利用します。</remarks>
        public PredicateElement Right { get; internal set; }


        /// <summary>
        /// 子要素が存在するかどうかを取得します。
        /// </summary>
        public bool HasChildren => this.Left != null || this.Right != null;
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="@operator">演算子</param>
        /// <param name="type">左辺のインスタンスの型</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="value">値</param>
        internal PredicateElement(ExpressionType @operator, Type type = null, string propertyName = null, object value = null)
        {
            this.Operator = @operator;
            this.Type = type;
            this.PropertyName = propertyName;
            this.Value = value;
        }
        #endregion
    }
}