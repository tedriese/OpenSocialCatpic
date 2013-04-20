/**
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * @fileoverview Params to include into the test suite execution.
 */


var Config = {};

// Tests to include.
Config.include = [];

// Test to exclude.
Config.exclude = ['broken', 'notImplementedYet'];

// Base url for the testing framework
Config.rootPath = 'http://opensocial-resources.googlecode.com/' +
                  'svn/tests/trunk/';
                  
Config.contentPath = 'http://qa.returnstrue.com/opensocial/stable/';

// Save current results to the user app data after the defined suite runs.
Config.saveGoldenResultsForSuite = '';
Config.goldenResultsLabel = 'original';

// Use data from qa instead of production.
Config.useQaData = false;

Config.defaultTimeout = 60000;

Config.domainToDataContructor = {};

Config.ready = false;

Config.baseUrl = null;

Config.securityToken = null;

if (typeof shindig != 'undefined' && shindig.auth.getSecurityToken) {
  Config.tokenPrefix = 'st';
  if (shindig.auth.getSecurityToken() != null) {
    Config.securityToken = Config.tokenPrefix + '='
        + shindig.auth.getSecurityToken();
  }
}

Config.saveUrls = function(baseUrl, securityToken) {
  Config.baseUrl = baseUrl;
  Config.securityToken = securityToken;
  if (Config.baseUrl != null && Config.securityToken != null) {
    Config.ready = true;
  }
}

/**
 * Sets the viewer depending on the opensocial version it's running
 */
if (gadgets && gadgets.util.hasFeature('opensocial-0.7')) {
  Config.VIEWER = opensocial.DataRequest.PersonId.VIEWER;
  Config.OWNER = opensocial.DataRequest.PersonId.OWNER;
} else if (gadgets && gadgets.util.hasFeature('opensocial-0.8')) {
  Config.VIEWER = opensocial.newIdSpec({userId: 'VIEWER'});
  Config.OWNER =  opensocial.newIdSpec({userId: 'OWNER'});
}
