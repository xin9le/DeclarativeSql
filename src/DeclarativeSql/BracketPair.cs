namespace DeclarativeSql
{
    /// <summary>
    /// Represents begin/end bracket pair.
    /// </summary>
    public class BracketPair
    {
        #region Properties
        /// <summary>
        /// Gets begin bracket.
        /// </summary>
        public char Begin { get; }


        /// <summary>
        /// Gets end bracket.
        /// </summary>
        public char End { get; }
        #endregion


        #region Constructors / Destructors
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="begin">Begin bracket character.</param>
        /// <param name="end">End bracket character.</param>
        internal BracketPair(char begin, char end)
        {
            this.Begin = begin;
            this.End = end;
        }
        #endregion
    }
}