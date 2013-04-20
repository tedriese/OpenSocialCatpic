// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMemoryCache.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides caching functionality which uses FCL built-in caching classes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils.Caching
{
    using System.Runtime.Caching;

    /// <summary>
    /// Provides caching functionality which uses FCL built-in caching classes
    /// </summary>
    public class RuntimeMemoryCache : ICache
    {
        /// <summary>
        /// Synchronization object
        /// </summary>
        private readonly object _syncLock = new object();

        /// <summary>
        /// Built-in .NET memory cache.
        /// </summary>
        private readonly MemoryCache _cache = MemoryCache.Default;

        /// <summary>
        /// Cache policy.
        /// </summary>
        private readonly CacheItemPolicy _policy = new CacheItemPolicy();

        /// <summary>
        /// Adds value to cache. Thread-safe
        /// </summary>
        /// <param name="key"> Cache key. </param>
        /// <param name="value"> Cache value. </param>
        public void Add(object key, object value)
        {
            // TODO investigate: is it really needed here?
            if (!this._cache.Contains(key.ToString()))
            {
                lock (this._syncLock)
                {
                    if (!this._cache.Contains(key.ToString()))
                    {
                        this._cache.Add(new CacheItem(key.ToString(), value), this._policy);
                    }
                }
            }
        }

        /// <summary>
        /// Adds value to cache. Thread-safe
        /// </summary>
        /// <param name="key"> Cache key. </param>
        /// <param name="value"> Cache value. </param>
        /// <param name="metadata"> Metadata associated with the cahe entry. </param>
        public void Add(object key, object value, Metadata metadata)
        {
            // TODO convert metadata to policy
            this.Add(key, value);
        }

        /// <summary>
        /// Gets value from cache
        /// </summary>
        /// <param name="key"> Cache key. </param>
        /// <returns> Cached value</returns>
        public object Get(object key)
        {
            var item = this._cache.GetCacheItem(key.ToString());
            if (item != null)
            {
                return item.Value;
            }

            return null;
        }

        /// <summary>
        /// Removes value from cache
        /// </summary>
        /// <param name="key"> Cache key. </param>
        public void Remove(object key)
        {
            this._cache.Remove(key.ToString());
        }

        /// <summary>
        /// Checks whether the value is present in cache
        /// </summary>
        /// <param name="key"> Cache key. </param>
        /// <returns> true if value exists in cache</returns>
        public bool Contains(object key)
        {
            return this._cache.Contains(key.ToString());
        }
    }
}
