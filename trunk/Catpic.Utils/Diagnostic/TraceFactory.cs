using System;
using System.Linq;
using System.Collections.Generic;
using Catpic.Utils;
using Catpic.Utils.Configuration;
using Catpic.Utils.Diagnostic;

namespace Catpic.Utils
{
    /// <summary>
    /// Tracer factory
    /// </summary>
    public static class TraceFactory
    {
        public const string Default = "default";
        private static readonly ITrace EmptyTrace = new EmptyTrace();
        private static Dictionary<string, ITrace> _traces = new Dictionary<string, ITrace>();

        public static void SetTrace(ITrace trace)
        {
            if (_traces.ContainsKey(Default))
                _traces[Default] = trace;
            else
                _traces.Add(Default, trace);
        }

        public static void Initialize(Dictionary<string, ITrace> traces)
        {
            _traces = traces;
        }

        public static bool IsInitialized
        {
            get { return _traces.Keys.Any(); }
        }

        /// <summary>
        /// Gets default tracer
        /// </summary>
        /// <returns></returns>
        public static ITrace GetTrace()
        {
            return GetTrace(Default);
        }

        /// <summary>
        /// Gets tracer associated with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITrace GetTrace(string name)
        {
            return !_traces.ContainsKey(name) ? EmptyTrace : _traces[name];
        }
    }
}
