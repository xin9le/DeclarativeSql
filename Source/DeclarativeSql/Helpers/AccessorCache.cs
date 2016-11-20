using System;
using System.Collections.Concurrent;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// プロパティ/フィールドへのアクセスするためのデリゲートをキャッシュする機能を提供します。
    /// </summary>
    internal static class AccessorCache
    {
        #region フィールド
        /// <summary>
        /// Get用デリゲートをキャッシュします。
        /// </summary>
        private static ConcurrentDictionary<Tuple<Type, string>, Func<object, object>> getters = new ConcurrentDictionary<Tuple<Type, string>, Func<object, object>>();
        #endregion


        #region 取得
        /// <summary>
        /// 指定された型とメンバー名に一致するGet用デリゲートを取得します。
        /// </summary>
        /// <param name="type">対象となる型</param>
        /// <param name="memberName">対象となるメンバー名</param>
        /// <returns>Get用デリゲート</returns>
        public static Func<object, object> LookupGet(Type type, string memberName)
        {
            var key = Tuple.Create(type, memberName);
            return getters.GetOrAdd(key, x => AccessorFactory.CreateGetDelegate(x.Item1, x.Item2));
        }
        #endregion
    }


    /// <summary>
    /// プロパティ/フィールドへのアクセスするためのデリゲートをキャッシュする機能を提供します。
    /// </summary>
    /// <typeparam name="T">対象となる型</typeparam>
    internal static class AccessorCache<T>
    {
        #region フィールド
        /// <summary>
        /// Get用デリゲートをキャッシュします。
        /// </summary>
        private static ConcurrentDictionary<string, Func<T, object>> getters = new ConcurrentDictionary<string, Func<T, object>>();
        #endregion


        #region 取得
        /// <summary>
        /// 指定メンバー名に一致するGet用デリゲートを取得します。
        /// </summary>
        /// <param name="memberName">対象となるメンバー名</param>
        /// <returns>Get用デリゲート</returns>
        public static Func<T, object> LookupGet(string memberName)
            => getters.GetOrAdd(memberName, AccessorFactory.CreateGetDelegate<T>);
        #endregion
    }
}