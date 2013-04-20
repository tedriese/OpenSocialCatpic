// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Gadget.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Default implementation of Gadget
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets
{
    using Catpic.Gadgets.Format;

    /// <summary>
    /// Default implementation of Gadget
    /// </summary>
    public class Gadget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gadget"/> class.
        /// </summary>
        /// <param name="definition"> Gadget definition. </param>
        /// <param name="context"> Gadget context. </param>
        public Gadget(GadgetDefinition definition, GadgetContext context)
        {
            this.Definition = definition;
            this.Context = context;
        }

        /// <summary>
        /// Gets gadget context.
        /// </summary>
        public GadgetContext Context { get; private set; }

        /// <summary>
        /// Gets gadget definition.
        /// </summary>
        public GadgetDefinition Definition { get; private set; }
    }
}
