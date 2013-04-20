using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Provides navigation node root
    /// </summary>
    public interface INavigationProvider
    {
        /// <summary>
        /// Returns root of navigation
        /// </summary>
        /// <returns></returns>
        INavigationNode GetRoot();
    }
}