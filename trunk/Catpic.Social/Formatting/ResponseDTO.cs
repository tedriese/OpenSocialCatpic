// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseDTO.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the ResultEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// General result entry
    /// </summary>
    [DataContract]
    public class ResultEntry
    {
        /// <summary>
        /// Gets or sets id of operation
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// Error result
    /// </summary>
    [DataContract]
    public class ErrorResult : ResultEntry
    {
        /// <summary>
        /// Gets or sets error details
        /// </summary>
        [DataMember(Name = "error")]
        public ErrorDetails Error { get; set; }
    }

    /// <summary>
    /// Error details
    /// </summary>
    [DataContract]
    public class ErrorDetails
    {
        /// <summary>
        /// Gets or sets erro code.
        /// </summary>
        [DataMember(Name = "code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets error message.
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }

    #region People

    /// <summary>
    /// Single result
    /// </summary>
    [DataContract]
    public class Record : ResultEntry
    {
        /// <summary>
        /// Gets or sets Result.
        /// </summary>
        [DataMember(Name = "result")]
        public object Result { get; set; }
    }

    /// <summary>
    /// Represents item collection result
    /// </summary>
    [DataContract]
    public class Collection : ResultEntry
    {
        /// <summary>
        /// Gets or sets Result.
        /// </summary>
        [DataMember(Name = "result")]
        public CollectionResultEntry Result { get; set; }
    }

    /// <summary>
    /// Many service operations return a list of OpenSocial resources. Lists are always returned in the "list" field of the result. 
    /// Lists can either be the full set of resources or a pageable subset. If the operation supports random access indexing of the full list
    ///  it will support the "startIndex" and "count" parameters which control what sublist of the full list is returned.
    ///  The paging mechanisms described here are based on the OpenSearch standard with the additional requirement that all indexes are 0 based.
    /// </summary>
    [DataContract]
    public class CollectionResultEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionResultEntry"/> class.
        /// </summary>
        public CollectionResultEntry()
        {
            this.IsFiltered = true;
            this.IsSorted = true;
            this.IsUpdatedSince = true;
        }

        /// <summary>
        /// Gets or sets StartIndex: the index of the first result returned in this response, relative to the starting index of all results
        ///  that would be returned if no startIndex had been requested. In general, this will be equal to the value requested by the startIndex, 
        /// or 0 if no specific startIndex was requested.
        /// </summary>
        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets ItemsPerPage: the number of results returned per page in this response. 
        /// In general, this will be equal to the count Query Parameter, but MAY be less if the Service Provider is unwilling to return as many
        /// results per page as requested, or if there are less than the requested number of results left to return when starting at the current startIndex. 
        /// This field MUST be present if and only if a value for count is specified in the request.
        /// </summary>
        [DataMember(Name = "itemsPerPage")]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets TotalResults: the total number of contacts that would be returned if there were no startIndex or count specified. 
        /// This value tells the Consumer how many total results to expect, regardless of the current pagination being used,
        ///  but taking into account the current filtering options in the request.
        /// </summary>
        [DataMember(Name = "totalResults")]
        public int TotalResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whethere the result honors filter params in the request. The default value is 'true' if the field does not exist.
        /// </summary>
        [DataMember(Name = "filtered")]
        public bool IsFiltered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whethere the items are sorted. The default value is 'true' if the field does not exist.
        /// </summary>
        [DataMember(Name = "sorted")]
        public bool IsSorted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whethere the result honors the updatedSince param in the request. The default value is 'true' if the field does not exist.
        /// </summary>
        [DataMember(Name = "updatedSince")]
        public bool IsUpdatedSince { get; set; }

        /// <summary>
        /// Gets or sets List: An array of objects, one for each item matching the request. 
        /// For consistency of parsing, if the request could possibly return multiple items (as is normally the case), 
        /// this value MUST always be an array of results, even if there happens to be 0 or 1 matching results. 
        /// (i.e. "entry": [ { /* first item */ }, { /* seond item */ } ]).
        /// NOTE: Current shindig's client side expects list instead of entry here
        /// </summary>
        [DataMember(Name = "list")]
        public object List { get; set; }
    }

    #endregion
}
