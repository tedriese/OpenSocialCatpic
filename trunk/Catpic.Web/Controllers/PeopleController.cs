// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeopleController.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides people service via REST protocol
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Catpic.Gadgets.Security;
    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Social.People;
    using Catpic.Web.Rules;

    /// <summary>
    /// Provides people service via REST protocol
    /// </summary>
    public class PeopleController : RestApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleController"/> class. 
        /// </summary>
        /// <param name="tokenFactory"> Security token factory.  </param>
        /// <param name="ruleChain"> The rule Chain. </param>
        /// <param name="services"> Social service handlers.  </param>
        public PeopleController(ISecurityTokenFactory tokenFactory, IRuleChain ruleChain, IEnumerable<SocialHandler> services)
            : base(tokenFactory, ruleChain, services)
        {
        }

        /// <summary>
        /// Gets self or friend list
        /// </summary>
        /// <param name="requestItemParams"> Request Item Params. </param>
        /// <param name="personItem"> Person item.  </param>
        /// <returns> Async task  </returns>
        [HttpGet]
        public Task<object> GetAsync([FromUri] RequestParamsItem requestItemParams, [FromUri] PersonItem<Person> personItem)
        {
            var requestItem = new RequestItem
                                          {
                                              Entity = personItem,
                                              Id = string.Empty,
                                              Operation = "get",
                                              ServiceName = "people",
                                              Params = requestItemParams
                                          };
            return this.ProcessRequestItem(requestItem);
        }

        /// <summary>
        /// Creates relationship between current user and person with provided id TODO: find way to remove Person dependency
        /// </summary>
        /// <param name="requestItemParams"> Request Item Params. </param>
        /// <param name="person"> The person. </param>
        /// <returns> The post async. </returns>
        public Task<object> PostAsync([FromUri] RequestParamsItem requestItemParams, [FromBody] Person person)
        {
            var requestItem = new RequestItem
                {
                    Entity = new PersonItem<Person>
                    {
                        Person = person
                    },
                    Params = requestItemParams,
                    Id = string.Empty,
                    Operation = "create",
                    ServiceName = "people"
                };

            return this.ProcessRequestItem(requestItem);
        }
    }
}
