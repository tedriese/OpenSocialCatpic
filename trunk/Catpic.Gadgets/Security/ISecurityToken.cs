// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecurityToken.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Security token
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    /// <summary>
    /// Security token
    /// </summary>
    public interface ISecurityToken
    {
        /// <summary>
        /// Gets owner of this gadget
        /// </summary>
        string OwnerId { get; }

        /// <summary>
        /// Gets viewer of this gadget
        /// </summary>
        string ViewerId { get; }

        /// <summary>
        /// Gets application id
        /// </summary>
        string AppId { get; }

        /// <summary>
        /// Gets url where the application lives
        /// </summary>
        string AppUrl { get; }

        /// <summary>
        /// Gets domain of the container 
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Gets name of containers
        /// </summary>
        string Container { get; }

        /// <summary>
        /// Gets module id of this gadget 
        /// </summary>
        string ModuleId { get; }

        /// <summary>
        /// Encrypts token to string
        /// </summary>
        /// <returns>Encrypted token</returns>
        string ToClientState();

        /// <summary>
        /// Restores internal state from string 
        /// </summary>
        /// <param name="state">Encrypted state</param>
        void FromClientState(string state);
    }
}
