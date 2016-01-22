using System;
using System.Linq.Expressions;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 条件式の演算子を表します。
    /// </summary>
    internal enum PredicateOperator
    {
        /// <summary>
        /// a && b
        /// </summary>
        AndAlso = 0,

        /// <summary>
        /// a || b
        /// </summary>
        OrElse,

        /// <summary>
        /// a < b
        /// </summary>
        LessThan,

        /// <summary>
        /// a <= b
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// a > b
        /// </summary>
        GreaterThan,

        /// <summary>
        /// a >= b
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// a == b
        /// </summary>
        Equal,

        /// <summary>
        /// a != b
        /// </summary>
        NotEqual,

        /// <summary>
        /// Enumerable.Contains(value)
        /// </summary>
        Contains,
    }



    /// <summary>
    /// PredicateOperatorに関する拡張機能を提供します。
    /// </summary>
    internal static class PredicateOperatorExtensions
    {
        /// <summary>
        /// ExpressionTypeからPredicateOperatorへの変換を提供します。
        /// </summary>
        /// <param name="expressionType">式木のノード型</param>
        /// <returns>条件式演算子</returns>
        public static PredicateOperator ToPredicateOperator(this ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.AndAlso:            return PredicateOperator.AndAlso;
                case ExpressionType.OrElse:             return PredicateOperator.OrElse;
                case ExpressionType.LessThan:           return PredicateOperator.LessThan;
                case ExpressionType.LessThanOrEqual:    return PredicateOperator.LessThanOrEqual;
                case ExpressionType.GreaterThan:        return PredicateOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual: return PredicateOperator.GreaterThanOrEqual;
                case ExpressionType.Equal:              return PredicateOperator.Equal;
                case ExpressionType.NotEqual:           return PredicateOperator.NotEqual;
            }
            throw new InvalidOperationException();
        }


        /// <summary>
        /// 指定された条件式演算子を反転します。
        /// </summary>
        /// <param name="@operator">演算子</param>
        /// <returns>反転された演算子</returns>
        public static PredicateOperator Flip(this PredicateOperator @operator)
        {
            switch (@operator)
            {
                case PredicateOperator.LessThan:            return PredicateOperator.GreaterThan;
                case PredicateOperator.LessThanOrEqual:     return PredicateOperator.GreaterThanOrEqual;
                case PredicateOperator.GreaterThan:         return PredicateOperator.LessThan;
                case PredicateOperator.GreaterThanOrEqual:  return PredicateOperator.LessThanOrEqual;
            }
            return @operator;
        }
    }
}