// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelper.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Provides reflection helpers for manipulating properties
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Returns property info for type using datamember's name
        /// </summary>
        public static PropertyInfo GetPropertyByContractName(string contractName, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                DataMemberAttribute[] attributes =
                    property.GetCustomAttributes(typeof(DataMemberAttribute), false) as DataMemberAttribute[];
                if (attributes.Any(a => a.Name == contractName))
                    return property;
            }
            return null;
        }

        public static string GetPropertyValue<T>(string contractName, T entity)
        {
            var properties = typeof(T).GetProperties();
            var property = GetPropertyByContractName(contractName, properties);

            return (string)property.GetValue(entity, null);
        }

        /// <summary>
        /// Copies not null properties from one object to another
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="destination"> The destination. </param>
        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        var value = sourceProperty.GetValue(source, new object[] { });
                        if (value != null)
                        {
                            destProperty.SetValue(destination, value, new object[] { });
                        }
                        break;
                    }
                }
            }
        }
    }
}
