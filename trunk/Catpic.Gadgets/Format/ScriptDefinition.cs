// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptDefinition.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Represents script
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Gadgets.Format
{
    using System;

    using Catpic.Utils;

    /// <summary>
    /// Script types
    /// </summary>
    public enum ScriptContentType
    {
        /// <summary>
        /// Inline script
        /// </summary>
        Inline,

        /// <summary>
        /// Remote script (scr)
        /// </summary>
        Remote,

        /// <summary>
        /// Script from resource
        /// </summary>
        Resource,

        /// <summary>
        /// Local scirpt
        /// </summary>
        Local
    }

    /// <summary>
    /// Represents script
    /// </summary>
    public class ScriptDefinition
    {
        /// <summary>
        /// Script content
        /// </summary>
        private string _content;

        /// <summary>
        /// Gets or sets Source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Content.
        /// </summary>
        public string Content
        {
            get
            {
                // content should be set by FeatureSet parser in case of inline script
                if (this._content != null || this.Type == ScriptContentType.Inline)
                {
                    return this._content;
                }

                // NOTE: try to get content of non-supported type
                if (this.Type == ScriptContentType.Resource)
                {
                    throw new NotImplementedException("Resource mode isn't supported yet");
                }

                return this._content = FileHelper.GetContent(this.Source);
            }

            set
            {
                this._content = value;
            }
        }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public ScriptContentType Type { get; set; }  
    }
}
