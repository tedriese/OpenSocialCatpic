using System;
using System.Collections.Generic;
using Catpic.Utils;
using Catpic.Utils.Caching;
using Catpic.Utils.Configuration;

namespace Catpic.Host.Engine.Caching
{
    /// <summary>
    /// Cache factory
    /// </summary>
    public static class CacheFactory
    {
        public const string Default = "default";
        private static readonly Dictionary<string, ICache> _caches = new Dictionary<string, ICache>();
        private static readonly object _lockInstance = new object();
        public static bool IsInitialized { get; private set; }


        static void Initialize()
        {
            try
            {
                //get traces
                //NOTE: dependency on configuration!!!
                var cacheConfigs = ConfigSettings.Instance.GetSections("utils/caching/cache");
                foreach (var cacheConfig in cacheConfigs)
                {
                    string name = cacheConfig.GetString("@name");
                    ICache trace = null;
                    try
                    {
                        trace = ObjectCreator.CreateAndConfigure<ICache>(cacheConfig);
                    }
                    catch
                    {
                        //NOTE unable to create trace
                        trace = new RuntimeMemoryCache();
                    }
                    _caches.Add(name, trace);
                }
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Fatal error: unable to register cache subsystem", ex);
            }
        }

        static void Ensure()
        {
            if (!IsInitialized)
                lock (_lockInstance)
                {
                    if (!IsInitialized)
                        Initialize();
                }
        }

        /// <summary>
        /// Gets default cache
        /// </summary>
        /// <returns></returns>
        public static ICache GetCache()
        {
            return GetCache(Default);
        }

        /// <summary>
        /// Gets cache associated with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ICache GetCache(string name)
        {
            Ensure();
            return _caches[name];
        }
    }
}
