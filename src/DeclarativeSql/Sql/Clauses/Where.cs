using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Cysharp.Text;
using DeclarativeSql.Internals;
using DeclarativeSql.Mapping;
using DeclarativeSql.Sql.Statements;
using FastMember;



namespace DeclarativeSql.Sql.Clauses
{
    /// <summary>
    /// Represents where clause.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Where<T> : Clause<T>, IWhere<T>, IWhereForSelect<T>
    {
        #region Properties
        /// <summary>
        /// Gets the expression that represents the filter condition.
        /// </summary>
        private Expression<Func<T, bool>> Predicate { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="predicate"></param>
        public Where(IStatement<T> parent, Expression<Func<T, bool>> predicate)
            : base(parent)
            => this.Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        #endregion


        #region override
        /// <inheritdoc/>
        internal override void Build(DbProvider dbProvider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
        {
            //--- Build parent
            if (this.ParentStatement != null)
            {
                this.ParentStatement.Build(dbProvider, ref builder, ref bindParameter);
                builder.AppendLine();
            }

            //--- Build body
            var tree = Parser.Parse(this.Predicate);
            tree.Build(dbProvider, ref builder, ref bindParameter);
        }
        #endregion


        #region Analyze expression tree (private class / enum only)
        /// <summary>
        /// Represents a conditional expression operator.
        /// </summary>
        private enum Operator
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
        /// Provides <see cref="Operator"/> helper methods.
        /// </summary>
        private static class OperatorExtensions
        {
            /// <summary>
            /// Converts from <see cref="ExpressionType"/> to <seealso cref="Operator"/>.
            /// </summary>
            /// <param name="expressionType"></param>
            /// <returns></returns>
            public static Operator From(ExpressionType expressionType)
                => expressionType switch
                {
                    ExpressionType.AndAlso => Operator.AndAlso,
                    ExpressionType.OrElse => Operator.OrElse,
                    ExpressionType.LessThan => Operator.LessThan,
                    ExpressionType.LessThanOrEqual => Operator.LessThanOrEqual,
                    ExpressionType.GreaterThan => Operator.GreaterThan,
                    ExpressionType.GreaterThanOrEqual => Operator.GreaterThanOrEqual,
                    ExpressionType.Equal => Operator.Equal,
                    ExpressionType.NotEqual => Operator.NotEqual,
                    _ => throw new InvalidOperationException(),
                };


            /// <summary>
            /// Inverts the specified operator.
            /// </summary>
            /// <param name="@operator"></param>
            /// <returns></returns>
            public static Operator Flip(Operator @operator)
                => @operator switch
                {
                    Operator.LessThan => Operator.GreaterThan,
                    Operator.LessThanOrEqual => Operator.GreaterThanOrEqual,
                    Operator.GreaterThan => Operator.LessThan,
                    Operator.GreaterThanOrEqual => Operator.LessThanOrEqual,
                    _ => @operator,
                };
        }


        /// <summary>
        /// Represents a tree that decomposes conditional expressions.
        /// </summary>
        private class Tree
        {
            #region Properties
            /// <summary>
            /// Gets the root node.
            /// </summary>
            private Node Root { get; }


            /// <summary>
            /// Gets the bind parameter count.
            /// </summary>
            private int BindParameterCount { get; }
            #endregion


            #region Constructors
            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="root"></param>
            /// <param name="bindParameterCount"></param>
            public Tree(Node root, int bindParameterCount)
            {
                this.Root = root;
                this.BindParameterCount = bindParameterCount;
            }
            #endregion


            #region override
            /// <summary>
            /// Builds query.
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="builder"></param>
            /// <param name="bindParameter"></param>
            public void Build(DbProvider provider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter)
            {
                var digit = NumericHelper.GetDigit(this.BindParameterCount);
                var digitFormat = ZString.Concat('D', digit);
                builder.AppendLine("where");
                builder.Append("    ");
                this.Root.Build(provider, ref builder, ref bindParameter, in digitFormat);
            }
            #endregion
        }


        /// <summary>
        /// Represents a node in the conditional expression tree.
        /// </summary>
        private abstract class Node
        {
            #region Properties
            /// <summary>
            /// Gets the operator.
            /// </summary>
            public Operator Operator { get; }
            #endregion


            #region Constructors
            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="operator"></param>
            protected Node(Operator @operator)
                => this.Operator = @operator;
            #endregion


            #region abstract
            /// <summary>
            /// Builds query.
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="builder"></param>
            /// <param name="bindParameter"></param>
            /// <param name="digitFormat"></param>
            public abstract void Build(DbProvider provider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter, in string digitFormat);
            #endregion
        }


        /// <summary>
        /// Represents and/or node.
        /// </summary>
        private class AndOr : Node
        {
            #region Properties
            /// <summary>
            /// Gets or sets left node.
            /// </summary>
            public Node Left { get; set; }


            /// <summary>
            /// Gets or sets right node.
            /// </summary>
            public Node Right { get; set; }
            #endregion


            #region Constructors
            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="operator"></param>
            public AndOr(Operator @operator)
                : base(@operator)
            {}
            #endregion


            #region override
            /// <summary>
            /// Builds query.
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="builder"></param>
            /// <param name="bindParameter"></param>
            /// <param name="digitFormat"></param>
            public override void Build(DbProvider provider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter, in string digitFormat)
            {
                if (this.Left == null || this.Right == null)
                    throw new InvalidOperationException();

                //--- left
                if ((this.Operator != this.Left.Operator) && this.Left is AndOr)  // needsBracket
                {
                    builder.Append('(');
                    this.Left.Build(provider, ref builder, ref bindParameter, in digitFormat);
                    builder.Append(')');
                }
                else
                {
                    this.Left.Build(provider, ref builder, ref bindParameter, in digitFormat);
                }

                //--- and / or
                if (this.Operator == Operator.AndAlso) builder.Append(" and ");
                if (this.Operator == Operator.OrElse)  builder.Append(" or ");

                //--- right
                if ((this.Operator != this.Right.Operator) && this.Right is AndOr)  // needsBracket
                {
                    builder.Append('(');
                    this.Right.Build(provider, ref builder, ref bindParameter, in digitFormat);
                    builder.Append(')');
                }
                else
                {
                    this.Right.Build(provider, ref builder, ref bindParameter, in digitFormat);
                }
            }
            #endregion
        }


        /// <summary>
        /// Represents a binary operation node.
        /// </summary>
        private class Binary : Node
        {
            #region Properties
            /// <summary>
            /// Gets left node property name.
            /// </summary>
            private string PropertyName { get; }


            /// <summary>
            /// Gets right node value.
            /// </summary>
            private object Value { get; }


            /// <summary>
            /// Gets bind parameter index.
            /// </summary>
            private int? BindParameterIndex { get; }
            #endregion


            #region Constructors
            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="operator"></param>
            /// <param name="propertyName">Left property name</param>
            public Binary(Operator @operator, string propertyName)
                : base(@operator)
            {
                this.PropertyName = propertyName;
                this.Value = null;
                this.BindParameterIndex = null;
            }


            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="operator"></param>
            /// <param name="propertyName">Left property name</param>
            /// <param name="value">Right property value</param>
            /// <param name="bindParameterIndex"></param>
            public Binary(Operator @operator, string propertyName, object value, int bindParameterIndex)
                : base(@operator)
            {
                this.PropertyName = propertyName;
                this.Value = value ?? throw new ArgumentNullException(nameof(value));
                this.BindParameterIndex = bindParameterIndex;
            }
            #endregion


            #region override
            /// <summary>
            /// Builds query.
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="builder"></param>
            /// <param name="bindParameter"></param>
            /// <param name="digitFormat"></param>
            public override void Build(DbProvider provider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter, in string digitFormat)
            {
                var bracket = provider.KeywordBracket;
                var columnName = TableInfo.Get<T>(provider.Database).ColumnsByMemberName[this.PropertyName].ColumnName;
                builder.Append(bracket.Begin);
                builder.Append(columnName);
                builder.Append(bracket.End);

                switch (this.Operator)
                {
                    case Operator.Equal:
                        if (this.Value == null)
                        {
                            builder.Append(" is null");
                            return;
                        }
                        builder.Append(" = ");
                        break;

                    case Operator.NotEqual:
                        if (this.Value == null)
                        {
                            builder.Append(" is not null");
                            return;
                        }
                        builder.Append(" <> ");
                        break;

                    case Operator.LessThan:           builder.Append(" < ");  break;
                    case Operator.LessThanOrEqual:    builder.Append(" <= "); break;
                    case Operator.GreaterThan:        builder.Append(" > ");  break;
                    case Operator.GreaterThanOrEqual: builder.Append(" >= "); break;
                    case Operator.Contains:           builder.Append(" in "); break;
                    default: throw new InvalidOperationException();
                }

                var name = ZString.Concat('p', this.BindParameterIndex.Value.ToString(digitFormat));
                if (bindParameter == null)
                    bindParameter = new BindParameter();
                bindParameter.Add(name, this.Value);  //--- cache parameter
                builder.Append(provider.BindParameterPrefix);
                builder.Append(name);
            }
            #endregion
        }


        /// <summary>
        /// Represents a logical (true / false) node.
        /// </summary>
        private class Boolean : Node
        {
            #region Properties
            /// <summary>
            /// Gets the value.
            /// </summary>
            private bool Value { get; }
            #endregion


            #region Constructors
            /// <summary>
            ///  Creates instance.
            /// </summary>
            public Boolean(bool value)
                : base(Operator.Equal)
                => this.Value = value;
            #endregion


            #region override
            /// <summary>
            /// Builds query.
            /// </summary>
            /// <param name="provider"></param>
            /// <param name="builder"></param>
            /// <param name="bindParameter"></param>
            /// <param name="digitFormat"></param>
            public override void Build(DbProvider provider, ref Utf16ValueStringBuilder builder, ref BindParameter bindParameter, in string digitFormat)
            {
                var sql = this.Value ? "1 = 1" : "1 = 0";
                builder.Append(sql);
            }
            #endregion
        }


        /// <summary>
        /// Provides a conditional expression analysis.
        /// </summary>
        private class Parser : ExpressionVisitor
        {
            #region Properties
            /// <summary>
            /// Gets the node cache.
            /// </summary>
            private Stack<Node> Cache { get; } = new Stack<Node>();


            /// <summary>
            /// Get the argument of an expression.
            /// </summary>
            /// <remarks>「x => x.Age == 33」の左辺の x の部分</remarks>
            private ParameterExpression Parameter { get; }


            /// <summary>
            /// Gets or sets root node.
            /// </summary>
            private Node Root { get; set; }


            /// <summary>
            /// Gets or sets bind parameter count.
            /// </summary>
            private int BindParameterCount { get; set; }
            #endregion


            #region Constructors
            /// <summary>
            /// Creates instance.
            /// </summary>
            /// <param name="parameter">式の引数</param>
            private Parser(ParameterExpression parameter)
                => this.Parameter = parameter;
            #endregion


            #region Parse
            /// <summary>
            /// Parses the specified expression tree.
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns></returns>
            public static Tree Parse(Expression<Func<T, bool>> predicate)
            {
                if (predicate == null)
                    throw new ArgumentNullException(nameof(predicate));

                var parser = new Parser(predicate.Parameters[0]);
                parser.Visit(predicate);
                return new Tree(parser.Root, parser.BindParameterCount);
            }
            #endregion


            #region override
            /// <summary>
            /// Scans the children of <see cref="BinaryExpression"/>.
            /// </summary>
            /// <param name="expression">Target expression</param>
            /// <returns></returns>
            protected override Expression VisitBinary(BinaryExpression expression)
            {
                switch (expression.NodeType)
                {
                    //--- AND/OR : Generated an element that holds the left and right
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        {
                            var @operator = OperatorExtensions.From(expression.NodeType);
                            var node = new AndOr(@operator);
                            return this.VisitCore(node, base.VisitBinary, expression);
                        }

                    //--- Comparison operator (<, <=, >=, >, ==, !=) : Extract property name of left node and value of right node
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                        {
                            //--- 'x.Hoge == value'
                            Node parse1()
                            {
                                var propertyName = this.ExtractMemberName(expression.Left);
                                if (propertyName == null)
                                    return null;

                                var @operator = OperatorExtensions.From(expression.NodeType);
                                var value = this.ExtractValue(expression.Right);
                                return value == null
                                    ? new Binary(@operator, propertyName)
                                    : new Binary(@operator, propertyName, value, ++this.BindParameterCount);
                            }
                            //--- 'value == x.Hoge'
                            Node parse2()
                            {
                                var propertyName = this.ExtractMemberName(expression.Right);
                                if (propertyName == null)
                                    return null;

                                var @operator = OperatorExtensions.From(expression.NodeType);
                                @operator = OperatorExtensions.Flip(@operator);  // 反転
                                var value = this.ExtractValue(expression.Left);
                                return value == null
                                    ? new Binary(@operator, propertyName)
                                    : new Binary(@operator, propertyName, value, ++this.BindParameterCount);
                            }

                            var node = parse1() ?? parse2() ?? throw new InvalidOperationException();
                            return this.VisitCore(node, base.VisitBinary, expression);
                        }
                }
                return base.VisitBinary(expression);
            }


            /// <summary>
            /// Scans the children of <see cref="MethodCallExpression"/>.
            /// </summary>
            /// <param name="expression">Target expression</param>
            /// <returns></returns>
            protected override Expression VisitMethodCall(MethodCallExpression expression)
            {
                //--- Enumerable.Contains
                if (expression.Method.DeclaringType == typeof(Enumerable)
                &&  expression.Method.Name == nameof(Enumerable.Contains))
                {
                    //--- Gets property name
                    var propertyName = this.ExtractMemberName(expression.Arguments[1]);
                    if (propertyName == null)
                        throw new InvalidOperationException();

                    //--- Generates element
                    //--- If there are more than 1000 in clauses, error will occur.
                    var source
                        = (this.ExtractValue(expression.Arguments[0]) as IEnumerable)
                        .Cast<object>()
                        .Buffer(SqlConstants.InClauseUpperLimitCount);

                    Node root = null;
                    foreach (var x in source)
                    {
                        var node = new Binary(Operator.Contains, propertyName, x, ++this.BindParameterCount);
                        root = (root == null)
                            ? node
                            : new AndOr(Operator.OrElse){ Left = root, Right = node } as Node;
                    }

                    //--- f there is no element in the in clause, it is forced to false.
                    if (root == null)
                        root = new Boolean(false);

                    return this.VisitCore(root, base.VisitMethodCall, expression);
                }

                //--- default
                return base.VisitMethodCall(expression);
            }
            #endregion


            #region Helpers
            /// <summary>
            /// Calls the base method and scans for the next expression.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="baseCall"></param>
            /// <param name="arg"></param>
            /// <returns></returns>
            private Expression VisitCore<TExpression>(Node node, Func<TExpression, Expression> baseCall, TExpression arg)
            {
                //--- Associates with parent node
                var parent = this.Cache.Count == 0 ? null : this.Cache.Peek();
                if (parent is AndOr x)
                {
                    if      (x.Left == null)  x.Left = node;
                    else if (x.Right == null) x.Right = node;
                    else throw new InvalidOperationException();
                }

                //--- Parse child node
                this.Cache.Push(node);
                var result = baseCall(arg);
                this.Cache.Pop();

                //--- If root
                if (this.Cache.Count == 0)
                    this.Root = node;

                //--- ok
                return result;
            }


            /// <summary>
            /// Extracts the member name from the specified expression.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            private string ExtractMemberName(Expression expression)
            {
                var member = ExpressionHelper.ExtractMemberExpression(expression);
                return member?.Expression == this.Parameter
                    ? member.Member.Name
                    : null;
            }


            /// <summary>
            /// Extracts the value from the specified expression.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            /// <remarks>Please use only for right node.</remarks>
            private object ExtractValue(Expression expression)
            {
                //--- Constant
                if (expression is ConstantExpression)
                    return ((ConstantExpression)expression).Value;

                //--- Creates instance
                if (expression is NewExpression)
                {
                    var expr = (NewExpression)expression;
                    var parameters = expr.Arguments.Select(this.ExtractValue).ToArray();
                    return expr.Constructor.Invoke(parameters);
                }

                //--- new T[]
                if (expression is NewArrayExpression)
                {
                    var expr = (NewArrayExpression)expression;
                    return expr.Expressions.Select(this.ExtractValue).ToArray();
                }

                //--- Method call
                if (expression is MethodCallExpression)
                {
                    var expr = (MethodCallExpression)expression;
                    var parameters = expr.Arguments.Select(this.ExtractValue).ToArray();
                    var obj = expr.Object == null
                            ? null                              //--- static
                            : this.ExtractValue(expr.Object);   //--- instance
                    return expr.Method.Invoke(obj, parameters);
                }

                //--- Delegate / Lambda
                if (expression is InvocationExpression invocation)
                {
                    var parameters = invocation.Arguments.Select(x => Expression.Parameter(x.Type)).ToArray();
                    var arguments = invocation.Arguments.Select(this.ExtractValue).ToArray();
                    var lambda = Expression.Lambda(invocation, parameters);
                    var result = lambda.Compile().DynamicInvoke(arguments);
                    return result;
                }

                //--- Indexer
                if (expression is BinaryExpression)
                if (expression.NodeType == ExpressionType.ArrayIndex)
                {
                    var expr = (BinaryExpression)expression;
                    var array = (Array)this.ExtractValue(expr.Left);
                    var index = (int)this.ExtractValue(expr.Right);
                    return array.GetValue(index);
                }

                //--- Field / Property
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
                    if (!(temp is MemberExpression member))
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
                    value = ObjectAccessor.Create(value)[memberNames[i]];
                return value;
            }
            #endregion
        }
        #endregion
    }
}
