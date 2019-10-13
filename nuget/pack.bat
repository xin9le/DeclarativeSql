dotnet pack ../src/DeclarativeSql/DeclarativeSql.csproj -c Release -o ./packages
dotnet pack ../src/DeclarativeSql.MicrosoftSqlClient/DeclarativeSql.MicrosoftSqlClient.csproj -c Release -o ./packages
dotnet pack ../src/DeclarativeSql.SystemSqlClient/DeclarativeSql.SystemSqlClient.csproj -c Release -o ./packages
