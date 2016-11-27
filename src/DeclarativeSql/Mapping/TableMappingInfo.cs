using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using This = DeclarativeSql.Mapping.TableMappingInfo;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// Provides mapping information between table and object.
    /// </summary>
    public sealed class TableMappingInfo
    {
        #region Fields
        /// <summary>
        /// Store cache of type mapping information.
        /// </summary>
        private static IDictionary<Type, This> cache = new Dictionary<Type, This>();
        #endregion


        #region Properties
        /// <summary>
        /// Gets the type to be mapped.
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string Schema { get; private set; }


        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// Gets a collection of column mapping information.
        /// </summary>
        public IReadOnlyList<ColumnMappingInfo> Columns { get; private set; }


        /// <summary>
        /// Gets the full name that combines the schema name and table name.
        /// </summary>
        public string FullName => string.IsNullOrWhiteSpace(this.Schema)
                                ? this.Name
                                : $"{this.Schema}.{this.Name}";
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        private TableMappingInfo()
        { }
        #endregion


        #region Generate
        /// <summary>
        /// Generates mapping information for the specified type.
        /// </summary>
        /// <typeparam name="T">Target type information</typeparam>
        /// <returns>Table mapping information</returns>
        public static This Create<T>() => This.Create(typeof(T));


        /// <summary>
        /// Generates mapping information for the specified type.。
        /// </summary>
        /// <param name="type">Target type information</param>
        /// <returns>Table mapping information</returns>
        public static This Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            lock (This.cache)
            {
                //--- gets from cache
                This result = null;
                if (!This.cache.TryGetValue(type, out result))
                {
                    //--- table info
                    var typeInfo = type.GetTypeInfo();
                    var table = typeInfo.GetCustomAttribute<TableAttribute>(false);
                    result = new This()
                    {
                        Schema = table?.Schema ?? null,
                        Name = table?.Name ?? type.Name,
                        Type = type
                    };

                    //--- column info
                    var flags = BindingFlags.Instance | BindingFlags.Public;
                    result.Columns = typeInfo.GetProperties(flags)
                                    .Where(x => x.CustomAttributes.All(y => y.AttributeType != typeof(NotMappedAttribute)))
                                    .Select(ColumnMappingInfo.From)
                                    .ToArray();

                    //--- cache
                    This.cache.Add(type, result);
                }
                return result;
            }
        }
        #endregion
    }
}