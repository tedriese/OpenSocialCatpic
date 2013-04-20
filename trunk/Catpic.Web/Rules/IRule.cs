// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents rule for social services
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// Represents rule for social services
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Checks request 
        /// </summary>
        /// <param name="requestItem"> The request item.  </param>
        /// <param name="token"> The token.  </param>
        /// <param name="context"> The context. </param>
        /// <returns> Validation result </returns>
        bool Validate(RequestItem requestItem, ISecurityToken token, RuleContext context);
    }
}
