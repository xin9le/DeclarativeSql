using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// Provides the extension functions for System.Dynamic.ExpandoObject.
    /// </summary>
    internal static class ExpandoObjectExtensions
    {
        /// <summary>
        /// Combines the specified instance property with the specified ExpandoObject.
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        /// <param name="self">Target ExpandoObject instance to combine.</param>
        /// <param name="instance">Target instance.</param>
        /// <param name="includeNonPublic">Whether non-public properties are also combined.</param>
        /// <param name="includeNotMapped">Whether properties which has NotMappedAttribute are also combined.</param>
        /// <returns>Combined instance</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, bool includeNonPublic = false, bool includeNotMapped = false)
        {
            if (self == null)     throw new ArgumentNullException(nameof(self));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var propertyNames
                = typeof(T)
                .GetRuntimeProperties()
                .Where(x => !x.GetMethod.IsStatic)
                .Where(x => includeNonPublic || x.GetMethod.IsPublic)
                .Where(x => includeNotMapped || !x.IsDefined<NotMappedAttribute>())
                .Select(x => x.Name);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// Combines the specified instance property with the specified ExpandoObject.
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        /// <param name="self">Target ExpandoObject instance to combine.</param>
        /// <param name="instance">Target instance.</param>
        /// <param name="properties">Target properties.</param>
        /// <returns>Combined instance</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, Expression<Func<T, object>> properties)
        {
            if (self == null)     throw new ArgumentNullException(nameof(self));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (properties == null)
                return self.Merge(instance);

            var propertyNames = ExpressionHelper.GetMemberNames(properties);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// Combines the specified instance property with the specified ExpandoObject.
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        /// <param name="self">Target ExpandoObject instance to combine.</param>
        /// <param name="instance">Target instance.</param>
        /// <param name="properties">Target properties.</param>
        /// <returns>Combined instance</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, IEnumerable<Expression<Func<T, object>>> properties)
        {
            if (self == null)     throw new ArgumentNullException(nameof(self));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (properties == null)
                return self.Merge(instance);

            properties = properties.Materialize();
            if (properties.IsEmpty())
                return self.Merge(instance);

            var propertyNames = ExpressionHelper.GetMemberNames(properties);
            return self.Merge(instance, propertyNames);
        }


        /// <summary>
        /// Combines the specified instance property with the specified ExpandoObject.
        /// </summary>
        /// <typeparam name="T">Instance type</typeparam>
        /// <param name="self">Target ExpandoObject instance to combine.</param>
        /// <param name="instance">Target instance.</param>
        /// <param name="propertyNames">Target property names.</param>
        /// <returns>Combined instance</returns>
        public static ExpandoObject Merge<T>(this ExpandoObject self, T instance, IEnumerable<string> propertyNames)
        {
            if (self == null)          throw new ArgumentNullException(nameof(self));
            if (instance == null)      throw new ArgumentNullException(nameof(instance));
            if (propertyNames == null) throw new ArgumentNullException(nameof(propertyNames));

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