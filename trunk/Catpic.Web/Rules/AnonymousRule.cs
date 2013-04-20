// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides default behaviour for anonymous user. Only get is allowed for anonymous user
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using Catpic.Gadgets;
    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// Provides default behaviour for anonymous user. Only get is allowed for anonymous user
    /// </summary>
    public class AnonymousRule : SocialRule
    {
        /// <summary>
        /// Validates create operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected override bool ValidateCreate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return this.IsPermittedAction(requestItem, token, context);
        }

        /// <summary>
        /// Validates delete operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected override bool ValidateDelete(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return this.IsPermittedAction(requestItem, token, context);
        }

        /// <summary>
        /// Validates update operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected override bool ValidateUpdate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return this.IsPermittedAction(requestItem, token, context);
        }

        /// <summary>
        /// Validate user id
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> Validation result </returns>
        private bool IsPermittedAction(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            if (token.OwnerId == GadgetConsts.AnonymousName)
            {
                context.ValidationErrors.Add("operation is not allowed for anonymous user");
                return false;
            }

            return true;
        }
    }
}
