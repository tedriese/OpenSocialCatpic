// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISocialFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Creates social types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    /// <summary>
    /// Creates user's profile
    /// </summary>
    /// <typeparam name="T"> Person entity </typeparam>
    public interface ISocialFactory<T>
    {
        /// <summary>
        /// Creates profile of a given user
        /// </summary>
        /// <typeparam name="T"> Person entity </typeparam>
        /// <param name="person"> The person.  </param>
        /// <returns> Created person </returns>
        T CreateProfile(T person);
    }
}
