namespace DeclarativeSql.Sql.Statements
{
    public interface IStatement<T> : ISql { }
    public interface ICount<T> : IStatement<T> { }
    public interface ISelect<T> : IStatement<T> { }
    public interface IInsert<T> : IStatement<T> { }
    public interface IUpdate<T> : IStatement<T> { }
    public interface IDelete<T> : IStatement<T> { }
    public interface ITruncate<T> : IStatement<T> { }
}
