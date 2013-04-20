// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialConsts.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines social consts
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    /// <summary>
    /// Defines social consts.
    /// </summary>
    public static class SocialConsts
    {
        #region Groups

        /// <summary>
        /// Self key
        /// </summary>
        public const string GroupIdSelf = "@self";

        /// <summary>
        /// Friends key
        /// </summary>
        public const string GroupIdFriends = "@friends";

        /// <summary>
        /// All key
        /// </summary>
        public const string GroupIdAll = "@all";

        /// <summary>
        /// Group id
        /// </summary>
        public const string GroupIdGroupId = "@groupId";

        #endregion

        #region Users

        /// <summary>
        /// Viewer key
        /// </summary>
        public const string UserIdViewer = "@viewer";

        /// <summary>
        /// Owner key
        /// </summary>
        public const string UserIdOwner = "@owner";

        /// <summary>
        /// Me key
        /// </summary>
        public const string UserIdMe = "@me";

        /// <summary>
        /// User id
        /// </summary>
        public const string UserIdUserId = "@userId";

        #endregion

        #region Services

        /// <summary>
        /// Service get operation
        /// </summary>
        public const string Get = "get";

        /// <summary>
        /// Service create operation
        /// </summary>
        public const string Create = "create";

        /// <summary>
        /// Service update operation
        /// </summary>
        public const string Update = "update";

        /// <summary>
        /// Service delete operation
        /// </summary>
        public const string Delete = "delete";

        #endregion

        #region Messaging

        /// <summary>
        /// Message service name
        /// </summary>
        public const string MessageServiceName = "messages";

        /// <summary>
        /// An email.
        /// @member opensocial.Message.Type
        /// </summary>
        public const string EmailMessage = "email";

        /// <summary>
        /// A short private message.
        ///  @member opensocial.Message.Type
        /// </summary>
        public const string NotificationMessageType = "notification";

        /// <summary>
        /// A message to a specific user that can be seen only by that user.
        /// @member opensocial.Message.Type
        /// </summary>
        public const string PrivateMessageType = "privateMessage";

        /// <summary>
        /// A message to a specific user that can be seen by more than that user.
        /// @member opensocial.Message.Type
        /// </summary>
        public const string PublicMessageType = "publicMessage";

        /// <summary>
        /// A new, unread message
        /// @member opensocial.Message.Status
        /// </summary>
        public const string NewMessageStatus = "new";

        /// <summary>
        /// A deleted message
        /// @member opensocial.Message.Status
        /// </summary>
        public const string DeletedMessageStatus = "deleted";

        /// <summary>
        /// A flagged message
        /// @member opensocial.Message.Status
        /// </summary>
        public const string FlaggedMessageStatus = "flagged";

        #endregion

    }
}
