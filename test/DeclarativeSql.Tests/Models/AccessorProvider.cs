namespace DeclarativeSql.Tests.Models
{
    internal class AccessorProvider
    {
        public string InstanceMethod() => "aaa";
        public int InstanceProperty => 31;
        public static string StaticMethod() => "aaa";
        public static int StaticProperty => 31;
    }
}