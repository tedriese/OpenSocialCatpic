// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleChain.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using System.Collections.Generic;
    using System.Linq;

    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// Represents rules chain
    /// </summary>
    public class RuleChain : IRuleChain
    {
        /// <summary>
        /// Rules list
        /// </summary>
        private readonly IList<IRule> _rules = new List<IRule>();

        /// <summary>
        /// Fluent interface for rule addition
        /// </summary>
        /// <param name="rule"> The rule. </param>
        /// <returns> This instance</returns>
        public RuleChain AddRule(IRule rule)
        {
            this._rules.Add(rule);
            return this;
        }

        /// <summary>
        /// Validates request using rules chain
        /// </summary>
        /// <param name="requestItem"> The request item.  </param>
        /// <param name="token"> The token.  </param>
        /// <param name="context"> The context. </param>
        /// <returns> Validation result </returns>
        public bool Validate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return this._rules.All(r => r.Validate(requestItem, token, context));
        }
    }
}
