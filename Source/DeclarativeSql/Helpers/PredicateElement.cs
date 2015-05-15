using System;
using System.Collections.Generic;



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
        public PredicateOperator Operator { get; }


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
        internal PredicateElement(PredicateOperator @operator, Type type = null, string propertyName = null, object value = null)
        {
            this.Operator = @operator;
            this.Type = type;
            this.PropertyName = propertyName;
            this.Value = value;
        }
        #endregion
    }



    /// <summary>
    /// PredicateElementの拡張機能を提供します。
    /// </summary>
    public static class PredicateElementExtensions
    {
        /// <summary>
        /// 指定された条件式要素の子要素を取得します。
        /// </summary>
        /// <param name="element">条件式要素</param>
        /// <returns>条件式要素のコレクション</returns>
        public static IEnumerable<PredicateElement> Children(this PredicateElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (element.HasChildren)
            {
                yield return element.Left;
                yield return element.Right;
            }
        }


        /// <summary>
        /// 指定された条件式要素の子孫要素を取得します。
        /// </summary>
        /// <param name="element">条件式要素</param>
        /// <returns>条件式要素のコレクション</returns>
        public static IEnumerable<PredicateElement> Descendants(this PredicateElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            foreach (var child in element.Children())
            {
                yield return child;
                foreach (var grandChild in child.Descendants())
                    yield return grandChild;
            }
        }


        /// <summary>
        /// 指定された条件式要素自身とその子要素を取得します。
        /// </summary>
        /// <param name="element">条件式要素</param>
        /// <returns>条件式要素のコレクション</returns>
        public static IEnumerable<PredicateElement> ChildrenAndSelf(this PredicateElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            yield return element;
            foreach (var x in element.Children())
                yield return x;
        }


        /// <summary>
        /// 指定された条件式要素自身とその子孫要素を取得します。
        /// </summary>
        /// <param name="element">条件式要素</param>
        /// <returns>条件式要素のコレクション</returns>
        public static IEnumerable<PredicateElement> DescendantsAndSelf(this PredicateElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            yield return element;
            foreach (var x in element.Descendants())
                yield return x;
        }
    }
}