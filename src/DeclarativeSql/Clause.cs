using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DeclarativeSql.Helpers;
using DeclarativeSql.Mapping;



namespace DeclarativeSql
{
    #region Clause Interfaces

    /// <summary>
    /// Represents a clause.
    /// </summary>
    public interface IClause
    {
        /// <summary>
        /// Gets database provider.
        /// </summary>
        DbProvider DbProvider { get; }


        /// <summary>
        /// Builds query.
        /// </summary>
        /// <returns>Built query</returns>
        Query Build();
    }



    /// <summary>
    /// Represents count clause.
    /// </summary>
    public interface ICountClause<T> : IClause {}



    /// <summary>
    /// Represents select clause.
    /// </summary>
    public interface ISelectClause<T> : IClause {}



    /// <summary>
    /// Represents insert clause.
    /// </summary>
    public interface IInsertClause<T> : IClause {}



    /// <summary>
    /// Represents update clause.
    /// </summary>
    public interface IUpdateClause<T> : IClause {}



    /// <summary>
    /// Represents delete clause.
    /// </summary>
    public interface IDeleteClause<T> : IClause {}



    /// <summary>
    /// Represents truncate clause.
    /// </summary>
    public interface ITruncateClause<T> : IClause {}



    /// <summary>
    /// Represents where clause.
    /// </summary>
    public interface IWhereClause<T> : IClause {}

    #endregion



    #region Internal Classes

    /// <summary>
    /// Represents a SQL clause.
    /// </summary>
    internal abstract class Clause : IClause
    {
        #region Properties
        /// <summary>
        /// Gets previous clause.
        /// </summary>
        protected Clause Previous { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        protected Clause(DbProvider provider, IClause previous)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            this.DbProvider = provider;
            this.Previous = (Clause)previous;
        }
        #endregion


        #region IClause implementations
        /// <summary>
        /// Gets database provider.
        /// </summary>
        public DbProvider DbProvider { get; }


        /// <summary>
        /// Builds query.
        /// </summary>
        /// <returns>Built query</returns>
        public Query Build()
        {
            var statement = new StringBuilder();
            var whereParameters = new ExpandoObject();
            this.Build(statement, whereParameters);
            return new Query(statement.ToString(), whereParameters);
        }
        #endregion


        #region Build
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal abstract void Build(StringBuilder statement, IDictionary<string, object> whereParameters);
        #endregion


        #region Override methods
        /// <summary>
        /// Converts this instance to string.
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
            => this.Build().Statement;
        #endregion
    }



