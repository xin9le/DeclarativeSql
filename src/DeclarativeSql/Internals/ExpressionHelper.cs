using System;
using System.Collections.Generic;
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
        public static HashSet<string> GetMemberNames<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var result = new HashSet<string>();
            if (expression.Body is UnaryExpression)  // for VB.NET
            {
                var unary = (UnaryExpression)expression.Body;
                if (unary.NodeType == ExpressionType.Convert)  // wrapped by convert expression
                {
                    if (unary.Operand is NewExpression)  // x => new { x.Id, x.Name }
                    {
                        var operand = (NewExpression)unary.Operand;
                        addMembers(result, operand);
                    }
                    else if (unary.Operand is MemberExpression)  // x => x.Id
                    {
                        var operand = (MemberExpression)unary.Operand;
                        result.Add(operand.Member.Name);
                    }
                }
            }
            else if (expression.Body is NewExpression)  // x => new { x.Id, x.Name }
            {
                var @new = (NewExpression)expression.Body;
                addMembers(result, @new);
            }
            else  // x => x.Id
            {
                var name = GetMemberName(expression);
                result.Add(name);
            }
            return result;


            #region Local Functions
            static void addMembers(HashSet<string> buffer, NewExpression expression)
            {
                var members = expression.Members;
                var count = members.Count;
                for (var i = 0; i < count; i++)
                {
                    var name = members[i].Name;
                    buffer.Add(name);
                }
            }
            #endregion
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
            if (unary is not null)
            if (unary.NodeType == ExpressionType.Convert)
            if (unary.Operand is MemberExpression)
                return (MemberExpression)unary.Operand;

            return null;
        }
    }
}
