using System;
using System.Linq.Expressions;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides delegate generation functions.
    /// </summary>
    internal static class AccessorFactory
    {
        #region Get Accessor
        /// <summary>
        /// Generates a Get delegate by specified type and member name.
        /// </summary>
        /// <param name="type">Target type</param>
        /// <param name="memberName">Target member name</param>
        /// <returns>Get delegate</returns>
        /// <remarks>
        /// (object target) => (object)((T)target).MemberName
        /// </remarks>
        public static Func<object, object> CreateGetDelegate(Type type, string memberName)
        {
            var target          = Expression.Parameter(typeof(object), "target");
            var convertToType   = Expression.Convert(target, type);
            var memberValue     = Expression.PropertyOrField(convertToType, memberName);
            var convertToObject = Expression.Convert(memberValue, typeof(object));
            var lambda          = Expression.Lambda(convertToObject, target);
            return (Func<object, object>)lambda.Compile();
        }


        /// <summary>
        /// Generates a Get delegate by specified type and member name.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <returns>Get delegate</returns>
        /// <remarks>
        /// (T target) => (object)target.MemberName
        /// </remarks>
        public static Func<T, object> CreateGetDelegate<T>(string memberName)
        {
            var target          = Expression.Parameter(typeof(T), "target");
            var memberValue     = Expression.PropertyOrField(target, memberName);
            var convertToObject = Expression.Convert(memberValue, typeof(object));
            var lambda          = Expression.Lambda(convertToObject, target);
            return (Func<T, object>)lambda.Compile();
        }


        /// <summary>
        /// Generates a Get delegate by specified type and member name.
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <typeparam name="TResult">Return type</typeparam>
        /// <param name="memberName">Target member name</param>
        /// <returns>Get用のデリゲート</returns>
        /// <remarks>
        /// (T target) => target.MemberName
        /// </remarks>
        public static Func<T, TResult> CreateGetDelegate<T, TResult>(string memberName)
        {
            var target      = Expression.Parameter(typeof(T), "target");
            var memberValue = Expression.PropertyOrField(target, memberName);
            var lambda      = Expression.Lambda(memberValue, target);
            return (Func<T, TResult>)lambda.Compile();
        }
        #endregion
    }
}