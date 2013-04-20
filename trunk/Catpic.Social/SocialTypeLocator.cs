// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialTypeLocator.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Resolves types associated with social services. Used by serializers to eliminate dependency on certain social entity (e.g. Person, Activity)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using System;
    using System.Collections.Generic;

    using Catpic.Social.Formatting;

    /// <summary>
    /// Resolves types associated with social services. Used by serializers to eliminate dependency on certain social entity (e.g. Person, Activity)
    /// </summary>
    public class SocialTypeLocator
    {
        /// <summary>
        /// Default type 
        /// </summary>
        private readonly Type _default;

        /// <summary>
        /// Maps string name to certain type
        /// </summary>
        private readonly IDictionary<string, Type> _registry = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialTypeLocator"/> class.
        /// </summary>
        public SocialTypeLocator()
        {
            this._default = typeof(CollectionItem);
        }

        /// <summary>
        /// Associates handler name with given type if it isn't registered
        /// </summary>
        /// <param name="name"> Name of type </param>
        /// <param name="type"> Type instance </param>
        /// <returns> Returns this (fluent interface) </returns>
        public SocialTypeLocator Register(string name, Type type)
        {
            if (!this._registry.ContainsKey(name))
            {
                this._registry.Add(name, type);
            }

            return this;
        }

        /// <summary>
        /// Returns type associated with given name
        /// </summary>
        /// <param name="name">Name of type</param>
        /// <returns>Type associated with the given name or default if there is no association</returns>
        public Type Resolve(string name)
        {
            Type type;
            this._registry.TryGetValue(name, out type);
            return type ?? this._default;
        }
    }
}
