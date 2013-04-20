// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatpicPrincipal.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default implementation if catpic principal
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Web.Security
{
    using System.Security.Principal;

    using Catpic.Gadgets.Security;

    /// <summary>
    /// Default implementation if catpic principal
    /// </summary>
    public class CatpicPrincipal : GenericPrincipal, ICatpicPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatpicPrincipal"/> class.
        /// </summary>
        /// <param name="token"> Security token. </param>
        /// <param name="identity"> User identity. </param>
        /// <param name="roles"> User roles. </param>
        public CatpicPrincipal(ISecurityToken token, IIdentity identity, string[] roles)
            : base(identity, roles)
        {
            Token = token;
        }

        /// <summary>
        /// Gets security token.
        /// </summary>
        public ISecurityToken Token { get; private set; }
    }
}
