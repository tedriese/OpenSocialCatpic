//TwitPic/OAuth.cs
//
// Code to do OAuth stuff, in support of a cropper plugin that sends
// a screen snap to TwitPic.com.
//
// There's one main class: OAuth.Manager.  It handles interaction with the OAuth-
// enabled service, for requesting temporary tokens (aka request tokens), as well
// as access tokens. It also provides a convenient way to construct an oauth
// Authorization header for use in any Http transaction.
//
// The code has been tested with Twitter and TwitPic, from a desktop application.
//
// -------------------------------------------------------
// Dino Chiesa
// Tue, 14 Dec 2010  12:31
//
// -------------------------------------------------------
// Last saved: <2011-April-20 15:59:04>

namespace Catpic.Utils.OAuth
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   A class to hold an OAuth response message.
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        ///   All of the text in the response. This is useful if the app wants
        ///   to do its own parsing.
        /// </summary>
        public string AllText { get; set; }
        private Dictionary<String, String> _params;

        /// <summary>
        ///   a Dictionary of response parameters.
        /// </summary>
        public string this[string ix]
        {
            get
            {
                return _params[ix];
            }
        }


        public OAuthResponse(string alltext)
        {
            AllText = alltext;
            _params = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach (var pair in kvpairs)
            {
                var kv = pair.Split('=');
                _params.Add(kv[0], kv[1]);
            }
            // expected keys:
            //   oauth_token, oauth_token_secret, user_id, screen_name, etc
        }
    }
}
