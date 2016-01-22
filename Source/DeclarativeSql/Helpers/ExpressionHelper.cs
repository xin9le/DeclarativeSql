using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using This = DeclarativeSql.Helpers.ExpressionHelper;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 式木の補助機能を提供します。
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// メンバーを表す式木からメンバー名を取得します。
        /// </summary>
        /// <typeparam name="T">メンバーを持つ型</typeparam>
        /// <param name="expressions">メンバーの式木</param>
        /// <returns>メンバー名</returns>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            var member = This.ExtractMemberExpression(expression);
            return member?.Member.Name;
        }


        /// <summary>
        /// メンバーを表す式木からメンバー名を取得します。
        /// </summary>
        /// <typeparam name="T">メンバーを持つ型</typeparam>
        /// <param name="expressions">メンバーの式木のコレクション</param>
        /// <returns>メンバー名のコレクション</returns>
        public static IEnumerable<string> GetMemberNames<T>(IEnumerable<Expression<Func<T, object>>> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException(nameof(expressions));
            return expressions.Select(This.GetMemberName);
        }


        /// <summary>
        /// 式木からメンバー名を取得します。
        /// </summary>
        /// <typeparam name="T">メンバーを持つ型</typeparam>
        /// <param name="expression">式木</param>
        /// <returns>メンバー名のコレクション</returns>
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
        /// ラムダ式からMemberExpressionを取得します。
        /// </summary>
        /// <param name="expression">ラムダ式</param>
        /// <returns>フィールドまたはプロパティを表す式</returns>
        public static MemberExpression ExtractMemberExpression(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return This.ExtractMemberExpression(expression.Body);
        }


        /// <summary>
        /// 式からMemberExpressionを取得します。
        /// </summary>
        /// <param name="expression">ラムダ式</param>
        /// <returns>フィールドまたはプロパティを表す式</returns>
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