    /// <summary>
    /// Represents a count clause.
    /// </summary>
    internal sealed class CountClause<T> : Clause, ICountClause<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        internal CountClause(DbProvider provider, IClause previous)
            : base(provider, previous)
        {}
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            var tableName = TableMappingInfo.Create(typeof(T)).FullName(this.DbProvider.KeywordBrackets);
            statement.AppendFormat("select count(*) as Count from {0}", tableName);
        }
        #endregion
    }



    /// <summary>
    /// Represents a select clause.
    /// </summary>
    internal sealed class SelectClause<T> : Clause, ISelectClause<T>
    {
        #region Properties
        /// <summary>
        /// Gets property expressions mapped columns.
        /// </summary>
        private Expression<Func<T, object>> Properties { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        internal SelectClause(DbProvider provider, IClause previous, Expression<Func<T, object>> properties)
            : base(provider, previous)
        {
            this.Properties = properties;
        }
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            //--- creates column - property name mapping
            var propertyNames = this.Properties == null
                              ? Enumerable.Empty<string>()
                              : ExpressionHelper.GetMemberNames(this.Properties);
            var table   = TableMappingInfo.Create(typeof(T));
            var columns = propertyNames.IsEmpty()
                        ? table.Columns
                        : table.Columns.Join
                        (
                            propertyNames,
                            x => x.PropertyName,
                            y => y,
                            (x, y) => x
                        );

            //--- build sql
            statement.Append("select");
            foreach (var x in columns)
            {
                statement.AppendLine();
                statement.AppendFormat("    {0} as {1},", x.ColumnName(this.DbProvider.KeywordBrackets), x.PropertyName);
            }
            statement.Length--;  //--- remove last colon.
            statement.AppendLine();
            statement.AppendFormat("from {0}", table.FullName(this.DbProvider.KeywordBrackets));
        }
        #endregion
    }



    /// <summary>
    /// Represents a insert clause.
    /// </summary>
    /// <typeparam name="T">Type information of table model.</typeparam>
    internal sealed class InsertClause<T> : Clause, IInsertClause<T>
    {
        #region Properties
        /// <summary>
        /// Gets if use sequence.
        /// </summary>
        private bool UseSequence { get; }


        /// <summary>
        /// Gets if set identity.
        /// </summary>
        private bool SetIdentity { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        /// <param name="useSequence">Whether use sequence</param>
        /// <param name="setIdentity">Whether set identity</param>
        internal InsertClause(DbProvider provider, IClause previous, bool useSequence, bool setIdentity)
            : base(provider, previous)
        {
            this.UseSequence = useSequence;
            this.SetIdentity = setIdentity;
        }
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            var table = TableMappingInfo.Create(typeof(T));
            var columns = table.Columns.Where(x => this.SetIdentity ? true : !x.IsAutoIncrement);

            //--- build sql
            statement.AppendFormat("insert into {0}", table.FullName(this.DbProvider.KeywordBrackets));
            statement.AppendLine();
            statement.Append("(");
            foreach (var x in columns)
            {
                statement.AppendLine();
                statement.AppendFormat("    {0},", x.ColumnName(this.DbProvider.KeywordBrackets));
            }
            statement.Length--;  //--- remove last colon.
            statement.AppendLine();
            statement.AppendLine(")");
            statement.AppendLine("values");
            statement.Append("(");
            foreach (var x in columns)
            {
                statement.AppendLine();
                statement.Append("    ");
                if (!this.UseSequence || x.Sequence == null)
                {
                    statement.AppendFormat("{0}{1},", this.DbProvider.BindParameterPrefix, x.PropertyName);
                    continue;
                }
                switch (this.DbProvider.Kind)
                {
                    case DbKind.SqlServer:
                        statement.AppendFormat("next value for {0},", x.Sequence.FullName(this.DbProvider.KeywordBrackets));
                        break;

                    //case DbKind.Oracle:
                        //builder.AppendFormat("{0}.nextval,", x.Sequence.FullName(this.DbProvider.KeywordBrackets));
                        //break;

                    case DbKind.PostgreSql:
                        statement.AppendFormat("nextval('{0}'),", x.Sequence.FullName(this.DbProvider.KeywordBrackets));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            statement.Length--;  //--- remove last colon.
            statement.AppendLine();
            statement.Append(")");
        }
        #endregion
    }



    /// <summary>
    /// Represents a update clause.
    /// </summary>
    /// <typeparam name="T">Type information of table model.</typeparam>
    internal sealed class UpdateClause<T> : Clause, IUpdateClause<T>
    {
        #region Properties
        /// <summary>
        /// Gets property expressions mapped columns.
        /// </summary>
        private Expression<Func<T, object>> Properties { get; }


        /// <summary>
        /// Gets if set identity.
        /// </summary>
        private bool SetIdentity { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        /// <param name="properties">Property expressions mapped columns. If null, targets all columns.</param>
        /// <param name="setIdentity">Whether set identity</param>
        internal UpdateClause(DbProvider provider, IClause previous, Expression<Func<T, object>> properties, bool setIdentity)
            : base(provider, previous)
        {
            this.Properties = properties;
            this.SetIdentity = setIdentity;
        }
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            var propertyNames = this.Properties == null
                              ? Enumerable.Empty<string>()
                              : ExpressionHelper.GetMemberNames(this.Properties);
            var table   = TableMappingInfo.Create(typeof(T));
            var columns = table.Columns.Where(x => this.SetIdentity ? true : !x.IsAutoIncrement);
            if (propertyNames.Any())
                columns = columns.Join(propertyNames, x => x.PropertyName, y => y, (x, y) => x);

            //--- build sql
            statement.AppendLine($"update {table.FullName(this.DbProvider.KeywordBrackets)}");
            statement.Append("set");
            foreach (var x in columns)
            {
                statement.AppendLine();
                statement.AppendFormat("    {0} = {1}{2},", x.ColumnName(this.DbProvider.KeywordBrackets), this.DbProvider.BindParameterPrefix, x.PropertyName);
            }
            statement.Length--;  //--- remove last colon.
        }
        #endregion
    }



    /// <summary>
    /// Represents a delete clause.
    /// </summary>
    /// <typeparam name="T">Type information of table model.</typeparam>
    internal sealed class DeleteClause<T> : Clause, IDeleteClause<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        internal DeleteClause(DbProvider provider, IClause previous)
            : base(provider, previous)
        {}
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            var tableName = TableMappingInfo.Create(typeof(T)).FullName(this.DbProvider.KeywordBrackets);
            statement.AppendFormat("delete from {0}", tableName);
        }
        #endregion
    }



    /// <summary>
    /// Represents a truncate clause.
    /// </summary>
    /// <typeparam name="T">Type information of table model.</typeparam>
    internal sealed class TruncateClause<T> : Clause, ITruncateClause<T>
    {
        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        internal TruncateClause(DbProvider provider, IClause previous)
            : base(provider, previous)
        {}
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            var tableName = TableMappingInfo.Create(typeof(T)).FullName(this.DbProvider.KeywordBrackets);
            statement.AppendFormat("truncate table {0}", tableName);
        }
        #endregion
    }



    /// <summary>
    /// Represents a where clause.
    /// </summary>
    /// <typeparam name="T">Type information of table model.</typeparam>
    internal sealed class WhereClause<T> : Clause, IWhereClause<T>
    {
        #region Properties
        /// <summary>
        /// Gets filter condition expression.
        /// </summary>
        private Expression<Func<T, bool>> Predicate { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="provider">Database provider</param>
        /// <param name="previous">Previous clause</param>
        /// <param name="predicate">Predicative expression</param>
        internal WhereClause(DbProvider provider, IClause previous, Expression<Func<T, bool>> predicate)
            : base(provider, previous)
        {
            this.Predicate = predicate;
        }
        #endregion


        #region Override methods
        /// <summary>
        /// Builds query core.
        /// </summary>
        /// <param name="statement">Statement builder</param>
        /// <param name="whereParameters">Where clause bind parameters</param>
        internal override void Build(StringBuilder statement, IDictionary<string, object> whereParameters)
        {
            //--- 親を実行する
            if (this.Previous != null)
            {
                this.Previous.Build(statement, whereParameters);
                statement.AppendLine();
                statement.AppendLine("where");
                statement.Append("    ");
            }   

            //--- 要素分解
            var root = PredicateParser.Parse(this.Predicate);

            //--- バインド変数の個数の桁を算出
            var parameterCount  = root.DescendantsAndSelf().Count(x =>
                                {
                                    return  x.Operator != PredicateOperator.AndAlso
                                        &&  x.Operator != PredicateOperator.OrElse
                                        &&  x.Value != null;
                                });
            var digit = NumericsHelpers.GetDigit(parameterCount - 1);

            //--- 組み立て
            var columnMap = TableMappingInfo.Create<T>().Columns.ToDictionary(x => x.PropertyName);
            uint index = 0;
            this.BuildSql(statement, whereParameters, columnMap, ref index, ref digit, root);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parameter"></param>
        /// <param name="columnMap"></param>
        /// <param name="index"></param>
        /// <param name="digit"></param>
        /// <param name="element"></param>
        private void BuildSql(StringBuilder statement, IDictionary<string, object> parameter, IDictionary<string, ColumnMappingInfo> columnMap, ref uint index, ref int digit, PredicateElement element)
        {
            if (element.HasChildren)
            {
                //--- left
                {
                    var needsBrackets = element.Operator != element.Left.Operator && element.Left.HasChildren;
                    if (needsBrackets) statement.Append('(');
                    BuildSql(statement, parameter, columnMap, ref index, ref digit, element.Left);
                    if (needsBrackets) statement.Append(')');
                }

                //--- and / or
                if (element.Operator == PredicateOperator.AndAlso) statement.Append(" and ");
                if (element.Operator == PredicateOperator.OrElse)  statement.Append(" or ");

                //--- right
                {
                    var needsBrackets = element.Operator != element.Right.Operator && element.Right.HasChildren;
                    if (needsBrackets) statement.Append('(');
                    BuildSql(statement, parameter, columnMap, ref index, ref digit, element.Right);
                    if (needsBrackets) statement.Append(')');
                }
            }
            else
            {
                statement.Append(columnMap[element.PropertyName].ColumnName(this.DbProvider.KeywordBrackets));
                switch (element.Operator)
                {
                    case PredicateOperator.Equal:
                        if (element.Value == null)
                        {
                            statement.Append(" is null");
                            return;
                        }
                        statement.Append(" = ");
                        break;

                    case PredicateOperator.NotEqual:
                        if (element.Value == null)
                        {
                            statement.Append(" is not null");
                            return;
                        }
                        statement.Append(" <> ");
                        break;

                    case PredicateOperator.LessThan:            statement.Append(" < ");  break;
                    case PredicateOperator.LessThanOrEqual:     statement.Append(" <= "); break;
                    case PredicateOperator.GreaterThan:         statement.Append(" > ");  break;
                    case PredicateOperator.GreaterThanOrEqual:  statement.Append(" >= "); break;
                    case PredicateOperator.Contains:            statement.Append(" in "); break;
                    default:                                    throw new InvalidOperationException();
                }

                var digitFormat = $"D{digit}";
                var parameterName = $"p{index.ToString(digitFormat)}";
                ++index;
                parameter.Add(parameterName, element.Value);  //--- cache parameter
                statement.Append(this.DbProvider.BindParameterPrefix);
                statement.Append(parameterName);
            }
        }
        #endregion
    }


    //--- todo : top
    //--- todo : order by

    #endregion
}