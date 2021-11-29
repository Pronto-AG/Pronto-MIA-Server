namespace Pronto_MIA.BusinessLogic.API.Logging
{
    /// <summary>
    /// Struct representing a query variable of graphQL.
    /// </summary>
    public struct QueryVariable
    {
        /// <summary>
        /// Gets or sets the name of the query variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the query variable.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the query variable.
        /// </summary>
        public string Value { get; set; }
    }
}
