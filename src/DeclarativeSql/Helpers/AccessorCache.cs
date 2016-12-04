using System;
using System.Collections.Concurrent;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides the cache for delegate to access properties and fields.
    /// </summary>
    internal static class AccessorCache
    {
        #region Fields
        /// <summary>
        /// Holds the cache for get delegates.
        /// </summary>
        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> getters = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        #endregion


        #region Getters
        /// <summary>
        /// Gets the get delegate that matches the specified type and member name.
        /// </summary>
        /// <param name="type">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <returns>Get delegate</returns>
        public static Func<object, object> LookupGet(Type type, string memberName)
        {
            var key = Tuple.Create(type, memberName);
            return getters.GetOrAdd(key, x => AccessorFactory.CreateGetDelegate(x.Item1, x.Item2));
        }
        #endregion
    }


    /// <summary>
    /// Provides the cache for delegate to access properties and fields.
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    internal static class AccessorCache<T>
    {
        #region Fields
        /// <summary>
        /// Holds the cache for get delegates.
        /// </summary>
        private static ConcurrentDictionary<string, Func<T, object>> getters = new ConcurrentDictionary<string, Func<T, object>>();
        #endregion


        #region Getters
        /// <summary>
        /// Gets the get delegate that matches the specified member name.
        /// </summary>
        /// <param name="memberName">Target member name</param>
        /// <returns>Get delegate</returns>
        public static Func<T, object> LookupGet(string memberName)
            => getters.GetOrAdd(memberName, AccessorFactory.CreateGetDelegate<T>);
        #endregion
    }
}