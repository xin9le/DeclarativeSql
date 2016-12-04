using System;
using DeclarativeSql.Mapping;



namespace DeclarativeSql
{
    /// <summary>
    /// 
    /// </summary>
    public static class KeywordBracketsExtensions
    {
        #region TableMappingInfo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string Schema(this TableMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Schema
                :   $"{bracket.Begin}{self.Schema}{bracket.End}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string Name(this TableMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Name
                :   $"{bracket.Begin}{self.Name}{bracket.End}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string FullName(this TableMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (bracket == null)
            {
                return  string.IsNullOrWhiteSpace(self.Schema)
                    ?   self.Name
                    :   $"{self.Schema}.{self.Name}";
            }
            else
            {
                var b = bracket.Begin;
                var e = bracket.End;
                return  string.IsNullOrWhiteSpace(self.Schema)
                    ?   $"{b}{self.Name}{e}"
                    :   $"{b}{self.Schema}{e}.{b}{self.Name}{e}";
            }
        }
        #endregion


        #region ColumnMappingInfo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string ColumnName(this ColumnMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.ColumnName
                :   $"{bracket.Begin}{self.ColumnName}{bracket.End}";
        }
        #endregion


        #region SequenceMappingInfo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string Schema(this SequenceMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Schema
                :   $"{bracket.Begin}{self.Schema}{bracket.End}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string Name(this SequenceMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Name
                :   $"{bracket.Begin}{self.Name}{bracket.End}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="bracket"></param>
        /// <returns></returns>
        public static string FullName(this SequenceMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (bracket == null)
            {
                return  string.IsNullOrWhiteSpace(self.Schema)
                    ?   self.Name
                    :   $"{self.Schema}.{self.Name}";
            }
            else
            {
                var b = bracket.Begin;
                var e = bracket.End;
                return  string.IsNullOrWhiteSpace(self.Schema)
                    ?   $"{b}{self.Name}{e}"
                    :   $"{b}{self.Schema}{e}.{b}{self.Name}{e}";
            }
        }
        #endregion
    }
}