// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncHelper.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the AsyncHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides async helper methods
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// For returning non-async stuff, using a TaskCompletionSource to avoid thread switches
        /// </summary>
        /// <returns>Async task</returns>
        public static Task GetEmptyTask()
        {
            return GetEmptyTask<object>(null);
        }

        /// <summary>
        /// For returning non-async stuff, using a TaskCompletionSource to avoid thread switches
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="result">Result of task</param>
        /// <returns>Async task</returns>
        public static Task<T> GetEmptyTask<T>(T result)
        {
            var emptyTask = new TaskCompletionSource<T>();
            emptyTask.SetResult(result);
            return emptyTask.Task;
        }

        /// <summary>
        /// Iterates the sequence of tasks
        /// </summary>
        /// <param name="asyncIterator">Task to be executed in series</param>
        /// <returns>Async task</returns>
        public static Task Iterate(IEnumerable<Task> asyncIterator)
        {
            if (asyncIterator == null)
            {
                throw new ArgumentNullException("asyncIterator");
            }

            var enumerator = asyncIterator.GetEnumerator();
            if (enumerator == null)
            {
                throw new InvalidOperationException("Invalid enumerable - GetEnumerator returned null");
            }

            var tcs = new TaskCompletionSource<object>();
            tcs.Task.ContinueWith(_ => enumerator.Dispose(), TaskContinuationOptions.ExecuteSynchronously);

            Action<Task> recursiveBody = null;
            recursiveBody = delegate
            {
                try
                {
                    if (enumerator.MoveNext())
                    {
                        enumerator.Current.ContinueWith(recursiveBody, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }
                }
                catch (Exception exc)
                {
                    tcs.TrySetException(exc);
                }
            };

            recursiveBody(null);
            return tcs.Task;
        }
    }
}
