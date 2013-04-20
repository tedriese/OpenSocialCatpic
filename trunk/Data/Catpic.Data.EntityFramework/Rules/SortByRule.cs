// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortByRule.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Applies default sortBy behavior
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework.Rules
{
    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Web.Rules;

    /// <summary>
    /// Applies default sortBy behavior
    /// </summary>
    public class SortByRule : IRule
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
            var collectionItem = requestItem.Entity as CollectionItem;
            if (requestItem.Operation == "get" && collectionItem != null && collectionItem.SortBy == null)
            {
                // NOTE EF fails when it tries to apply skip operation without applying order
                collectionItem.SortBy = "id";
            }

            return true;
        }

        #endregion
    }
}
