// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBundle.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Processes localized message collection
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Catpic.Utils;
    using Catpic.Utils.Diagnostic;

    /// <summary>
    /// Processes localized message collection.
    /// </summary>
    public class MessageBundle : IDictionary<string, string>
    {
        /// <summary>
        /// Trace category.
        /// </summary>
        private const string TraceCategory = "message.bundle";

        /// <summary>
        /// Trace instance.
        /// </summary>
        private static readonly ITrace Trace = TraceFactory.GetTrace();

        /// <summary>
        /// Message source uri
        /// </summary>
        private readonly Uri _uri;

        /// <summary>
        /// Message mapping
        /// </summary>
        private readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Gets a value indicating whether message bundle is initialized.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBundle"/> class.
        /// </summary>
        /// <param name="messagesUri"> Messages uri. </param>
        public MessageBundle(Uri messagesUri)
        {
            _uri = messagesUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBundle"/> class.
        /// </summary>
        /// <param name="xMessages"> Xml node. </param>
        public MessageBundle(XElement xMessages)
        {
            Initialize(xMessages);
        }

        /// <summary>
        /// Initialzes message bundle
        /// </summary>
        /// <returns> Async task </returns>
        public Task InitializeAsync()
        {
            // just invoke callback
            if (this.IsReady)
            {
                return AsyncHelper.GetEmptyTask();
            }

            Trace.Debug(TraceCategory, string.Format("try to get messages from {0}", this._uri));
            return
                RemoteFetchHelper.GetFetchDataTask(this._uri, "GET", null, TaskCreationOptions.None).ContinueWith(
                    t =>
                        {
                            try
                    {
                        Trace.Debug(TraceCategory, string.Format("parse response {0}", _uri));
                        var response = t.Result;
                        using (var responseStream = response.GetResponseStream())
                        {
                            XDocument xDoc = XDocument.Load(new StreamReader(responseStream, Encoding.UTF8));
                            XElement xMessages = xDoc.Root;
                            Initialize(xMessages);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.Error(TraceCategory, "unable to initialize", ex);
                        throw;
                    }
                },
                    TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Initializes message bundle
        /// </summary>
        /// <param name="xMessages"> Messages in xml format. </param>
        private void Initialize(XElement xMessages)
        {
            this.ParseXml(xMessages);
            this.IsReady = true;
        }

        /// <summary>
        /// Parses serialized messages
        /// </summary>
        /// <param name="xMessages"> Xml element. </param>
        private void ParseXml(XElement xMessages)
        {
            // TODO parse messages
            foreach (var xMsg in xMessages.Elements("msg"))
            {
                var name = xMsg.Attribute("name").Value;
                var @value = xMsg.Value;
                this.Add(name, @value);
            }
        }

        public IDictionary<string, string> InnerDictionary
        {
            get { return _dictionary; }
        }

        #region IDictionary implementation

        public void Add(string key, string value)
        {
            if(!_dictionary.ContainsKey(key))
                _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                if (!IsReady)
                    throw new InvalidOperationException("Unable to use uninitialized MessageBundle");
                return _dictionary.Keys;
            }
        }

        public bool Remove(string key)
        {
            if (!IsReady)
                throw new InvalidOperationException("Unable to use uninitialized MessageBundle");
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            if (!IsReady)
                throw new InvalidOperationException("Unable to use uninitialized MessageBundle");
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get
            {
                if (!IsReady)
                    throw new InvalidOperationException("Unable to use uninitialized MessageBundle");
                return _dictionary.Values;
            }
        }

        public string this[string key]
        {
            get
            {
                if(!IsReady)
                    throw new InvalidOperationException("Unable to use uninitialized MessageBundle");
                return _dictionary[key];
            }
            set { _dictionary[key] = value; }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return _dictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_dictionary as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
