// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using Catpic.Gadgets.Security;
    using Catpic.Social;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class SocialRule : IRule
    {
        /// <summary>
        /// Service name
        /// </summary>
        protected readonly string Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialRule"/> class.
        /// </summary>
        protected SocialRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialRule"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        protected SocialRule(string service)
        {
            this.Service = service;
        }

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
            if (!string.IsNullOrEmpty(this.Service) && requestItem.ServiceName != this.Service)
            {
                return true;
            }

            switch (requestItem.Operation)
            {
                case SocialConsts.Get:
                    return this.ValidateGet(requestItem, token, context);
                case SocialConsts.Create:
                    return this.ValidateCreate(requestItem, token, context);
                case SocialConsts.Update:
                    return this.ValidateUpdate(requestItem, token, context);
                case SocialConsts.Delete:
                    return this.ValidateDelete(requestItem, token, context);
                default:
                    return this.ValidateSpecialCase(requestItem, token, context);
            }
        }

        #endregion

        /// <summary>
        /// Validates get operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected virtual bool ValidateGet(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return true;
        }

        /// <summary>
        /// Validates create operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected virtual bool ValidateCreate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return true;
        }

        /// <summary>
        /// Validates update operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected virtual bool ValidateUpdate(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return true;
        }

        /// <summary>
        /// Validates delete operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected virtual bool ValidateDelete(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            return true;
        }

        /// <summary>
        /// Validates specific operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected virtual bool ValidateSpecialCase(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            // NOTE always false here
            return false;
        }
    }
}
