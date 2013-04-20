// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcFormatter.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Json formatter for rpc calls. It does a lot of works to translate json-request to internal representation (RequestItem)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using Catpic.Social;
    using Catpic.Social.Formatting;
    using Catpic.Utils;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Json formatter for rpc calls. It does a lot of works to translate json-request to internal representation (RequestItem)
    /// </summary>
    public class JsonRpcFormatter : JsonMediaTypeFormatter
    {
        #region OpenSocial RPC constants

        /// <summary>
        /// Default page size
        /// </summary>
        protected const int DefaultPageSize = 25;

        /// <summary>
        /// Id key
        /// </summary>
        protected const string Id = "id";

        /// <summary>
        /// Method key
        /// </summary>
        protected const string Method = "method";

        /// <summary>
        /// Params key
        /// </summary>
        protected const string Params = "params";

        /// <summary>
        /// Application id key
        /// </summary>
        protected const string AppId = "appId";

        /// <summary>
        /// User id key
        /// </summary>
        protected const string UserId = "userId";

        /// <summary>
        /// Group id key
        /// </summary>
        protected const string GroupId = "groupId";

        /// <summary>
        /// Fields key
        /// </summary>
        protected const string Fields = "fields";

        /// <summary>
        /// Filter field key
        /// </summary>
        protected const string FilterBy = "filterBy";

        /// <summary>
        /// Filter operation key
        /// </summary>
        protected const string FilterOp = "filterOp";

        /// <summary>
        /// Filter value key
        /// </summary>
        protected const string FilterValue = "filterValue";

        /// <summary>
        /// Sort field key
        /// </summary>
        protected const string SortBy = "sortBy";

        /// <summary>
        /// Sort order key
        /// </summary>
        protected const string SortOrder = "sortOrder";

        /// <summary>
        /// Start index key
        /// </summary>
        protected const string StartIndex = "startIndex";

        /// <summary>
        /// Count key
        /// </summary>
        protected const string Count = "count";

        #endregion

        /// <summary>
        /// Social types locator
        /// </summary>
        private readonly SocialTypeLocator _locator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcFormatter"/> class.
        /// </summary>
        /// <param name="locator"> Social type locator. </param>
        public JsonRpcFormatter(SocialTypeLocator locator)
        {
            _locator = locator;
            this.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        /// <summary>
        /// Standard formatter method
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <param name="readStream"> The read stream. </param>
        /// <param name="content"> The content. </param>
        /// <param name="formatterLogger"> The formatter logger. </param>
        /// <returns>Async task </returns>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            IList<RequestItem> requestItems = new List<RequestItem>();
            try
            {
                var reader = new StreamReader(readStream);
                var body = reader.ReadToEnd();
                var jsonRequests = JsonConvert.DeserializeObject<JContainer>(body);

                if (jsonRequests is JArray)
                {
                    foreach (var jsonRequest in jsonRequests)
                    {
                        requestItems.Add(CreateItem(jsonRequest));
                    }
                }
                else
                {
                    // it's single entity
                    requestItems.Add(CreateItem(jsonRequests));
                }
            }
            catch (Exception ex)
            {
                formatterLogger.LogError(string.Empty, ex.Message);
                requestItems = null;
            }

            return AsyncHelper.GetEmptyTask(requestItems as object);
        }

        /// <summary>
        /// Standard formatter method
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns> True if can read </returns>
        public override bool CanReadType(Type type)
        {
            return typeof(IEnumerable<RequestItem>).Equals(type);
        }

        /// <summary>
        /// Standard formatter method
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns> True if can read </returns>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        #region Protected methods

        /// <summary>
        /// Gets request id
        /// </summary>
        /// <param name="jsonRequest"> Json token Request. </param>
        /// <returns> Request id. </returns>
        protected virtual string GetId(JToken jsonRequest)
        {
            return JsonHelper.SafeGetStringParam(Id, jsonRequest);
        }

        /// <summary>
        /// Gets service operation name
        /// </summary>
        /// <param name="jsonRequest"> Json token Request. </param>
        /// <returns> Operation name. </returns>
        protected virtual string GetOperation(JToken jsonRequest)
        {
            return GetMethod(jsonRequest).Split('.')[1];
        }

        /// <summary>
        /// Gets service name
        /// </summary>
        /// <param name="jsonRequest"> Json token Request. </param>
        /// <returns> Service name. </returns>
        protected virtual string GetServiceName(JToken jsonRequest)
        {
            return GetMethod(jsonRequest).Split('.')[0];
        }

        /// <summary>
        /// Creates single request item from JToken
        /// </summary>
        /// <param name="jsonRequest"> Json token Request. </param>
        /// <returns> Request item. </returns>
        protected virtual RequestItem CreateItem(JToken jsonRequest)
        {
            var requestItem = new RequestItem();
            try
            {
                // get service parameters
                requestItem.Id = GetId(jsonRequest) ?? string.Empty;
                requestItem.ServiceName = GetServiceName(jsonRequest);
                requestItem.Operation = GetOperation(jsonRequest);

                var type = _locator.Resolve(requestItem.ServiceName);
                var jParams = jsonRequest[Params];

                var jUserId = jParams["userId"];
                string userId;
                if (jUserId is JArray)
                {
                    var array = JsonHelper.SafeGetArrayParams("userId", jParams);
                    userId = array.First();
                }
                else
                {
                    userId = JsonHelper.SafeGetStringParam("userId", jParams);
                }

                requestItem.Params = new RequestParamsItem
                {
                    AppId = JsonHelper.SafeGetStringParam("appId", jParams), 
                    UserId = userId
                };
                requestItem.Entity = JsonConvert.DeserializeObject(jParams.ToString(), type);
            }
            catch (Exception ex)
            {
                // TODO: Write to trace
            }

            return requestItem;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets service method body
        /// </summary>
        /// <param name="jsonRequest"> Json token Request.  </param>
        /// <returns> Method name. </returns>
        private string GetMethod(JToken jsonRequest)
        {
            var method = jsonRequest[Method];
            return method.Value<string>();
        }

        #endregion
    }
}
