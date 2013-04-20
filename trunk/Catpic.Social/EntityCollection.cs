// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCollection.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents a collection of entities
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a collection of entities.
    /// </summary>
    /// <typeparam name="T"> Entity type. </typeparam>
    [DataContract]
    public class EntityCollection<T> where T : IIdentityField
    {
        /// <summary>
        /// Gets or sets internal Id.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets collection id.
        /// </summary>
        [DataMember(Name = "id")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets collection title.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Entities.
        /// </summary>
        public ICollection<T> Entities { get; set; }
    }
}
