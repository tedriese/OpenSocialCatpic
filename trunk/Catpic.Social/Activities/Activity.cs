// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Activity.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default OpenSocial 1.x activity. An OpenSocial Activity represents a short summary or notification of a timestamped event,  often with pointers for more information.
//   NOTE: deprecated in OpenSocial 2.0
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Activities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///  Default OpenSocial 1.x activity
    /// </summary>
    [DataContract]
    public class Activity : IIdentityField
    {
        /// <summary>
        /// Gets or sets Id: activity id 
        /// </summary>
        [DataMember(Name = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets UserId: ID of the user who this activity is for.
        /// </summary>
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Body: Specifying an optional expanded version of an activity. 
        /// Bodies may only have the following HTML tags: b i, a, span. The container may ignore this formatting when rendering the activity.
        /// </summary>
        [DataMember(Name = "body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets StreamTitle: Specifing the title of the stream.
        /// </summary>
        [DataMember(Name = "streamTitle")]
        public string StreamTitle { get; set; }

        /// <summary>
        /// Gets or sets Title: Specifying the primary text of an activity. 
        /// Titles may only have the following HTML tags: b i, a, span. The container may ignore this formatting when rendering the activity.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets MediaItems: any photos, videos, or images that should be associated with the activity. Higher priority ones are higher in the list.
        /// </summary>
        [DataMember(Name = "mediaItems")]
        public IEnumerable<MediaItem> MediaItems { get; set; }
    }

    /// <summary>
    /// Represents images, movies, and audio.
    /// </summary>
    [DataContract]
    public class MediaItem
    {
        /// <summary>
        /// Gets or sets MimeType: The MIME type of media, specified as a string.
        /// </summary>
        [DataMember(Name = "mimeType")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets Type: Describing the media item.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets Url: Specifying the URL where the media can be found.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}