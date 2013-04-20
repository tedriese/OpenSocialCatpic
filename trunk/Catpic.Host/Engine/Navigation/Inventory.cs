using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catpic.Host.Engine.Navigation
{
    /// <summary>
    /// Represents navigation address for given ID
    /// </summary>
    [Serializable]
    public class Inventory
    {
        /// <summary>
        /// Inventory id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// MVC Area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// MVC Controller
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// MVC Action
        /// </summary>
        public string Action { get; set; }

        public Inventory(string id,  string area, string controller, string action)
        {
            Id = id;

            Area = area;
            Controller = controller;
            Action = action;
        }
    }
}