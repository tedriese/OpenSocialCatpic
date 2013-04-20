// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialHandler.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides base methods for social service handlers
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catpic.Gadgets.Security;
    using Catpic.Social.DTO;
    using Catpic.Social.Formatting;
    using Catpic.Utils;

    /// <summary>
    /// Provides base methods for social service handlers 
    /// </summary>
    public abstract class SocialHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocialHandler"/> class.
        /// </summary>
        /// <param name="name">Name of social service registration</param>
        protected SocialHandler(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets Name.
        /// </summary>
        public string Name { get; private set; }

        #region Abstract members

        /// <summary>
        /// True, if request can be handled
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>true, if request can be processed</returns>
        public abstract bool Validate(RequestItem requestItem, ISecurityToken token);

        /// <summary>
        /// Processes request item 
        /// </summary>
        /// <param name="requestItem">Request item</param>
        /// <param name="token">Security token</param>
        /// <returns>Social service response</returns>
        public abstract Task<object> ProcessAsync(RequestItem requestItem, ISecurityToken token);

        #endregion

        /// <summary>
        /// Wraps single result in DTO record
        /// </summary>
        /// <param name="requestItem"> RequestItem object </param>
        /// <param name="result"> Abstract result </param>
        /// <returns> DTO object </returns>
        protected Record GetRecordResult(RequestItem requestItem, object result)
        {
            return new Record
                       {
                           Id = requestItem.Id,
                           Result = result
                       };
        }

        /// <summary>
        /// Wraps collection result in DTO record
        /// </summary>
        /// <param name="requestItem"> RequestItem object </param>
        /// <param name="collection"> Collection of social results </param>
        /// <returns> The get collection result. </returns>
        protected Collection GetCollectionResult(RequestItem requestItem, IEnumerable<object> collection)
        {
            var collectionItem = requestItem.Entity as CollectionItem;
            if (collectionItem == null)
            {
                throw new InvalidOperationException("Unable to process request: collection item is null");
            }

            return new Collection
                {
                    Id = requestItem.Id,
                    Result = new CollectionResultEntry
                            {
                                List = collection,
                                ItemsPerPage = collectionItem.Count,
                                StartIndex = collectionItem.StartIndex,
                                TotalResults = collection.Count(),
                                IsFiltered = !string.IsNullOrEmpty(collectionItem.FilterBy),
                                IsSorted = !string.IsNullOrEmpty(collectionItem.SortBy)
                            }
                };
        }

        /// <summary>
        /// Returns empty result
        /// </summary>
        /// <param name="requestItem"> RequestItem object </param>
        /// <returns> Empty result </returns>
        protected Record GetEmptyResult(RequestItem requestItem)
        {
            return new Record { Id = requestItem.Id, Result = new object() };
        }

        /// <summary>
        /// Wraps error in DTO
        /// </summary>
        /// <param name="requestItem"> RequestItem object </param>
        /// <param name="message"> Error description </param>
        /// <returns> Return error DTO </returns>
        protected Task<object> GetError(RequestItem requestItem, string message)
        {
            var result = new ErrorResult
                {
                    Id = requestItem.Id, Error = new ErrorDetails { Code = 500, Message = message } 
                };
            return AsyncHelper.GetEmptyTask<object>(result);
        }
    }
}
