// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the JsonHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides json extension methods
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Block comments regex
        /// </summary>
        private const string BlockComments = @"/\*(.*?)\*/";
        
        /// <summary>
        /// Line comments regex
        /// </summary>
        private const string LineComments = @"//(.*?)\r?\n";
        
        /// <summary>
        /// Strings regex
        /// </summary>
        private const string Strings = @"""((\\[^\n]|[^""\n])*)""";
        
        /// <summary>
        /// Verbatim strings regex
        /// </summary>
        private const string VerbatimStrings = @"@(""[^""]*"")+";

        /// <summary>
        /// Removes comment from json string
        /// </summary>
        /// <param name="json">Json string</param>
        /// <returns>Uncomment string</returns>
        public static string Uncomment(string json)
        {
            return Regex.Replace(
                json,
                BlockComments + "|" + LineComments + "|" + Strings + "|" + VerbatimStrings,
                me =>
                    {
                        if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                        {
                            return me.Value.StartsWith("//") ? Environment.NewLine : string.Empty;
                        }

                        // Keep the literal strings
                        return me.Value;
                    },
                    RegexOptions.Singleline);
        }

        /// <summary>
        /// Normalize string: removes new lines
        /// </summary>
        /// <param name="value"> Target string. </param>
        /// <returns> Normalized string</returns>
        public static string Normalize(string value)
        {
            return value.Trim('\n');
        }

        /// <summary>
        /// Gets value safely from JToken
        /// </summary>
        /// <param name="name"> Name of parameter. </param>
        /// <param name="parameters"> Json parameters. </param>
        /// <returns> String param. </returns>
        public static string SafeGetStringParam(string name, JToken parameters)
        {
            try
            {
                var param = parameters[name];
                if (param != null)
                {
                    if (param is JArray)
                    {
                        return param.Values<string>().First();
                    }

                    return param.Value<string>();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets values safely from JToken
        /// </summary>
        /// <param name="name"> Name of parameter. </param>
        /// <param name="parameters"> Json parameters. </param>
        /// <returns> List of string params. </returns>
        public static IEnumerable<string> SafeGetArrayParams(string name, JToken parameters)
        {
            try
            {
                var param = parameters[name];
                if (param != null)
                {
                    if (param is JArray)
                    {
                        return param.Values<string>();
                    }

                    return null;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets values safely from JToken
        /// </summary>
        /// <param name="name"> Name of parameter. </param>
        /// <param name="parameters"> Json parameters. </param>
        /// <param name="default"> Default value. </param>
        /// <returns> int value</returns>
        public static int SafeGetIntParam(string name, JToken parameters, int @default)
        {
            try
            {
                var param = parameters[name];
                if (param != null)
                {
                    if (param is JArray)
                    {
                        return param.Values<int>().First();
                    }

                    return param.Value<int>();
                }
            }
            catch
            {
            }

            return @default;
        }

        /// <summary>
        /// Gets dictionary from string content
        /// </summary>
        /// <param name="content"> String content. </param>
        /// <returns> Dictionary object</returns>
        public static IDictionary<string, string> GetDictionary(string content)
        {
            var dict = JsonConvert.DeserializeObject<IDictionary<string, string>>(content);

            return dict;
        }
    }
}
