// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Group.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents opensocial group
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Groups
{
    using System.Runtime.Serialization;

    /// <summary>
    /// OpenSocial Groups are owned by people, and are used to tag or categorize people and their relationships. 
    /// Each group has a display name, an identifier which is unique within the groups owned by that person, and a 
    /// URI link. A group may be a private, invitation-only, public or a personal group used to organize friends.
    /// </summary>
    [DataContract]
    public class Group : IIdentityField
    {
        /// <summary>
        /// Gets or sets Id: unique ID for this group (Required).
        /// </summary>
        [DataMember(Name = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets Title: title of group (Required).
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Description: description of group (Optional).
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
