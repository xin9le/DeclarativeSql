using System;
using System.Collections.Generic;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Represents an element obtained by decomposing a predicative expression.
    /// </summary>
    internal sealed class PredicateElement
    {
        #region Properties
        /// <summary>
        /// Gets predicative operator.
        /// </summary>
        public PredicateOperator Operator { get; }


        /// <summary>
        /// Gets the type of left side.
        /// </summary>
        public Type Type { get; internal set; }


        /// <summary>
        /// Gets the property name of left side.
        /// </summary>
        public string PropertyName { get; internal set; }


        /// <summary>
        /// Gets the value of right side.
        /// </summary>
        public object Value { get; internal set; }


        /// <summary>
        /// Gets the predicative operator of left side.
        /// </summary>
        /// <remarks>左辺がさらに式になっている場合に利用します。</remarks>
        public PredicateElement Left { get; internal set; }


        /// <summary>
        /// Gets the predicative operator of right side.
        /// </summary>
        /// <remarks>右辺がさらに式になっている場合に利用します。</remarks>
        public PredicateElement Right { get; internal set; }


        /// <summary>
        /// Gets whether child elements exists.
        /// </summary>
        public bool HasChildren => this.Left != null || this.Right != null;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="@operator">Predicative operator</param>
        /// <param name="type">Type of left side instance</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
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
    /// Provides PredicateElement extensions.
    /// </summary>
    internal static class PredicateElementExtensions
    {
        /// <summary>
        /// Gets the child elements of the specified predicative element.
        /// </summary>
        /// <param name="element">Predicative element</param>
        /// <returns>Elements</returns>
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
        /// Gets the descendant elements of the specified predicative element.
        /// </summary>
        /// <param name="element">Predicative element</param>
        /// <returns>Elements</returns>
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
        /// Gets the itself and child elements of the specified predicative element.
        /// </summary>
        /// <param name="element">Predicative element</param>
        /// <returns>Elements</returns>
        public static IEnumerable<PredicateElement> ChildrenAndSelf(this PredicateElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            yield return element;
            foreach (var x in element.Children())
                yield return x;
        }


        /// <summary>
        /// Gets the itself and descendant elements of the specified predicative element.
        /// </summary>
        /// <param name="element">Predicative element</param>
        /// <returns>Elements</returns>
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