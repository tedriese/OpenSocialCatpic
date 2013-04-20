// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilExtensions.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the UtilExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides various exstension methods
    /// </summary>
    public static class UtilExtensions
    {
        /// <summary>
        /// Gets value from dictionary
        /// </summary>
        /// <typeparam name="T"> Any type</typeparam>
        ///  <param name="dictionary"> Target dictionary </param>
        /// <param name="key"> Key to extract</param>
        /// <param name="default"> Default value </param>
        /// <returns> Extracted value. </returns>
        public static T SafeGet<T>(this IDictionary<string, object> dictionary, string key, T @default) where T : class
        {
            if (dictionary == null)
            {
                return @default;
            }

            T value = @default;
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key] as T;
            }

            return value;
        }
    }
}
