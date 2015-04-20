using System;
using System.Reflection;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 属性の補助機能を提供します。
    /// </summary>
    internal static class AttributeHelper
    {
        /// <summary>
        /// 指定された列挙体に指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型</typeparam>
        /// <param name="value">列挙体</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool Has<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var info = type.GetField(name);
            return info.Has<TAttribute>();
        }


        /// <summary>
        /// 指定されたアセンブリに指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型</typeparam>
        /// <param name="assembly">アセンブリ</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool Has<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            return Attribute.IsDefined(assembly, typeof(TAttribute));
        }


        /// <summary>
        /// 指定されたメンバーに指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型</typeparam>
        /// <param name="info">メンバー情報</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool Has<TAttribute>(this MemberInfo info)
            where TAttribute : Attribute
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            return Attribute.IsDefined(info, typeof(TAttribute));
        }


        /// <summary>
        /// 指定されたモジュールに指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型</typeparam>
        /// <param name="module">モジュール</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool Has<TAttribute>(this Module module)
            where TAttribute : Attribute
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            return Attribute.IsDefined(module, typeof(TAttribute));
        }


        /// <summary>
        /// 指定されたパラメーターに指定の属性が付与されているかどうかを取得します。
        /// </summary>
        /// <typeparam name="TAttribute">属性の型</typeparam>
        /// <param name="info">パラメーター情報</param>
        /// <returns>属性が存在する場合true</returns>
        public static bool Has<TAttribute>(this ParameterInfo info)
            where TAttribute : Attribute
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            return Attribute.IsDefined(info, typeof(TAttribute));
        }
    }
}