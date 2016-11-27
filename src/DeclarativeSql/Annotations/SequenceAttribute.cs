using System;



namespace DeclarativeSql.Annotations
{
    /// <summary>
    /// Represents an attribute representing a sequence name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SequenceAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Gets sequence name.
        /// </summary>
        public string Name{ get; }


        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        public string Schema{ get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="name">sequence name</param>
        public SequenceAttribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }
        #endregion
    }
}