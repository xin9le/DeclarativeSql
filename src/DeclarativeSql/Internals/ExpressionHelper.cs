using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;



namespace DeclarativeSql.Internals
{
    /// <summary>
    /// Provides helper functions for the expression.
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Gets the member name from the expression tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var member = ExtractMemberExpression(expression);
            return member?.Member.Name;
        }


        /// <summary>
        /// Gets the member name from the expression tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetMemberNames<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            //--- ctor
            var body = expression.Body as NewExpression;
            if (body != null)
                return body.Members.Select(x => x.Name);

            return new [] { GetMemberName(expression) };
        }


        /// <summary>
        /// Extracts the <see cref="MemberExpression"/> from the expression tree.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberExpression ExtractMemberExpression(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return ExtractMemberExpression(expression.Body);
        }


        /// <summary>
        /// Extracts the <see cref="MemberExpression"/> from the expression tree.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression is MemberExpression)
                return (MemberExpression)expression;

            //--- for boxing
            var unary = expression as UnaryExpression;
            if (unary != null)
            if (unary.NodeType == ExpressionType.Convert)
            if (unary.Operand is MemberExpression)
                return (MemberExpression)unary.Operand;

            return null;
        }
    }
}
