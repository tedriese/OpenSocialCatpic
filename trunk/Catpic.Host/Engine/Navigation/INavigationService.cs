using System.Collections.Generic;
using System.Linq;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Represents navigation service which helps to build menu
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Returns inventory id by path
        /// </summary>
        string GetInventoryId(string path);

        /// <summary>
        /// Returns navigation root
        /// </summary>
        /// <returns></returns>
        INavigationNode GetNavigation();

        /// <summary>
        /// Returns navigation by inventory
        /// </summary>
        INavigationNode GetNavigation(string inventory);


    }
}