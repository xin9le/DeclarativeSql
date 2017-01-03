using System;
using DeclarativeSql.Mapping;



namespace DeclarativeSql
{
    /// <summary>
    /// Extension functions for kyeword bracket.
    /// </summary>
    public static class KeywordBracketsExtensions
    {
        #region TableMappingInfo
        /// <summary>
        /// Creates schema name.
        /// </summary>
        /// <param name="self">Table mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Schema name</returns>
        public static string Schema(this TableMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Schema
                :   $"{bracket.Begin}{self.Schema}{bracket.End}";
        }


        /// <summary>
        /// Creates table name.
        /// </summary>
        /// <param name="self">Table mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Table name</returns>
        public static string Name(this TableMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Name
                :   $"{bracket.Begin}{self.Name}{bracket.End}";
        }


        /// <summary>
        /// Creates full table name (Schema + Name).
        /// </summary>
        /// <param name="self">Table mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Full table name</returns>
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
        /// Creates column name.
        /// </summary>
        /// <param name="self">Column mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Column name</returns>
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
        /// Creates schema name.
        /// </summary>
        /// <param name="self">Sequence mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Schema name</returns>
        public static string Schema(this SequenceMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Schema
                :   $"{bracket.Begin}{self.Schema}{bracket.End}";
        }


        /// <summary>
        /// Creates sequence name.
        /// </summary>
        /// <param name="self">Sequence mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Sequence name</returns>
        public static string Name(this SequenceMappingInfo self, BracketPair bracket)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return  bracket == null
                ?   self.Name
                :   $"{bracket.Begin}{self.Name}{bracket.End}";
        }


        /// <summary>
        /// Creates full sequence name (Schema + Name).
        /// </summary>
        /// <param name="self">Sequence mapping information</param>
        /// <param name="bracket">Keyword brackets</param>
        /// <returns>Full sequence name</returns>
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