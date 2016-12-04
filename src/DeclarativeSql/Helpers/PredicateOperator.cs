using System;
using System.Linq.Expressions;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Represents predicative expression operators.
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
    /// Provides PredicateOperator extensions.
    /// </summary>
    internal static class PredicateOperatorExtensions
    {
        /// <summary>
        /// Provides conversion from ExpressionType to PredicateOperator.
        /// </summary>
        /// <param name="expressionType">Node type of expression tree</param>
        /// <returns>Predicative expression operators</returns>
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
        /// Inverts the specified predicative expression operator.
        /// </summary>
        /// <param name="@operator">Operator</param>
        /// <returns>Inverted operator</returns>
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