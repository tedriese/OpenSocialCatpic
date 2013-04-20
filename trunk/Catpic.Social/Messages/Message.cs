// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the Message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents message
    /// </summary>
    [DataContract(Namespace = "http://ns.opensocial.org/2008/opensocial")]
    public class Message : IIdentityField
    {
        /// <summary>
        /// Gets or sets Id:  unique ID for this message.
        /// </summary>
        [DataMember(Name = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets Type: the type of the message.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets Body: the main text of the message. HTML attributes are allowed and are sanitized by the container.
        /// </summary>
        [DataMember(Name = "body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets BodyId: the main text of the message as a message template. Specifies the message ID to use in the gadget xml.
        /// </summary>
        [DataMember(Name = "bodyId")]
        public string BodyId { get; set; }

        /// <summary>
        /// Gets or sets Title: the title of the message. HTML attributes are allowed and are sanitized by the container.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets TitleId: the title of the message as a message template. Specifies the message ID to use in the gadget xml.
        /// </summary>
        [DataMember(Name = "titleId")]
        public string TitleId { get; set; }

        /// <summary>
        /// Gets or sets Recipients: Array of person IDs.
        /// </summary>
        [DataMember(Name = "recipients")]
        public string[] Recipients { get; set; }

        /// <summary>
        /// Gets or sets SenderId: Id of person who sent the message.
        /// </summary>
        [DataMember(Name = "senderId")]
        public string SenderId { get; set; }

        /// <summary>
        /// Gets or sets TimeSent: UTC time message was sent.
        /// </summary>
        [DataMember(Name = "timeSent")]
        public string TimeSent { get; set; }

        /// <summary>
        /// Gets or sets InReplyTo: message ID, use for threaded comments/messages. Reference the sematics of the Atom Threading model defined in rfc4685. URLs should be mapped to Atom 'link rel="type" '
        /// </summary>
        [DataMember(Name = "inReplyTo")]
        public string InReplyTo { get; set; }

        /// <summary>
        /// Gets or sets Replies: array of message ids. Reference the sematics of the Atom Threading model defined in rfc4685. URLs should be mapped to Atom 'link rel="type"'
        /// </summary>
        [DataMember(Name = "replies")]
        public IEnumerable<string> Replies { get; set; }

        /// <summary>
        /// Gets or sets Status: status of the message. (NEW, READ, DELETED).
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets AppUrl which identifies the application that generated this message.
        /// </summary>
        [DataMember(Name = "appUrl")]
        public string AppUrl { get; set; }

        /// <summary>
        /// Gets or sets CollectionIds which identifies the messages collection IDs this message is contained in.
        /// </summary>
        [DataMember(Name = "collectionIds")]
        public string[] CollectionIds { get; set; }

        /// <summary>
        /// Gets or sets Updated: last update for this message.
        /// </summary>
        [DataMember(Name = "updated")]
        public string Updated { get; set; }

        /// <summary>
        /// Gets or sets Urls: List of related URLs for this message. Supported URL types include 'alternate', alternate for for this mailbox (text/html being the most common).
        /// </summary>
        [DataMember(Name = "urls")]
        public string[] Urls { get; set; }
    }
}
