using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// データソースのデータプロバイダー名を表す属性を表します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class ProviderNameAttribute : Attribute
    {
        #region プロパティ
        /// <summary>
        /// データソースのデータプロバイダー名を保持します。
        /// </summary>
        public string ProviderName{ get; }
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="providerName">データプロバイダー名</param>
        public ProviderNameAttribute(string providerName)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));
            this.ProviderName = providerName;
        }
        #endregion
    }
}