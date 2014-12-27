using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// 対象となるDbConnectionの型名を付与する属性を表します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class DbConnectionAttribute : Attribute
    {
        #region プロパティ
        /// <summary>
        /// DbConnectionの型名を取得します。
        /// </summary>
        public string TypeName{ get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="typeName">DbConnectionの型名</param>
        public DbConnectionAttribute(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException(nameof(typeName));
            this.TypeName = typeName;
        }
        #endregion
    }
}