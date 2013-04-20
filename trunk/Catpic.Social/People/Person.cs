// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default OpenSocial Person entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social.People
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Default OpenSocial Person entity
    /// </summary>
    [DataContract]
    public class Person : IIdentityField
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        [DataMember(Name = "id")]
        public virtual string Id { get; set; }

        /// <summary>
        /// Gets or sets Name: The broken-out components and fully formatted version of the person's real name.
        /// </summary>
        [DataMember(Name = "name")]
        public Name Name { get; set; }

        /// <summary>
        /// Gets or sets Gender: The gender of this person. Service Providers SHOULD return one of the following Canonical Values, 
        /// if appropriate:male, female, or undisclosed, and MAY return a different value if it is not covered by one of these Canonical Values.
        /// </summary>
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets DisplayName: Required. The name of this Person, suitable for display to end-users. 
        /// Each Person returned MUST include a non-empty displayName value. The name SHOULD be the full name of the Person being described 
        /// if known (e.g. Cassandra Doll or Mrs. Cassandra Lynn Doll, Esq.), but MAY be a username or handle, if that is all that is available
        ///  (e.g. doll). The value provided SHOULD be the primary textual label by which this Person is normally displayed by the Service Provider 
        /// when presenting it to end-users.
        /// </summary>
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets Thumbnail: Person's photo thumbnail URL, specified as a string. This URL must be fully qualified. Relative URLs will not work in gadgets.
        /// </summary>
        [DataMember(Name = "thumbnailUrl")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets Birthday: The birthday of this person. The value MUST be a valid Date. The year value MAY be set to 0000 when the age of the Person is private or the year is not available.
        /// </summary>
        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// Gets or sets Nickname: The broken-out components and fully formatted, native-language version of the person's real name.
        /// </summary>
        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets AboutMe: 
        /// </summary>
        [DataMember(Name = "aboutMe")]
        public string AboutMe { get; set; }

        /// <summary>
        /// Gets or sets Age: The age of this person. Sometimes sites might want to show age without revealing the specific birthday.
        /// </summary>
        [DataMember(Name = "age")]
        public string Age { get; set; }

        /// <summary>
        /// Gets or sets URL of a web page relating to this Person. The value SHOULD be canonicalized by the Service Provider, e.g.http://josephsmarr.com/about/ instead of JOSEPHSMARR.COM/about/.
        /// In addition to the standard Canonical Values for type, this field also defines the additional Canonical Values blog and profile.
        /// </summary>
        [DataMember(Name = "urls")]
        public IEnumerable<string> Urls { get; set; }
    }

    /// <summary>
    /// The components of the person's real name. Providers MAY return just the full name as a single string in the formatted sub-field, 
    /// or they MAY return just the individual component fields using the other sub-fields, or they MAY return both.
    ///  If both variants are returned, they SHOULD be describing the same name, with the formatted name indicating how the component 
    /// fields should be combined.
    /// </summary>
    [DataContract]
    public class Name
    {
        /// <summary>
        /// Gets or sets GivenName: The given name of this Person, or "First Name" in most Western languages (e.g. Joseph given the full name Mr. Joseph Robert Smarr, Esq.).
        /// </summary>
        [DataMember(Name = "givenName")]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets FamilyName:
        /// </summary>
        [DataMember(Name = "familyName")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets Formatted: The full name, including all middle names, titles, and suffixes as appropriate, formatted for display (e.g. Mr. Joseph Robert Smarr, Esq.). This is the Primary Sub-Field for this field, for the purposes of sorting and filtering.
        /// </summary>
        [DataMember(Name = "formatted")]
        public string Formatted { get; set; }
    }
}