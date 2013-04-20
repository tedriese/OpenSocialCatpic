// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GadgetConsts.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines gadget consts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines gadget consts.
    /// </summary>
    public static class GadgetConsts
    {
        /// <summary>
        /// Container must set these befor json response NOTE defined in core.io feature (processResponse).
        /// </summary>
        public const string UnparseableCruft = "throw 1; < don't be evil' >";

        /// <summary>
        /// Anonymous user id.
        /// </summary>
        public const string AnonymousName = "john.doe";

        /// <summary>
        /// Name of token cache which is used by oauth functionality at least.
        /// </summary>
        public const string TokenCache = "tokens";

        #region regex

        /// <summary>
        /// Regex which is used for finding message patterns in view.
        /// </summary>
        public static readonly Regex MessageRegex = new Regex(@"__MSG_(\w*?)__", RegexOptions.Compiled);

        /// <summary>
        /// Regex which is used for finding module patterns in view.
        /// </summary>
        public static readonly Regex ModuleIdRegex = new Regex(@"__MODULE_ID__", RegexOptions.Compiled);
        
        #endregion
    }
}
