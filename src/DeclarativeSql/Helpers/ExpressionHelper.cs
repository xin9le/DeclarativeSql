using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using This = DeclarativeSql.Helpers.ExpressionHelper;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides an auxiliary function of expression trees.
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Gets the member name from the expression tree representing the member.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expressions">Expressions</param>
        /// <returns>Member name</returns>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            var member = This.ExtractMemberExpression(expression);
            return member?.Member.Name;
        }


        /// <summary>
        /// Gets the member name from the expression tree representing the member.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expressions">Expressions</param>
        /// <returns>Collection of member names</returns>
        public static IEnumerable<string> GetMemberNames<T>(IEnumerable<Expression<Func<T, object>>> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions));
            return expressions.Select(This.GetMemberName);
        }


        /// <summary>
        /// Gets the member name from the expression tree.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Collection of member names</returns>
        public static IEnumerable<string> GetMemberNames<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            //--- 本体がコンストラクタ呼び出しの場合
            var body = expression.Body as NewExpression;
            if (body != null)
                return body.Members.Select(x => x.Name);

            //--- それ以外は通常処理
            return new [] { This.GetMemberName(expression) };
        }


        /// <summary>
        /// Gets a MemberExpression from a lambda expression.
        /// </summary>
        /// <param name="expression">Lambda expression</param>
        /// <returns>An expression that represents a field or property</returns>
        public static MemberExpression ExtractMemberExpression(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return This.ExtractMemberExpression(expression.Body);
        }


        /// <summary>
        /// Gets a MemberExpression from a expression.
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns>An expression that represents a field or property</returns>
        public static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            //--- メンバー
            if (expression is MemberExpression)
                return (MemberExpression)expression;

            //--- Boxingのためにobjectへの型変換が入っている場合はそれを考慮
            var unary = expression as UnaryExpression;
            if (unary != null)
            if (unary.NodeType == ExpressionType.Convert)
            if (unary.Operand is MemberExpression)
                return (MemberExpression)unary.Operand;

            return null;
        }
    }
}
