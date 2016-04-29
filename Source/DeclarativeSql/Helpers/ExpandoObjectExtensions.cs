﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// System.Dynamic.ExpandoObject型に対する拡張機能を提供します。
    /// </summary>
    internal static class ExpandoObjectExtensions
    {
        /// <summary>
        /// 指定されたExpandoObjectに指定のインスタンスプロパティを結合します。
        /// </summary>
        /// <typeparam name="T">インスタンスの型</typeparam>
        /// <param name="self">結合先のExpandoObjectインスタンス</param>
        /// <param name="instance">結合するインスタンス</param>
        /// <param name="includeNonPublic">非パブリックなプロパティも結合するかどうか</param>
        /// <param name="includeNotMapped">NotMapped属性が付いたプロパティも結合するかどうか</param>
        /// <returns>結合されたインスタンス</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, bool includeNonPublic = false, bool includeNotMapped = false)
        {
            if (self == null)       throw new ArgumentNullException(nameof(self));
            if (instance == null)   throw new ArgumentNullException(nameof(instance));

            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (includeNonPublic)
                flags |= BindingFlags.NonPublic;

            var propertyNames   = typeof(T).GetProperties(flags)
                                .Where(x => includeNotMapped || !x.Has<NotMappedAttribute>())
                                .Select(x => x.Name);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// 指定されたExpandoObjectに指定されたインスタンスのプロパティを結合します。
        /// </summary>
        /// <typeparam name="T">インスタンスの型</typeparam>
        /// <param name="self">結合先のExpandoObjectインスタンス</param>
        /// <param name="instance">結合するインスタンス</param>
        /// <param name="properties">結合対象のプロパティ。nullの場合はNotMapped属性を持たないすべてのPublicプロパティが対象になります。</param>
        /// <returns>結合されたインスタンス</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, Expression<Func<T, object>> properties)
        {
            if (self == null)       throw new ArgumentNullException(nameof(self));
            if (instance == null)   throw new ArgumentNullException(nameof(instance));

            if (properties == null)
                return self.Merge(instance);

            var propertyNames = ExpressionHelper.GetMemberNames(properties);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// 指定されたExpandoObjectに指定されたインスタンスのプロパティを結合します。
        /// </summary>
        /// <typeparam name="T">インスタンスの型</typeparam>
        /// <param name="self">結合先のExpandoObjectインスタンス</param>
        /// <param name="instance">結合するインスタンス</param>
        /// <param name="properties">結合対象のプロパティ。nullもしくは空の場合はNotMapped属性を持たないすべてのPublicプロパティが対象になります。</param>
        /// <returns>結合されたインスタンス</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, IEnumerable<Expression<Func<T, object>>> properties)
        {
            if (self == null)       throw new ArgumentNullException(nameof(self));
            if (instance == null)   throw new ArgumentNullException(nameof(instance));

            if (properties == null || properties.IsEmpty())
                return self.Merge(instance);

            var propertyNames = ExpressionHelper.GetMemberNames(properties);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// 指定されたExpandoObjectに指定のインスタンスプロパティを結合します。
        /// </summary>
        /// <typeparam name="T">インスタンスの型</typeparam>
        /// <param name="self">結合先のExpandoObjectインスタンス</param>
        /// <param name="instance">結合するインスタンス</param>
        /// <param name="propertyNames">結合対象のプロパティ名</param>
        /// <returns>結合されたインスタンス</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, IEnumerable<string> propertyNames)
        {
            if (self == null)           throw new ArgumentNullException(nameof(self));
            if (instance == null)       throw new ArgumentNullException(nameof(instance));
            if (propertyNames == null)  throw new ArgumentNullException(nameof(propertyNames));

            var appendable = self as IDictionary<string, object>;
            foreach (var name in propertyNames)
            {
                var getter = AccessorCache<T>.LookupGet(name);
                appendable.Add(name, getter(instance));
            }
            return self;
        }
    }
}