// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserIdRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Modifies user id
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using System;

    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// Modifies user id
    /// </summary>
    public class UserIdRule : IRule
    {
        #region Implementation of IRule

        /// <summary>
        /// Checks request 
        /// </summary>
        /// <param name="requestItem"> The request item.  </param>
        /// <param name="token"> The token.  </param>
        /// <param name="context"> The context. </param>
        /// <returns> Validation result </returns>
        public bool Validate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            try
            {
                // translate user id
                requestItem.Params.UserId = this.GetUserId(requestItem.Params.UserId, token);
                return true;
            }
            catch (Exception ex)
            {
                context.ValidationErrors.Add(ex.Message);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Translate user id received from client to internal user id
        /// </summary>
        /// <param name="rawValue"> raw user id </param>
        /// <param name="token"> Security token </param>
        /// <returns> Internal user id </returns>
        protected string GetUserId(string rawValue, ISecurityToken token)
        {
            switch (rawValue)
            {
                case SocialConsts.UserIdViewer:
                case SocialConsts.UserIdMe:
                    return token.ViewerId;
                case SocialConsts.UserIdOwner:
                    return token.OwnerId;
                default:
                    return rawValue;
            }
        }
    }
}
