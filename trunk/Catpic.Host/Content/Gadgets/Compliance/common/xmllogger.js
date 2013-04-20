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
 * @fileoverview Logger class that takes the ResultSet and generate
 * XML document.
 */
var XmlLoggerUtils = {
  /*
   * Results used to generate XML
   */
  resultSets_ : [],

  /*
   * Container information of tests on which it was
   */
  container : null,

  /**
   * Context information for the test. i.e. viewer and owner info.
   */
  context : null
};

/**
 * Stores the resultSet in this static context.
 *
 * @param {object} resultSet sets the array with the resultSet.
 */
XmlLoggerUtils.setResultSet = function(resultSet) {
  XmlLoggerUtils.resultSets_ = resultSet;
};

/**
 * Gets the resultSet stored in the static context.
 *
 * @return {Array.<object>} An array with the resultSet inside.
 */
XmlLoggerUtils.getResultSet = function() {
  return XmlLoggerUtils.resultSets_;
};

/**
 * Stores the container information in this static context.
 *
 * @param {String} container on which tests were run.
 */
XmlLoggerUtils.setContainer = function(container) {
  XmlLoggerUtils.container = container;
};

/**
 * Stores the context information for xml logger
 */
XmlLoggerUtils.setContext = function(context) {
  XmlLoggerUtils.context = context;
}

/**
 *
 * @param {Array.<object>} results The results to be displayed.
 */
XmlLoggerUtils.generateXML = function() {
  var doc = document.implementation.createDocument("", "", null);
  var testRun = doc.createElement("TestRun");
  testRun.setAttribute("name", this.resultSets_.name);
  testRun.setAttribute("id", this.resultSets_.id);
  testRun.setAttribute("container", this.container);
  testRun.setAttribute("gadgetUrl", gadgets.util.getUrlParameters()['url']);
  testRun.setAttribute("timeStamp",  new Date().toUTCString());
  testRun.setAttribute("viewerId", this.context.getViewer().getId());
  testRun.setAttribute("viewerName", this.context.getViewer().getDisplayName());
  testRun.setAttribute("selfId", this.context.getOwner().getId());
  testRun.setAttribute("ownerName", this.context.getOwner().getDisplayName());

  for (var i = 0; i < this.resultSets_.length; i++) {
    var suite = this.resultSets_[i];
    var suiteElem = doc.createElement("Suite");

    suiteElem.setAttribute("name", suite.name);
    suiteElem.setAttribute("id", suite.id);
    suiteElem.setAttribute("time",
        suite.finishtime.toString() - suite.starttime.toString());
    for (var j = 0; j < suite.results.length; j++) {
      var result = suite.results[j];
      var resultElem = doc.createElement("Test");
      resultElem.setAttribute("name", result.name);
      resultElem.setAttribute("id", result.id);
      if (result.bugs != undefined) {
        resultElem.setAttribute("bug", result.bugs.join(', '));
      }
      resultElem.setAttribute(
          "status", Result.severity.getString(result.severity));
      resultElem.setAttribute("priority",
          result.test.priority ? result.test.priority : Test.PRIORITY.P2);
      suiteElem.appendChild(resultElem);
    }
    testRun.appendChild(suiteElem);
  }
  doc.appendChild(testRun);

  var xmlWindow =
      window.open('','name','height=400,width=500,scrollbars=1,toolbar=1');
  var xmlString = new XMLSerializer().serializeToString(doc);
  xmlWindow.document.open('content-type: text/xml');

  xmlWindow.document.write('<?xml version="1.0" encoding="UTF-8"?>' + xmlString);
  xmlWindow.document.close();
};
