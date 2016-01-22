using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// 自動採番を表す属性を表します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoIncrementAttribute : Attribute
    {
        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        public AutoIncrementAttribute()
        {}
        #endregion
    }
}