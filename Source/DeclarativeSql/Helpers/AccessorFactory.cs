using System;
using System.Linq.Expressions;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// プロパティを高速にアクセスするためのデリゲート生成機能を提供します。
    /// </summary>
    internal static class AccessorFactory
    {
        #region Get Accessor
        /// <summary>
        /// 指定された型とメンバー名のGet用デリゲートを生成します。
        /// </summary>
        /// <param name="type">対象となる型</param>
        /// <param name="memberName">対象となるメンバー名</param>
        /// <returns>Get用のデリゲート</returns>
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
        /// 指定されたメンバー名のGet用デリゲートを生成します。
        /// </summary>
        /// <typeparam name="T">対象となる型</typeparam>
        /// <param name="memberName">対象となるメンバー名</param>
        /// <returns>Get用のデリゲート</returns>
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
        /// 指定されたメンバー名のGet用デリゲートを生成します。
        /// </summary>
        /// <typeparam name="T">対象となる型</typeparam>
        /// <typeparam name="TResult">取得されるデータ型</typeparam>
        /// <param name="memberName">対象となるメンバー名</param>
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