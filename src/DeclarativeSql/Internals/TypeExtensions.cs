﻿using System;



namespace DeclarativeSql.Internals
{
    /// <summary>
    /// Provides <see cref="Type"/> extension methods.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets whether specified type is <see cref="Nullable{T}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
