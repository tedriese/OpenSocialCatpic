// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleContext.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents Rule context which is shared between rules in rule chain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents Rule context which is shared between rules in rule chain
    /// </summary>
    public class RuleContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleContext"/> class.
        /// </summary>
        public RuleContext()
        {
            this.ValidationErrors = new List<string>();
            this.Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets ValidationErrors.
        /// </summary>
        public IList<string> ValidationErrors { get; set; }

        /// <summary>
        /// Gets or sets Parameters.
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

    }
}
