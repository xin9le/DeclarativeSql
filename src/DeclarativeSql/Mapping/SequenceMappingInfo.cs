using System;
using DeclarativeSql.Annotations;
using This = DeclarativeSql.Mapping.SequenceMappingInfo;



namespace DeclarativeSql.Mapping
{
    /// <summary>
    /// Provides mapping information of the sequence.
    /// </summary>
    public sealed class SequenceMappingInfo
    {
        #region Properties
        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string Schema { get; }


        /// <summary>
        /// Gets the sequence name.
        /// </summary>
        public string Name { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="schema">Schema name</param>
        /// <param name="name">Sequence name</param>
        internal SequenceMappingInfo(string schema, string name)
        {
            this.Schema = schema;
            this.Name = name;
        }
        #endregion


        #region Generates
        /// <summary>
        /// Creates an instance from a sequence attribute.
        /// </summary>
        /// <param name="attribute">Sequence attribute</param>
        /// <returns>Generated instance</returns>
        internal static This From(SequenceAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            return new This(attribute.Schema, attribute.Name);
        }
        #endregion
    }
}