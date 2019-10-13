namespace DeclarativeSql.Sql.Clauses
{
    public interface IClause<T> : ISql<T> { }
    public interface IWhere<T> : IClause<T> { }
    public interface IWhereForSelect<T> : IClause<T> { }
    public interface IOrderBy<T> : IClause<T> { }
    public interface IThenBy<T> : IClause<T> { }
}