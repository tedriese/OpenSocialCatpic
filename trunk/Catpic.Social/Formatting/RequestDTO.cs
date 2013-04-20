// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestDTO.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Contains request DTO object definitions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains main request parameters
    /// </summary>
    [DataContract]
    public class RequestParamsItem
    {
        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets application id
        /// </summary>
        [DataMember(Name = "appId")]
        public string AppId { get; set; }
    }

    /// <summary>
    /// Wraps Person
    /// </summary>
    /// <typeparam name="T"> Person entity </typeparam>
    [DataContract]
    public class PersonItem<T> : CollectionItem
    {
        /// <summary>
        /// Gets or sets user id 
        /// </summary>
        [DataMember(Name = "itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets message collection id
        /// </summary>
        [DataMember(Name = "groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets Person.
        /// </summary>
        [DataMember(Name = "person")]
        public T Person { get; set; }
    }

    /// <summary>
    /// Wraps Activity 
    /// </summary>
    /// <typeparam name="T"> Activity entity </typeparam>
    [DataContract]
    public class ActivityItem<T> : CollectionItem
    {
        /// <summary>
        /// Gets or sets user id 
        /// </summary>
        [DataMember(Name = "activityId")]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets message collection id
        /// </summary>
        [DataMember(Name = "groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets Activity.
        /// </summary>
        [DataMember(Name = "activity")]
        public T Activity { get; set; }
    }

    /// <summary>
    /// Wraps Message: used for create and get methods
    /// </summary>
    /// <typeparam name="T"> Message entity. </typeparam>
    [DataContract]
    public class MessageItem<T> : CollectionItem
    {
        /// <summary>
        /// Gets or sets message id
        /// </summary>
        [DataMember(Name = "messageId")]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets message collection id
        /// </summary>
        [DataMember(Name = "messageCollectionId")]
        public string MessageCollectionId { get; set; }

        /// <summary>
        /// Gets or sets message collection id
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        [DataMember(Name = "message")]
        public T Message { get; set; }
    }

    /// <summary>
    /// Wraps Group 
    /// </summary>
    /// <typeparam name="T"> Group entity </typeparam>
    [DataContract]
    public class GroupItem<T> : CollectionItem
    {
        /// <summary>
        /// Gets or sets user id 
        /// </summary>
        [DataMember(Name = "itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets message collection id
        /// </summary>
        [DataMember(Name = "groupId")]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets Group.
        /// </summary>
        [DataMember(Name = "group")]
        public T Group { get; set; }
    }

    /// <summary>
    /// Wraps collection request
    /// </summary>
    [DataContract]
    public class CollectionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItem"/> class.
        /// </summary>
        public CollectionItem()
        {
            this.Count = 25;
        }

        #region Collection

        /// <summary>
        /// Gets or sets Count: the page size for a paged collection. If no parameter is specified the container can choose 
        /// how many items in the collection should be returned
        /// </summary>
        [DataMember(Name = "count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets StartIndex: index into a paged collection
        /// </summary>
        [DataMember(Name = "startIndex")]
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets FilterBy: for a collection, return entries filtered by the given field name.
        /// </summary>
        [DataMember(Name = "filterBy")]
        public string FilterBy { get; set; }

        /// <summary>
        /// Gets or sets FilterOp: the operation to use when filtering a collection by a field specified in 'filterBy', defaults to "contains".
        /// Valid values are 'contains', 'equals', 'startsWith', and 'present'.
        /// </summary>
        [DataMember(Name = "filterOp")]
        public string FilterOp { get; set; }

        /// <summary>
        /// Gets or sets FilterOp: the value to use when filtering a collection. For example, { ... filterBy : name, filterOp : startsWith, filterValue : "John" ...}
        /// return all items whose name field starts with John. Johnny and John Doe would both be included.)
        /// </summary>
        [DataMember(Name = "filterValue")]
        public string FilterValue { get; set; }

        /// <summary>
        /// Gets or sets SortBy: sort field
        /// </summary>
        [DataMember(Name = "sortBy")]
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets SortOrder: can either be 'ascending' or 'descending', defaults to ascending. Used to sort objects in a collection.
        /// </summary>
        [DataMember(Name = "sortOrder")]
        public string SortOrder { get; set; }

        /// <summary>
        /// Gets or sets Fields: an array of field names to include in the representation or in the members of a collection. 
        /// If no fields are specified in the request it is up to the container to decide which fields to return, 
        /// however, the response MUST always include a minimum set of fields. For people this is [id, name, thumbnailUrl]. 
        /// For activities this is [id, title]. In place of an array '@all' is accepted to indicate returning all available fields.
        /// </summary>
        [DataMember(Name = "fields")]
        public string[] Fields { get; set; }

        #endregion
    }
}
