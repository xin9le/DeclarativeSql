using System;
using System.Collections.Generic;
using System.Linq;
using This = DeclarativeSql.Helpers.TypeHelper;



namespace DeclarativeSql.Helpers
{
    /// <summary>
    /// 型情報の補助機能を提供します。
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// 指定されたコレクションを表す型情報から要素の型情報を取得します。
        /// </summary>
        /// <typeparam name="TCollection">コレクションの型情報</typeparam>
        /// <returns></returns>
        public static Type GetElementType<TCollection>() => This.GetElementType(typeof(TCollection));


        /// <summary>
        /// 指定されたコレクションを表す型情報から要素の型情報を取得します。
        /// </summary>
        /// <param name="collectionType">コレクションの型情報</param>
        /// <returns>要素の型情報</returns>
        /// <remarks>要素の型が取得できない場合はnullを返します。</remarks>
        public static Type GetElementType(Type collectionType)
        {
            if (collectionType == null)
                throw new ArgumentNullException(nameof(collectionType));

            return  new []{ collectionType }
                    .Where(x => x.IsInterface)
                    .Concat(collectionType.GetInterfaces())
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(x => x.GetGenericArguments()[0])
                    .FirstOrDefault();
        }
    }
}