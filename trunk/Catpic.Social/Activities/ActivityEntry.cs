// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityEntry.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents social activity according to http://activitystrea.ms/head/json-activity.html
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Activities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents social activity according to http://activitystrea.ms/head/json-activity.html
    /// </summary>
    [DataContract]
    public class ActivityEntry : IIdentityField
    {
        /// <summary>
        /// Gets or sets UserId. NOTE: Used for internal purposes, not specification-compliant
        /// </summary>
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Id of activity
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Title: Natural-language title or headline for the activity encoded as a single JSON String containing HTML markup. 
        /// An activity MAY contain a title property.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Published: the date and time at which the activity was published. An activity MUST contain a published property.
        /// </summary>
        [DataMember(Name = "published")]
        public DateTime Published { get; set; }

        /// <summary>
        /// Gets or sets Actor: describes the entity that performed the activity. An activity MUST contain one actor property whose value is a single ActivityObject
        /// </summary>
        [DataMember(Name = "actor")]
        public ActivityObject Actor { get; set; }

        /// <summary>
        /// Gets or sets Verb: identifies the action that the activity describes. 
        /// An activity SHOULD contain a verb property whose value is a JSON String that is non-empty and matches either the "isegment-nz-nc" or 
        /// the "IRI" production in RFC3339. Note that the use of a relative reference other than a simple name is not allowed. 
        /// If the verb is not specified, or if the value is null, the verb is assumed to be "post".
        /// </summary>
        [DataMember(Name = "verb")]
        public string Verb { get; set; }

        /// <summary>
        /// Gets or sets ObjectEntry: Describes the primary object of the activity. For instance, in the activity,
        /// "John saved a movie to his wishlist", the object of the activity is "movie". 
        /// An activity SHOULD contain an object property whose value is a single Object. If the object property is not contained, the primary object of the activity MAY be implied by context.
        /// </summary>
        [DataMember(Name = "object")]
        public ActivityObject ObjectEntry { get; set; }

        /// <summary>
        /// Gets or sets TargetEntry: Describes the target of the activity. The precise meaning of the activity's target is dependent on the activities verb,
        ///  but will often be the object the English preposition "to". For instance, in the activity, "John saved a movie to his wishlist", the target of the activity is "wishlist". 
        /// The activity target MUST NOT be used to identity an indirect object that is not a target of the activity. An activity MAY contain a target property whose value is a single ActivityObject.
        /// </summary>
        [DataMember(Name = "target")]
        public ActivityObject TargetEntry { get; set; }

        // TODO opensocial section
    }

    /// <summary>
    /// An object is a thing, real or imaginary, which participates in an activity. It may be the entity performing the activity,
    /// or the entity on which the activity was performed. Because Activity Streams are often used in the context of a social platform, 
    /// OpenSocial adds an additional field to the data model, "deliverTo:". Because these are extensions, they are contained in an eclosing "namespace", org.opensocial.
    /// </summary>
    [DataContract]
    public class ActivityObject
    {
        /// <summary>
        /// Gets or sets Id which provides a permanent, universally unique identifier for the object in the form of an absolute IRI [RFC3987]. 
        /// An object SHOULD contain a single id property. If an object does not contain an id property, consumers MAY use the value of the url property as a less-reliable, non-unique identifier.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets DisplayName: a natural-language, human-readable and plain-text name for the object. HTML markup MUST NOT be included. An object MAY contain a displayName property.
        /// If the object does not specify an objectType property, the object SHOULD specify a displayName.
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Url: an IRI [RFC3987] identifying a resource providing an HTML representation of the object. An object MAY contain a url property
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets ObjectType which identifies the type of object. An object MAY contain an objectType property whose value is a JSON String
        /// that is non-empty and matches either the "isegment-nz-nc" or the "IRI" production in [RFC3987]. 
        /// Note that the use of a relative reference other than a simple name is not allowed. 
        /// If no objectType property is contained, the object has no specific type.
        /// </summary>
        [DataMember(Name = "objectType")]
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets Image: description of a resource providing a visual representation of the object, intended for human consumption. 
        /// An object MAY contain an image property whose value is aMediaLink.
        /// </summary>
        [DataMember(Name = "image")]
        public MediaLink Image { get; set; }
    }

    /// <summary>
    /// Some types of objects may have an alternative visual representation in the form of an image, video or embedded HTML fragments. A Media Link represents a hyperlink to such resources.
    /// </summary>
    [DataContract]
    public class MediaLink
    {
        /// <summary>
        /// Gets or sets Url: The IRI of the media resource being linked. A media link MUST have a url property. 
        /// OpenSocial note: Many OpenSocial containers currently use Media Items as defined by this specification.
        /// When a container creates a Media Link that is based on a Media Item, the Media Link URL MUST match the URL of the Media Item.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets Width: a hint to the consumer about the width, in pixels, of the media resource identified by the url property.
        /// A media link MAY contain a width property when the target resource is a visual media item such as an image, video or embeddable HTML page.
        /// </summary>
        [DataMember(Name = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets Height: a hint to the consumer about the height, in pixels, of the media resource identified by the url property. 
        /// A media link MAY contain a height property when the target resource is a visual media item such as an image, video or embeddable HTML page.
        /// </summary>
        [DataMember(Name = "height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets Duration: A hint to the consumer about the length, in seconds, of the media resource identified by the url property. 
        /// A media link MAY contain a "duration" property when the target resource is a time-based media item such as an audio or video.
        /// </summary>
        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets MediaItemId. Optional. Provides a mapping from an Activity Streams MediaLink to an OpenSocialMediaItem. Identifies the corresponding MediaItem that this MediaLink maps to, if any. 
        /// This field is namespaced as an "openSocial" extension.
        /// </summary>
        [DataMember(Name = "mediaItemId")]
        public int MediaItemId { get; set; }
    }
}
