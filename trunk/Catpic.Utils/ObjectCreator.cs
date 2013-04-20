// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectCreator.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides the helper methods to create instances of different type using additional logic
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System;

    using Catpic.Utils.Configuration;

    /// <summary>
    /// Provides the helper methods to create instances of different type using additional logic
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// Creates the instance of type T and call IConfigurable.Configure if type is configurable
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="config">Config node for type</param>
        /// <param name="type">Xpath string which represents type name to create</param>
        /// <returns>New instance</returns>
        public static T CreateAndConfigure<T>(IConfigSection config, string type = "@type")
        {
            var instance = config.GetInstance<T>(type);
            if (instance is IConfigurable)
            {
                (instance as IConfigurable).Configure(config);
            }

            return instance;
        }

        /// <summary>
        /// Creates the instance, invokes action on the created instance and configure it if it is neсcessary
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="config">config node for type</param>
        /// <param name="action">Action which will be executed before Configure call</param>
        /// <param name="type">xpath string which represents type name to create</param>
        /// <returns>New instance</returns>
        public static T CreateAndConfigure<T>(IConfigSection config, Action<T> action, string type = "@type")
        {
            var instance = config.GetInstance<T>(type);
            action(instance);
            if (instance is IConfigurable)
            {
                (instance as IConfigurable).Configure(config);
            }

            return instance;
        }

        /// <summary>
        /// Creates the instance, invokes action on the created instance and configure it if it is neсcessary
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="config">config node for type</param>
        /// <param name="type">xpath string which represents type name to create</param>
        /// <param name="args">Arguments list</param>
        /// <returns>New instance</returns>
        public static T CreateAndConfigure<T>(IConfigSection config, string type = "@type", params object[] args)
        {
            var instance = config.GetInstance<T>(type, args);
            if (instance is IConfigurable)
            {
                (instance as IConfigurable).Configure(config);
            }

            return instance;
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="t">Type of instance</param>
        /// <param name="args">Arguments list</param>
        /// <returns>New instance</returns>
        public static object Create(Type t, params object[] args)
        {
            return Activator.CreateInstance(t, args);
        }

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <typeparam name="T">Type of instance</typeparam>
        /// <param name="args">Arguments list</param>
        /// <returns>New instance</returns>
        public static T Create<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }
    }
}
