using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Provides navigation for pages
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly INavigationProvider _provider;
        public NavigationService(INavigationProvider provider)
        {
            _provider = provider;
        }

        #region Implementation of INavigationService

        /// <summary>
        /// Returns inventory id by path
        /// </summary>
        public string GetInventoryId(string path)
        {
            if (path.StartsWith(@"/"))
                path = path.Substring(1);

            //TODO remove query string

            string[] parts = path.Split('/');

            //attach default action
            if (parts.Length < 3)
            {
                if(parts.Length<2)
                {
                    // path is empty: use default inventory of root
                    var root = _provider.GetRoot();
                    return root.Inventory.Id;
                }
                return String.Format("{0}/{1}/Index", parts[0], parts[1]);
            }

            return path;
        }

        public INavigationNode GetNavigation()
        {
            return _provider.GetRoot();
        }

        /// <summary>
        /// Returns navigation by path
        /// </summary>
        public INavigationNode GetNavigation(string inventoryId)
        {
            var root = _provider.GetRoot();
            return Traverse(inventoryId.ToUpperInvariant(), Clone(root));
        }

        #endregion

        #region static members

        /// <summary>
        /// Finds node in tree by inventory, clones it and set its current property to true
        /// </summary>
        private static INavigationNode Traverse(string inventory, INavigationNode node)
        {
            foreach (var navigationNode in node.Children)
            {
                //NOTE: inventory is case insensitive
                if (navigationNode.Inventory !=null && navigationNode.Inventory.Id.ToUpperInvariant() == inventory)
                {
                    node.IsCurrent = true;
                    navigationNode.IsCurrent = true;
                    return node;
                }

                //TODO: replace with depth or breadth first search
                var result = Traverse(inventory, navigationNode);
                if (result != null)
                {
                    node.IsCurrent = true;
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Clones navigation node to prevent concurrence issues
        /// </summary>
        private static INavigationNode Clone(INavigationNode node)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, node);
                stream.Seek(0, SeekOrigin.Begin);
                return (INavigationNode)formatter.Deserialize(stream);
            }
        }

        #endregion
    }
}