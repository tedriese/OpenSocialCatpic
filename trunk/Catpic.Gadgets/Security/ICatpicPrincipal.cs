// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICatpicPrincipal.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Binds open social user with .NET security user
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Security
{
    using System.Security.Principal;

    /// <summary>
    /// Binds open social user with .NET security user
    /// </summary>
    public interface ICatpicPrincipal : IPrincipal
    {
        /// <summary>
        /// Gets token which stores information about opensocial user and application
        /// </summary>
        ISecurityToken Token { get; }
    }
}
