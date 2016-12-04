using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using This = DeclarativeSql.Helpers.TypeHelper;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides helper functions for type information.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Provides the type information of element from type information representing the specified collection.
        /// </summary>
        /// <typeparam name="TCollection">Type information of colleciton</typeparam>
        /// <returns>Type information of element</returns>
        public static Type GetElementType<TCollection>() => This.GetElementType(typeof(TCollection));


        /// <summary>
        /// Provides the type information of element from type information representing the specified collection.
        /// </summary>
        /// <param name="collectionType">Type information of colleciton</param>
        /// <returns>Type information of element</returns>
        /// <remarks>Returns null if fails to get type information of element.</remarks>
        public static Type GetElementType(this Type collectionType)
        {
            if (collectionType == null)
                throw new ArgumentNullException(nameof(collectionType));

            var type = collectionType.GetTypeInfo();
            return  new []{ type }
                    .Where(x => x.IsInterface)
                    .Concat(type.GetInterfaces().Select(x => x.GetTypeInfo()))
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(x => x.GetGenericArguments()[0])
                    .FirstOrDefault();
        }


        /// <summary>
        /// Checks whether the specified type is a collection type.
        /// </summary>
        /// <typeparam name="T">Type information</typeparam>
        /// <returns>Returns true if specified type is collection.</returns>
        public static bool IsCollection<T>() => typeof(T).IsCollection();


        /// <summary>
        /// Checks whether the specified type is a collection type.
        /// </summary>
        /// <param name="type">Type information</param>
        /// <returns>Returns true if specified type is collection.</returns>
        public static bool IsCollection(this Type type) => This.GetElementType(type) != null;
    }
}