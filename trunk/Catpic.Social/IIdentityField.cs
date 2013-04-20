// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityField.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Type which has identity property
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    /// <summary>
    /// Type which has identity property
    /// </summary>
    public interface IIdentityField
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        string Id { get; set; }
    }
}
