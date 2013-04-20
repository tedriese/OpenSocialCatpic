// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestItem.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents social service operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using Catpic.Social.Formatting;

    /// <summary>
    /// Represents social service operation
    /// </summary>
    public class RequestItem
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets name of service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets name of operation
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets Params.
        /// </summary>
        public RequestParamsItem Params { get; set; }

        /// <summary>
        /// Gets or sets entity which stores entity associated with service operation
        /// NOTE: make type as RequestParamsItem?
        /// </summary>
        public object Entity { get; set; }
    }
}
