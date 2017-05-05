using System;
using System.Reflection;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides auxiliary function for attribute.
    /// </summary>
    internal static class AttributeHelper
    {
        /// <summary>
        /// Gets whether the specified attribute is given to the specified enumeration.
        /// </summary>
        /// <typeparam name="TAttribute">Target type</typeparam>
        /// <param name="value">Enumeration</param>
        /// <returns>True if exists attribute</returns>
        public static bool IsDefined<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var info = type.GetRuntimeField(name);
            return info.IsDefined<TAttribute>();
        }


        /// <summary>
        /// Gets whether the specified attribute is given to the specified assembly.
        /// </summary>
        /// <typeparam name="TAttribute">Target type</typeparam>
        /// <param name="assembly">Assembly</param>
        /// <returns>True if exists attribute</returns>
        public static bool IsDefined<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            return assembly.IsDefined(typeof(TAttribute));
        }


        /// <summary>
        /// Gets whether the specified attribute is given to the specified member.
        /// </summary>
        /// <typeparam name="TAttribute">Target type</typeparam>
        /// <param name="info">Member information</param>
        /// <returns>True if exists attribute</returns>
        public static bool IsDefined<TAttribute>(this MemberInfo info)
            where TAttribute : Attribute
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            return info.IsDefined(typeof(TAttribute));
        }


        /// <summary>
        /// Gets whether the specified attribute is given to the specified module.
        /// </summary>
        /// <typeparam name="TAttribute">Target type</typeparam>
        /// <param name="module">Module</param>
        /// <returns>True if exists attribute</returns>
        public static bool IsDefined<TAttribute>(this Module module)
            where TAttribute : Attribute
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            return module.IsDefined(typeof(TAttribute));
        }


        /// <summary>
        /// Gets whether the specified attribute is given to the specified parameter.
        /// </summary>
        /// <typeparam name="TAttribute">Target type</typeparam>
        /// <param name="info">Parameter information</param>
        /// <returns>True if exists attribute</returns>
        public static bool IsDefined<TAttribute>(this ParameterInfo info)
            where TAttribute : Attribute
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            return info.IsDefined(typeof(TAttribute));
        }
    }
}