// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Validates message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Rules
{
    using System;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Social.Messages;

    /// <summary>
    /// Validates message
    /// </summary>
    public class MessageRule : SocialRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRule"/> class.
        /// </summary>
        public MessageRule() : base(SocialConsts.MessageServiceName)
        {
        }

        /// <summary>
        /// Validates get operation
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns> True if validation is passed</returns>
        protected override bool ValidateGet(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            if (requestItem.Params.UserId != token.OwnerId)
            {
                context.ValidationErrors.Add(string.Format("unable to get messages for {0}", requestItem.Params.UserId));
                return false;
            }

            return base.ValidateGet(requestItem, token, context);
        }

        /// <summary>
        /// Validates send
        /// </summary>
        /// <param name="requestItem"> The request item. </param>
        /// <param name="token"> The token. </param>
        /// <param name="context"> The context. </param>
        /// <returns>Validation result </returns>
        protected override bool ValidateSpecialCase(RequestItem requestItem, ISecurityToken token, RuleContext context)
        {
            if (requestItem.Operation != "send")
            {
                // unknown operation, apply default behavior
                return base.ValidateSpecialCase(requestItem, token, context);
            }

            var item = requestItem.Entity as MessageItem<Message>;
            if (item != null && item.Message != null)
            {
                // do not allow to send message in behalf of different user
                item.Message.SenderId = token.OwnerId;
                item.Message.Updated = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
                item.Message.TimeSent = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
                return true;
            }

            return false;
        }

    }
}
