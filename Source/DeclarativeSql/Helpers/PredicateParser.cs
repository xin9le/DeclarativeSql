using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using This = DeclarativeSql.Helpers.PredicateParser;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 条件式を走査/解析する機能を提供します。
    /// </summary>
    public sealed class PredicateParser : ExpressionVisitor
    {
        #region フィールド
        /// <summary>
        /// 計算中の要素キャッシュを保持します。
        /// </summary>
        private readonly Stack<PredicateElement> cache;


        /// <summary>
        /// 式の左辺パラメーターを保持します。
        /// </summary>
        private readonly ParameterExpression parameter;
        #endregion


        #region プロパティ
        /// <summary>
        /// 最上位要素を取得または設定します。
        /// </summary>
        private PredicateElement Root { get; set; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="parameter">左辺パラメーター</param>
        private PredicateParser(ParameterExpression parameter)
        {
            this.cache = new Stack<PredicateElement>();
            this.parameter = parameter;
        }
        #endregion


        #region オーバーライド
        /// <summary>
        /// BinaryExpressionの子を走査します。
        /// </summary>
        /// <param name="node">走査する式</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            //--- AND/OR : 左右を保持する要素として生成
            //--- 比較演算子 (<, <=, >=, >, ==, !=) : 左辺のプロパティ名と右辺の値を抽出
            PredicateElement item;
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    item = new PredicateElement(node.NodeType);
                    break;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    item = this.ParseBinary(node);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            //--- 親要素と関連付け
            var parent = this.cache.Count == 0 ? null : this.cache.Peek();
            if (parent != null)
            {
                if      (parent.Left == null)   parent.Left = item;
                else if (parent.Right == null)  parent.Right = item;
                else                            throw new InvalidOperationException();
            }

            //--- 子要素を解析
            this.cache.Push(item);
            var result = base.VisitBinary(node);
            this.cache.Pop();

            //--- キャッシュがなくなった場合 (= Root)
            if (this.cache.Count == 0)
                this.Root = item;

            //--- ok
            return result;
        }
        #endregion


        #region 補助
        /// <summary>
        /// 二項演算子を持つ式を解析します。
        /// </summary>
        /// <param name="node">対象となる式</param>
        /// <returns>条件式の要素</returns>
        private PredicateElement ParseBinary(BinaryExpression node)
        {
            //--- 'x.Hoge == value'
            var propertyName = this.ExtractPropertyName(node.Left);
            if (propertyName != null)
            {
                var @operator = node.NodeType;
                var value = this.ExtractValue(node.Right);
                return new PredicateElement(@operator, this.parameter.Type, propertyName, value);
            }

            //--- 'value == x.Hoge'
            propertyName = this.ExtractPropertyName(node.Right);
            if (propertyName != null)
            {
                var @operator = This.FilpOperator(node.NodeType);
                var value = this.ExtractValue(node.Left);
                return new PredicateElement(@operator, this.parameter.Type, propertyName, value);
            }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// 指定された式からプロパティ名を抽出します。
        /// </summary>
        /// <param name="expression">抽出対象の式</param>
        /// <returns>プロパティ名</returns>
        private string ExtractPropertyName(Expression expression)
        {
            var member = ExpressionHelper.ExtractMemberExpression(expression);
            if (member != null)
            if (member.Expression == this.parameter)
                return member.Member.Name;
            return null;
        }


        /// <summary>
        /// 指定された式から値を抽出します。
        /// </summary>
        /// <param name="expression">対象となる式</param>
        /// <returns>値</returns>
        private object ExtractValue(Expression expression)
        {
            //--- 定数
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;

            //--- インスタンス生成
            if (expression is NewExpression)
            {
                var @new = (NewExpression)expression;
                var parameters = @new.Arguments.Select(this.ExtractValue).ToArray();
                return @new.Constructor.Invoke(parameters);
            }

            //--- 変数
            var memberNames = new List<string>();
            var temp = expression;
            while (!(temp is ConstantExpression))
            {
                if (temp is UnaryExpression)
                if (temp.NodeType == ExpressionType.Convert)
                {
                    temp = ((UnaryExpression)temp).Operand;
                    continue;
                }

                var member = (MemberExpression)temp;
                memberNames.Add(member.Member.Name);
                temp = member.Expression;
            }

            var value = ((ConstantExpression)temp).Value;
            for (int i = memberNames.Count - 1; 0 <= i; i--)
            {
                var getter = AccessorCache.LookupGet(value.GetType(), memberNames[i]);
                value = getter(value);
            }
            return value;
        }


        /// <summary>
        /// 指定された比較演算子の反転します。
        /// </summary>
        /// <param name="@operator">演算子</param>
        /// <returns>反転された演算子</returns>
        private static ExpressionType FilpOperator(ExpressionType @operator)
        {
            switch (@operator)
            {
                case ExpressionType.LessThan:           return ExpressionType.GreaterThan;
                case ExpressionType.LessThanOrEqual:    return ExpressionType.GreaterThanOrEqual;
                case ExpressionType.GreaterThan:        return ExpressionType.LessThan;
                case ExpressionType.GreaterThanOrEqual: return ExpressionType.LessThanOrEqual;
            }
            return @operator;
        }
        #endregion


        #region 解析
        /// <summary>
        /// 指定された条件式を解析します。
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="predicate">条件式</param>
        /// <returns>条件式要素の最上位ノード</returns>
        public static PredicateElement Parse<T>(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var visitor = new This(predicate.Parameters[0]);
            visitor.Visit(predicate);
            return visitor.Root;
        }
        #endregion
    }
}