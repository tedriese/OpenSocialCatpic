// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuleChain.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents rules chain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// Represents rules chain
    /// </summary>
    public interface IRuleChain
    {
        /// <summary>
        /// Fluent interface for rule addition
        /// </summary>
        /// <param name="rule"> The rule. </param>
        /// <returns> This instance</returns>
        RuleChain AddRule(IRule rule);

        /// <summary>
        /// Validates request using rules chain
        /// </summary>
        /// <param name="requestItem"> The request item.  </param>
        /// <param name="token"> The token.  </param>
        /// <param name="context"> The context. </param>
        /// <returns> Validation result </returns>
        bool Validate(RequestItem requestItem, ISecurityToken token, RuleContext context);
    }
}