using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            return this.VisitCore(() =>
            {
                //--- AND/OR : 左右を保持する要素として生成
                //--- 比較演算子 (<, <=, >=, >, ==, !=) : 左辺のプロパティ名と右辺の値を抽出
                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        return new PredicateElement(node.NodeType.ToPredicateOperator());

                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                        return this.ParseBinary(node);

                    default:
                        throw new InvalidOperationException();
                }
            },
            () => base.VisitBinary(node));
        }


        /// <summary>
        /// MethodCallExpressionの子を走査します。
        /// </summary>
        /// <param name="node">走査する式</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //--- Enumerable.Contains
            if (node.Method.DeclaringType == typeof(Enumerable))
            if (node.Method.Name == nameof(Enumerable.Contains))
            {
                return this.VisitCore(() =>
                {
                    //--- プロパティ名を取得
                    var propertyName = this.ExtractMemberName(node.Arguments[1]);
                    if (propertyName == null)
                        throw new InvalidOperationException();

                    //--- 要素生成
                    //--- in句は1000件以上あるとエラーが発生するためorでつなぐ
                    var source  = (this.ExtractValue(node.Arguments[0]) as IEnumerable)
                                .Cast<object>()
                                .Buffer(1000)
                                .Select(x => x.ToArray());

                    PredicateElement root = null;
                    foreach (var x in source)
                    {
                        if (root != null)
                        {
                            var parent   = new PredicateElement(PredicateOperator.OrElse);
                            parent.Left  = new PredicateElement(PredicateOperator.Contains, this.parameter.Type, propertyName, x);
                            parent.Right = root;
                            root         = parent;
                            continue;
                        }
                        root = new PredicateElement(PredicateOperator.Contains, this.parameter.Type, propertyName, x);
                    }
                    return root;
                },
                () => base.VisitMethodCall(node));
            }

            //--- default
            return base.VisitMethodCall(node);
        }
        #endregion


        #region 補助
        /// <summary>
        /// 式を走査します。
        /// </summary>
        /// <param name="elementGenerator">要素生成デリゲート</param>
        /// <param name="baseCall">基底メソッド呼び出しデリゲート</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        private Expression VisitCore(Func<PredicateElement> elementGenerator, Func<Expression> baseCall)
        {
            //--- 要素生成
            var element = elementGenerator();

            //--- 親要素と関連付け
            var parent = this.cache.Count == 0 ? null : this.cache.Peek();
            if (parent != null)
            {
                if      (parent.Left == null)   parent.Left = element;
                else if (parent.Right == null)  parent.Right = element;
                else                            throw new InvalidOperationException();
            }

            //--- 子要素を解析
            this.cache.Push(element);
            var result = baseCall();
            this.cache.Pop();

            //--- キャッシュがなくなった場合 (= Root)
            if (this.cache.Count == 0)
                this.Root = element;

            //--- ok
            return result;
        }


        /// <summary>
        /// 二項演算子を持つ式を解析します。
        /// </summary>
        /// <param name="node">対象となる式</param>
        /// <returns>条件式の要素</returns>
        private PredicateElement ParseBinary(BinaryExpression node)
        {
            //--- 'x.Hoge == value'
            var propertyName = this.ExtractMemberName(node.Left);
            if (propertyName != null)
            {
                var @operator = node.NodeType.ToPredicateOperator();
                var value = this.ExtractValue(node.Right);
                return new PredicateElement(@operator, this.parameter.Type, propertyName, value);
            }

            //--- 'value == x.Hoge'
            propertyName = this.ExtractMemberName(node.Right);
            if (propertyName != null)
            {
                var @operator = node.NodeType.ToPredicateOperator().Flip();
                var value = this.ExtractValue(node.Left);
                return new PredicateElement(@operator, this.parameter.Type, propertyName, value);
            }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// 指定された式からメンバー名を抽出します。
        /// </summary>
        /// <param name="expression">抽出対象の式</param>
        /// <returns>メンバー名</returns>
        private string ExtractMemberName(Expression expression)
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
                var expr = (NewExpression)expression;
                var parameters = expr.Arguments.Select(this.ExtractValue).ToArray();
                return expr.Constructor.Invoke(parameters);
            }

            //--- 配列生成
            if (expression is NewArrayExpression)
            {
                var expr = (NewArrayExpression)expression;
                return expr.Expressions.Select(this.ExtractValue).ToArray();
            }

            //--- メソッド呼び出し
            if (expression is MethodCallExpression)
            {
                var expr = (MethodCallExpression)expression;
                var parameters = expr.Arguments.Select(this.ExtractValue).ToArray();
                var obj = expr.Object == null
                        ? null                              //--- static
                        : this.ExtractValue(expr.Object);   //--- instance
                return expr.Method.Invoke(obj, parameters);
            }

            //--- フィールド / プロパティ
            var memberNames = new List<string>();
            var temp = expression;
            while (!(temp is ConstantExpression))
            {
                //--- cast
                if (temp is UnaryExpression)
                if (temp.NodeType == ExpressionType.Convert)
                {
                    temp = ((UnaryExpression)temp).Operand;
                    continue;
                }

                //--- not member
                var member = temp as MemberExpression;
                if (member == null)
                    return this.ExtractValue(temp);

                //--- static
                if (member.Expression == null)
                {
                    if (member.Member.MemberType == MemberTypes.Property)
                    {
                        var info = (PropertyInfo)member.Member;
                        return info.GetValue(null);
                    }
                    if (member.Member.MemberType == MemberTypes.Field)
                    {
                        var info = (FieldInfo)member.Member;
                        return info.GetValue(null);
                    }
                    throw new InvalidOperationException("Not field or property.");
                }

                //--- instance
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