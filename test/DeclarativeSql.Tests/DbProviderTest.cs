using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace DeclarativeSql.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://blogs.msdn.microsoft.com/visualstudioalm/2016/09/01/announcing-mstest-v2-framework-support-for-net-core-1-0-rtm/
    /// </remarks>
    [TestClass]
    public class DbProviderTest
    {
        #region Factory
        [TestMethod]
        public void SqlServerFactory生成()
            => DbProvider.SqlServer.Factory.IsNotNull();

        
        [TestMethod]
        public void MySqlFactory生成()
            => DbProvider.MySql.Factory.IsNotNull();


        [TestMethod]
        public void SqliteFactory生成()
            => DbProvider.Sqlite.Factory.IsNotNull();
        #endregion
    }
}