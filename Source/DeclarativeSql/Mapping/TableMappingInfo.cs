using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using This = DeclarativeSql.Mapping.TableMappingInfo;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// テーブルとオブジェクトのマッピング情報を提供します。
    /// </summary>
    public sealed class TableMappingInfo
    {
        #region フィールド
        /// <summary>
        /// 型とのマッピング情報のキャッシュを保持します。
        /// </summary>
        private static readonly IDictionary<Type, This> cache = new Dictionary<Type, This>();
        #endregion


        #region プロパティ
        /// <summary>
        /// マッピングする型を取得します。
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// スキーマ名を取得します。
        /// </summary>
        public string Schema { get; private set; }


        /// <summary>
        /// テーブル名を取得します。
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 列マッピング情報のコレクションを取得します。
        /// </summary>
        public IReadOnlyList<ColumnMappingInfo> Columns { get; private set; }


        /// <summary>
        /// スキーマ名とテーブル名を結合したフルネームを取得します。
        /// </summary>
        public string FullName => string.IsNullOrWhiteSpace(this.Schema)
                                ? this.Name
                                : $"{this.Schema}.{this.Name}";
        #endregion


        #region コンストラクタ
        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        private TableMappingInfo()
        { }
        #endregion


        #region 生成
        /// <summary>
        /// 指定された型に対するマッピング情報を生成します。
        /// </summary>
        /// <typeparam name="T">対象となる型情報</typeparam>
        /// <returns>テーブルマッピング情報</returns>
        public static This Create<T>() => This.Create(typeof(T));


        /// <summary>
        /// 指定された型に対するマッピング情報を生成します。
        /// </summary>
        /// <param name="type">対象となる型情報</param>
        /// <returns>テーブルマッピング情報</returns>
        public static This Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            This result = null;
            lock (This.cache)
            {
                //--- キャッシュから取得
                if (!This.cache.TryGetValue(type, out result))
                {
                    //--- テーブル情報
                    var table = type.GetCustomAttribute<TableAttribute>(false);
                    result = new This()
                    {
                        Schema  = table?.Schema ?? null,
                        Name    = table?.Name ?? type.Name,
                        Type    = type
                    };

                    //--- 列情報
                    var flags = BindingFlags.Instance | BindingFlags.Public;
                    var notMapped = typeof(NotMappedAttribute);
                    result.Columns = type.GetProperties(flags)
                                    .Where(x => x.CustomAttributes.All(y => y.AttributeType != notMapped))
                                    .Select(ColumnMappingInfo.From)
                                    .ToArray();

                    //--- キャッシュ
                    This.cache.Add(type, result);
                }
            }
            return result;
        }
        #endregion
    }
}