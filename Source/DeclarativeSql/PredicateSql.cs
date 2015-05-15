using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;
using This = DeclarativeSql.PredicateSql;



namespace DeclarativeSql
{
    /// <summary>
    /// 条件式のSQLを表します。
    /// </summary>
    public sealed class PredicateSql
    {
        #region プロパティ
        /// <summary>
        /// 命令文を取得します。
        /// </summary>
        public string Statement { get; }


        /// <summary>
        /// パラメーターを取得します。
        /// </summary>
        public ExpandoObject Parameter { get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="statement">命令文</param>
        /// <param name="parameter">パラメーター</param>
        private PredicateSql(string statement, ExpandoObject parameter)
        {
            this.Statement = statement;
            this.Parameter = parameter;
        }
        #endregion


        #region 生成
        /// <summary>
        /// 条件を表す式からSQLを生成します
        /// </summary>
        /// <typeparam name="T">テーブルの型</typeparam>
        /// <param name="targetDatabase">対象となるデータベース</param>
        /// <param name="predicate">条件式</param>
        /// <returns>条件SQL</returns>
        public static This From<T>(DbKind targetDatabase, Expression<Func<T, bool>> predicate)
        {
            //--- SQL文 / パラメーター生成の定義
            uint index = 0;
            var columnMap = TableMappingInfo.Create<T>().Columns.ToDictionary(x => x.PropertyName);
            IDictionary<string, object> parameter = new ExpandoObject();
            Func<PredicateElement, string> sqlBuilder = null;
            sqlBuilder = element =>
            {
                if (element.HasChildren)
                {
                    var left  = sqlBuilder(element.Left);
                    var right = sqlBuilder(element.Right);
                    if (element.Operator != element.Left .Operator && element.Left .HasChildren)  left  = $"({left})";
                    if (element.Operator != element.Right.Operator && element.Right.HasChildren)  right = $"({right})";
                    if (element.Operator == PredicateOperator.AndAlso)  return $"{left} and {right}";
                    if (element.Operator == PredicateOperator.OrElse)   return $"{left} or {right}";
                    throw new InvalidOperationException();
                }
                else
                {
                    var builder = new StringBuilder();
                    builder.Append(columnMap[element.PropertyName].ColumnName);
                    switch (element.Operator)
                    {
                        case PredicateOperator.Equal:
                            if (element.Value == null)
                            {
                                builder.Append(" is null");
                                return builder.ToString();
                            }
                            builder.Append(" = ");
                            break;

                        case PredicateOperator.NotEqual:
                            if (element.Value == null)
                            {
                                builder.Append(" is not null");
                                return builder.ToString();
                            }
                            builder.Append(" <> ");
                            break;

                        case PredicateOperator.LessThan:            builder.Append(" < ");  break;
                        case PredicateOperator.LessThanOrEqual:     builder.Append(" <= "); break;
                        case PredicateOperator.GreaterThan:         builder.Append(" > ");  break;
                        case PredicateOperator.GreaterThanOrEqual:  builder.Append(" >= "); break;
                        case PredicateOperator.Contains:            builder.Append(" in "); break;
                        default:                                    throw new InvalidOperationException();
                    }

                    var parameterName = $"p{index++}";
                    parameter.Add(parameterName, element.Value);  //--- cache parameter

                    var prefix = targetDatabase.GetBindParameterPrefix();
                    builder.Append($"{prefix}{parameterName}");                    
                    return builder.ToString();
                }
            };

            //--- 解析実行
            var root = PredicateParser.Parse(predicate);
            return new This(sqlBuilder(root), parameter as ExpandoObject);
        }
        #endregion
    }
}