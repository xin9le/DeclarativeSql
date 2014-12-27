using System;
using System.Linq;
using System.Reflection;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 属性の補助機能を提供します。
    /// </summary>
    internal static class AttributeHelper
    {
        /// <summary>
        /// 指定されたプロパティに指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="T">属性の型</typeparam>
        /// <param name="info">プロパティ情報</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool HasAttribute<T>(this PropertyInfo info)
            where T : Attribute
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            return info.CustomAttributes.Any(x => x.AttributeType == typeof(T));
        }
    }
}