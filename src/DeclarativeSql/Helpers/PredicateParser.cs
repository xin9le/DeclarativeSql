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
    /// Provides the function to scan / analyze the predicative expression.
    /// </summary>
    internal sealed class PredicateParser : ExpressionVisitor
    {
        #region Properties
        /// <summary>
        /// 計算中の要素キャッシュを取得します。
        /// </summary>
        private Stack<PredicateElement> Cache { get; } = new Stack<PredicateElement>();


        /// <summary>
        /// 式の左辺パラメーターを取得します。
        /// </summary>
        private ParameterExpression Parameter { get; }


        /// <summary>
        /// 最上位要素を取得または設定します。
        /// </summary>
        private PredicateElement Root { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="parameter">左辺パラメーター</param>
        private PredicateParser(ParameterExpression parameter)
        {
            this.Parameter = parameter;
        }
        #endregion


        #region Overrides
        /// <summary>
        /// BinaryExpressionの子を走査します。
        /// </summary>
        /// <param name="node">走査する式</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            //--- AND/OR : 左右を保持する要素として生成
            //--- 比較演算子 (<, <=, >=, >, ==, !=) : 左辺のプロパティ名と右辺の値を抽出
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    {
                        var element = new PredicateElement(node.NodeType.ToPredicateOperator());
                        return this.VisitCore(element, base.VisitBinary, node);
                    }

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    {
                        var element = this.ParseBinary(node);
                        return this.VisitCore(element, base.VisitBinary, node);
                    }
            }
            return base.VisitBinary(node);
        }


        /// <summary>
        /// MemberExpressionの子を走査します。
        /// </summary>
        /// <param name="node">走査する式</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            //--- 「x => x.CanPlay == true」ではなく「x => x.CanPlay」のような書き方への対応
            if (this.IsBooleanProperty(node as MemberExpression))
            {
                //--- 親要素がない
                var info = (PropertyInfo)node.Member;
                var parent = this.Cache.Count == 0 ? null : this.Cache.Peek();
                if (parent == null)
                {
                    var element = new PredicateElement(PredicateOperator.Equal, info.PropertyType, info.Name, true);
                    return this.VisitCore(element, base.VisitMember, node);
                }

                switch (parent.Operator)
                {
                    //--- && か || の場合は左辺/右辺のどちらか
                    case PredicateOperator.AndAlso:
                    case PredicateOperator.OrElse:
                        {
                            var element = new PredicateElement(PredicateOperator.Equal, info.PropertyType, info.Name, true);
                            return this.VisitCore(element, base.VisitMember, node);
                        }

                    //--- == / != / !x.CanPlay の場合
                    case PredicateOperator.Equal:
                    case PredicateOperator.NotEqual:
                        if (parent.PropertyName == null)
                        {
                            parent.Type = info.PropertyType;
                            parent.PropertyName = info.Name;
                            parent.Value = true;
                        }
                        break;
                }
            }
            return base.VisitMember(node);
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
                        parent.Left  = new PredicateElement(PredicateOperator.Contains, this.Parameter.Type, propertyName, x);
                        parent.Right = root;
                        root         = parent;
                        continue;
                    }
                    root = new PredicateElement(PredicateOperator.Contains, this.Parameter.Type, propertyName, x);
                }
                return this.VisitCore(root, base.VisitMethodCall, node);
            }

            //--- default
            return base.VisitMethodCall(node);
        }


        /// <summary>
        /// UnaryExpressionの子を走査します。
        /// </summary>
        /// <param name="node">走査する式。</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            //--- !x.CanPlay の形式は xCanPlay != true として扱う
            if (node.NodeType == ExpressionType.Not)
            if (this.IsBooleanProperty(node.Operand as MemberExpression))
            {
                var element = new PredicateElement(PredicateOperator.NotEqual);
                return this.VisitCore(element, base.VisitUnary, node);
            }   
           return base.VisitUnary(node);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// 式を走査します。
        /// </summary>
        /// <param name="element">要素生成</param>
        /// <param name="baseCall">基底メソッド呼び出しデリゲート</param>
        /// <returns>式またはいずれかの部分式が変更された場合は変更された式。それ以外の場合は元の式。</returns>
        private Expression VisitCore<TState>(PredicateElement element, Func<TState, Expression> baseCall, TState state)
        {
            //--- 親要素と関連付け
            var parent = this.Cache.Count == 0 ? null : this.Cache.Peek();
            if (parent != null)
            {
                if      (parent.Left == null)   parent.Left = element;
                else if (parent.Right == null)  parent.Right = element;
                else                            throw new InvalidOperationException();
            }

            //--- 子要素を解析
            this.Cache.Push(element);
            var result = baseCall(state);
            this.Cache.Pop();

            //--- キャッシュがなくなった場合 (= Root)
            if (this.Cache.Count == 0)
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
                return new PredicateElement(@operator, this.Parameter.Type, propertyName, value);
            }

            //--- 'value == x.Hoge'
            propertyName = this.ExtractMemberName(node.Right);
            if (propertyName != null)
            {
                var @operator = node.NodeType.ToPredicateOperator().Flip();
                var value = this.ExtractValue(node.Left);
                return new PredicateElement(@operator, this.Parameter.Type, propertyName, value);
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
            if (member.Expression == this.Parameter)
                return member.Member.Name;
            return null;
        }


        /// <summary>
        /// 指定された式から値を抽出します。
        /// </summary>
        /// <param name="expression">対象となる式</param>
        /// <returns>値</returns>
        /// <remarks>右辺専用</remarks>
        private object ExtractValue(Expression expression)
        {
            // ToDo: performance improvement

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

            //--- デリゲート/ラムダ式の呼び出し
            if (expression is InvocationExpression)
            {
                var invocation = (InvocationExpression)expression;
                var parameters = invocation.Arguments.Select(x => Expression.Parameter(x.Type)).ToArray();
                var arguments = invocation.Arguments.Select(this.ExtractValue).ToArray();
                var lambda = Expression.Lambda(invocation, parameters);
                var result = lambda.Compile().DynamicInvoke(arguments);
                return result;
            }

            //--- インデクサ
            if (expression is BinaryExpression)
            if (expression.NodeType == ExpressionType.ArrayIndex)
            {
                var expr = (BinaryExpression)expression;
                var array = (Array)this.ExtractValue(expr.Left);
                var index = (int)this.ExtractValue(expr.Right);
                return array.GetValue(index);
                /*
                var index = expr.Right.Type == typeof(int)
                          ? (int) this.ExtractValue(expr.Right)
                          : (long)this.ExtractValue(expr.Right);
                return array.GetValue((int)index);
                */
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
                    if (member.Member is PropertyInfo pi) return pi.GetValue(null);
                    if (member.Member is FieldInfo fi)    return fi.GetValue(null);
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


        /// <summary>
        /// 指定された式がbool型のプロパティかどうかを判定します。
        /// </summary>
        /// <param name="expression">対象となる式</param>
        /// <returns>bool型のプロパティの場合true</returns>
        /// <remarks>左辺専用</remarks>
        private bool IsBooleanProperty(MemberExpression expression)
            =>  expression != null
            &&  expression.Member is PropertyInfo
            &&  expression.Expression == this.Parameter
            &&  ((PropertyInfo)expression.Member).PropertyType == typeof(bool);
        #endregion


        #region Parse
        /// <summary>
        /// Parses the specified predicative expression.
        /// </summary>
        /// <typeparam name="T">Table type</typeparam>
        /// <param name="predicate">Predicative expression</param>
        /// <returns>Root node for predicative expression</returns>
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