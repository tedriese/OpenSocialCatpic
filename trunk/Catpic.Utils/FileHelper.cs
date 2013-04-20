// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileHelper.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the FileHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides file extension methods
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Resolves path on disk
        /// </summary>
        /// <param name="path"> Path to resolve. </param>
        /// <returns> Resolved path</returns>
        public static string ResolvePath(string path)
        {
            // TODO imporve approach of resolving/working with virtual path
            return path.StartsWith("~") || path.StartsWith("\\") ?
                System.Web.Hosting.HostingEnvironment.MapPath(path) : 
                path;
        }

        /// <summary>
        /// Gets content of the file
        /// </summary>
        /// <param name="path"> File  path. </param>
        /// <returns> Content of the file</returns>
        public static string GetContent(string path)
        {
            return File.ReadAllText(ResolvePath(path));
        }

        /// <summary>
        /// Gets fetch data task
        /// </summary>
        /// <param name="relativePath"> Relative path. </param>
        /// <param name="asyncState"> Async state. </param>
        /// <param name="taskOptions"> Async task options. </param>
        /// <returns> Async task</returns>
        public static Task<string> GetFetchDataTask(string relativePath, object asyncState, TaskCreationOptions taskOptions)
        {
            string content = string.Empty;
            try
            {
                var path = ResolvePath(relativePath);
                content = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
            }

            // NOTE just read file sync and return stub Task
            // TODO do it async
            return AsyncHelper.GetEmptyTask(content);
        }
    }
}
