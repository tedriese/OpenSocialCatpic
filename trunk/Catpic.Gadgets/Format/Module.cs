// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Module.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents a module (gadget with moduleId?)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using System;

    /// <summary>
    /// Represents a module (gadget with moduleId?)
    /// </summary>
    public class Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        /// <param name="id"> Name of the module . </param>
        /// <param name="url"> Url of the module. </param>
        public Module(int id, Uri url)
        {
            this.Id = id;
            this.Uri = url;
        }

        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Uri.
        /// </summary>
        public Uri Uri { get; set; }
    }
}
