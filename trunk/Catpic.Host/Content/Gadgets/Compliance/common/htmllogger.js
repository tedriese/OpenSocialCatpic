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
 * @fileoverview Logger class that takes the results array, and outputs each
 * of the result in HTML format. Uses CSS styles defined in test.css.<br>
 * This class renders two tables primarily. The first table that is rendered is
 * a progress table where tests are listed and as they finish the table is
 * updated to reflect the status of the tests, the second table rendered are the
 * actual results of the tests with all its internal assertions and
 * sub-results.<br>
 * Both tables look alike the only difference is that they use different styles
 * to display, as mentioned above styles are taken from the tests.css file.<br>
 * <b>The first table: Progress report</b><br>
 * The first table displays the header and all the id's of the tests along with
 * an area for the tests to report it's status.<br>
 * This table uses little styles but still look a lot alike than its complete
 * counterpart (see below).<br>
 * The tests usually reports a change in its status by using callbacks that will
 * look for specific div elements using its Id's thus allowing change in the
 * contents of the DIVs.<br>
 * <b>The second table: Results report</b><br>
 * This table looks similar to the first one with notable differences.
 * One and maybe the most important is that it has all the result information
 * for each tests, this information is displayed as tables that has more data
 * than the progress table.<br>
 * The second difference is the use of styles to display this table, in fact an
 * effective way to know if this table is displayed and not it's counterpart is
 * to look for the 'header' style class since when this style is present in the
 * rendered div it means that the results have been rendered.<br>
 * The information rendered by this logger varies from result to result,
 * successful tests only displays the id, name and actual result, while
 * failures and warnings displays the id, name, actual and expected results; if
 * there is an exception the result is marked as failed and also the exception
 * is logged in a special format that allows better readability of them.
 * Information results (log messages) will display their id and the data
 * logged.<br>
 * Also for each test a mini-summary is presented, this contains the outcomes of
 * the sub-results.<br>
 * At the top level there is a summary along with links to the failed tests.
 */

/**
 * Creates a new HtmlLogger to render tests results as HTML in a given element.
 * @param {element} outputDiv The element where the output will be logged.
 * @param {string} headerMessage A message to be printed in the header of the
 *     test area.
 * @param {TestSuiteDirectory} testSuiteDirectory to be executed
 * @constructor
 */
function HtmlLogger(outputDiv, headerMessage, testSuiteDirectory) {
  this.outputDiv = outputDiv;
  this.headerMessage = headerMessage;
  this.testSuiteDirectory = testSuiteDirectory;
  this.directoryPassed = true;
  this.summaryResult = 0;
  this.statusClassName = ['pass', 'unv', 'warn', 'fail'];

  this.priorityFailInfo = {};

  this.priorityFailInfo[Test.PRIORITY.P4] = [];
  this.priorityFailInfo[Test.PRIORITY.P3] = [];
  this.priorityFailInfo[Test.PRIORITY.P2] = [];
  this.priorityFailInfo[Test.PRIORITY.P1] = [];
  this.priorityFailInfo[Test.PRIORITY.P0] = [];
}

/**
 * Creates table for each test which will be filled when the result is
 * available.
 * @param {Test} test to be displayed
 * @return {string} HTML table for the test
 */
HtmlLogger.prototype.createTestTableEntry = function(test) {
  return '<table class="resultList" id="table_' + test.id + '"><tbody>' +
      '</tbody></table>';
};

/**
 * Returns new HTML results summary table
 * @param {string} suiteId locator for suite
 * @return {string} HTML table for test suite summary tabulation
 */
HtmlLogger.prototype.createSuiteSummaryTable = function(suiteId) {
  return '<table id="' + suiteId + '_table"><tr>' +
      '<td>Passed</td><td>Failed</td><td>Warnings</td><td>Unverified</td>' +
      '<td>Total</td></tr><tr>' +
      '<td id="' + suiteId + '_PASS">0</td>' +
      '<td id="' + suiteId + '_FAIL">0</td>' +
      '<td id="' + suiteId + '_WARN">0</td>' +
      '<td id="' + suiteId + '_UNV">0</td>' +
      '<td id="' + suiteId + '_TOT">0</td></tr></table>';
};

/**
 * Prepares the report format. This report is shown when tests are launched
 * and as tests begin to finish it is updated with other details.<br>
 * As test completes results are added (hidden initially) and summary table
 * is updated.
 */
HtmlLogger.prototype.logTestGrid = function() {
  var html = [];
  var hidden = this.testSuiteDirectory.suites.length > 1;

  // header
  if (this.headerMessage) {
    html.push('<table class="header"><tbody><tr>');
    html.push('<td class="header" align="center">' + this.headerMessage);
    html.push('</td></tr><tr><td class="note" align="center">' +
        'Note: Please ignore any flash or messages that appear. ' +
        'They are generated from our tests.' +
        '</td></tr></tbody></table>');
  }

  // Gadget Summary
  html.push('<table class="gadget-summary" id="summary"><tbody><tr>');
  html.push('<td width="10%" id="execution-status" class="execution-status">');
  html.push('Executing...</td></td><td width="45%">');
  html.push(this.testSuiteDirectory.name + '</td><td width="45%">' +
      this.createSuiteSummaryTable('TOTAL') + '</td></tr>');
  html.push('<tr><td colspan="3">' +
      '<a href="#" onclick="XmlLoggerUtils.generateXML(); return false;" ' +
      'id="xml-link" style="display:none;">Generate XML report</a>' +
      '<textarea id="xmlArea" name="xmlArea" style="display:none;">' +
      '</textarea></td></tr><tr><td colspan="3">');
  html.push('<table id="priority-summary"><tbody></tbody>' +
              '</table></td></tr></tbody></table>');
  html.push('<div id="priority-fail-info" style="display: none;"></div>');
  var allResults = [];
  var failCases = 0;
  var totalCases = 0;
  for (var i = 0; i < this.testSuiteDirectory.suites.length; i++) {
    var suiteId = this.testSuiteDirectory.suites[i].id;
    var tests = this.testSuiteDirectory.suites[i].tests;
    var suiteName = this.testSuiteDirectory.suites[i].name;
    // generate results body for suite
    var htmlBody = [];
    var manualTests = 0;
    for (var j = 0; j < tests.length; j++) {
      if (tests[j].manual) {
        manualTests += 1;
      }
      htmlBody.push(this.createTestTableEntry(tests[j]));
    }

    // Suite Summary
    html.push('<table class="suitesummary"><tbody>');
    html.push('<tr id="' + suiteId + '_summary_row"><td  width="10%">');
    html.push('<input id="' + suiteId + '_button" value="' +
        (hidden ? 'Expand' : 'Collapse') + '" type="button" ' +
        'onClick="HtmlLoggerUtils.toggleHideTestSuiteResults(this)">');
    html.push('<td width="45%">');
    html.push('<span class="suitename" id="' + suiteId + '_title">' +
        suiteName + '</span></td>');

    // print suiteSummaryTable
    html.push('<td width="45%">')
    html.push(this.createSuiteSummaryTable(suiteId));
    html.push('</td></tr>');

    html.push('<tr class="test-summary"');
    if (hidden) {
      html.push(' style="display: none"');
    }
    html.push('><td colspan=3>');
    // SuiteHeader
    html.push('<table><tbody><tr><td colspan="3">');
    if (tests.length > 1) {
      html.push('<input type="button" value="' +
          (hidden ? 'Expand all sub results' : 'Collapse all sub results') +
          '" onClick="HtmlLoggerUtils.toggleHideAllSubResults(this)"' +
          ' id="' + suiteId + '_error_expand">');
      html.push(' <input type="button" " value="Re run suite" ' +
          'onClick="testRunner.reRunSuite(\'' + suiteId + '\')">');
    }
    html.push('<span id="' + suiteId + '_error_links">Failures :: 0</span>');
    html.push('</td></tr></tbody></table>');

    // test suites
    html.push(htmlBody.join(''));
    // close suite summary
    html.push('</td></tr></tbody></table>');
  }

  // footer - json script
  html.push('<table class="json" id="json">' +
      '<tbody><tr><td colspan="2" align="right">');

  html.push('<input id="save-results" value="Save this results as baseline" type="button" ' +
      'onClick="HtmlLoggerUtils.saveGoldenResults(this, \'' +
      this.testSuiteDirectory.id + '\', \'testArea\')">');
  html.push('</td><td align="left" colspan="2">');
  html.push('<input id="update-results" value="Update baseline results" type="button" ' +
      'onClick="HtmlLoggerUtils.saveGoldenResults(this, \'' +
      this.testSuiteDirectory.id + '\', \'goldenArea\')">');

  html.push('</td></tr><tr><td width="5%">Results:</td><td width="45%">');
  html.push('<textarea id="testArea" name="testArea" rows="2" ' +
      'cols="45"></textarea>');
  html.push('</td><td width="5%">Baseline Results:</td><td width="45%">');
  html.push('<textarea id="goldenArea" name="goldeArea"' +
      ' cols="45" rows="2"></textarea></td></tr></tbody></table>');

  this.outputDiv.innerHTML = html.join('');

  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * Displays form to set required fields for restful testing.
 * @param finishCallback
 */
HtmlLogger.prototype.restfulPreTestDialog = function(finishCallback) {
  if (Config.ready || Config.baseUrl != null && Config.securityToken != null) {
    Config.ready = true;
    this.logTestGrid();
    finishCallback();
    return;
  }
  HtmlLoggerUtils.logger = this;
  HtmlLoggerUtils.callback = finishCallback;
  var html = [];
  html.push('<table><tbody><th colspan="2">');
  html.push('Enter details to continue testing.</th>');
  if (Config.baseUrl == null) {
    html.push('<tr><td>Base URL:</td></td>' +
      '<input id="baseUrl" type="text" name="baseUrl" size="50" value=""/>' +
      '</td></tr>');
  }
  if (Config.securityToken == null) {
    html.push('<tr><td>Security Token Prefix:</td><td>' +
      '<input id="tokenPrefix" type="text" name="tokenPrefix" value="st"' +
      ' size="50"/></td></tr>');
    html.push('<tr><td>Security Token:</td><td>' +
      '<input id="securityToken" type="text" name="securityToken" size="50"/>' +
      '</td></tr>');
  }
  html.push('<tr><hr/></tr><tr>Sample url: http://BASEURL?field=value1&' +
      'TOKEN-PREFIX=SECURITY-TOKEN</tr>');
  html.push('<tr><td><input type="button" value="Submit" ' +
      'onClick="HtmlLoggerUtils.logger.handleRestfulPreTestConfig()"/>' +
      '</td></tr></tbody></table>');
  this.outputDiv.innerHTML = html.join('');
};

/**
 * Sets up the config file for restful testing. Displays test grid and calls
 * callback. 
 */
HtmlLogger.prototype.handleRestfulPreTestConfig = function() {
  var baseUrl = document.getElementById('baseUrl');
  baseUrl = baseUrl ? baseUrl.value : Config.baseUrl;
  var tokenPrefix = document.getElementById('tokenPrefix');
  tokenPrefix = tokenPrefix ? tokenPrefix.value : Config.tokenPrefix;
  var securityToken = document.getElementById('securityToken');
  securityToken = securityToken ? tokenPrefix + '=' + securityToken.value
      : Config.securityToken;
  Config.saveUrls(baseUrl, securityToken);
  this.logTestGrid();
  HtmlLoggerUtils.callback();
};

/**
 * Updates result for the executed test. This method is called to log result
 * each time test is executed. It performs following tasks<br>
 * 1. It creates entry for the test<br>
 * 2. Notes if the test result is different from golden result<br>
 * 3. It adds bugs link for known bugs for the test<br>
 * 4. Adds buttons to re-run and expand/collapse sub-results
 *
 *  @param {ResultGroup} result of the test executed
 */
HtmlLogger.prototype.updateTestResult = function(result) {
  var tableElem = document.getElementById('table_' + result.test.id);
  var newRowLoc = tableElem.rows.length;
  tableElem.insertRow(newRowLoc);
  var newRow = tableElem.rows[newRowLoc];
  newRow.style.display = 'table-row';

  var changeStatus = (result.delta) ? 'broken' : 'fixed';

  var className = 'category';
  className += result.delta ? ('-changed-' + changeStatus) : '';
  newRow.className = className;

  var time = result.startTime && result.finishtime ?
      ' [' + (result.finishtime - result.startTime) + ' ms] ' : '';

  var isStableTest = Assert.arrayContains(result.tags, 'stable');

  var entry = [];

  entry.push('<td class="' + className + '">');
  entry.push('<span class="testname">');
  entry.push('[' + result.id + '] [' +
      (result.test.priority ? result.test.priority : Test.PRIORITY.P2) +
      ' ]:: ');
  entry.push('<a name="' + result.id + '"> ');
  entry.push(result.name);
  entry.push('</a></span>: ');
  if (result.severity < Result.severity.PASS) {
    result.severity = Result.severity.UNVERIFIED;
  }
  entry.push('<span class="result ' +
      Result.severity.getString(result.severity).toLowerCase() +
      '">' + Result.severity.getString(result.severity) + '</span>');
  if (result.delta) {
    entry.push('<span class="result"><b>[CHANGED FROM LAST RUN]</b></span>');
  }
  entry.push(time);

  // Display if the test is marked as stable.
  if (result.tags && result.tags.length > 0) {
    if (isStableTest) {
      entry.push('<br><span class="stable"> Stable Test</span>');
    }
  }
  // Display test result roll up.
  var total = result.getTotal();

  if (Config.internal && Config.internal == true) {
    // Display test case associated bugs links.
    if (result.bugs && result.bugs.length > 0) {
      entry.push('<br>Bugs: ');
      for (var i = 0; i < result.bugs.length; i++) {
        entry.push('<a href="http://b/issue?id=' + result.bugs[i] +
            '" target="_blank">' + result.bugs[i] + '</a> ');
      }
    }
  }
  var hidden = !result.delta && total.warnings == 0 && total.failed == 0;
  // Add Run Again button.
  entry.push('<br><input value="' + (hidden ? 'Expand' : 'Collapse') + '" ' +
      'type="button" onClick="HtmlLoggerUtils.toggleHideSubResult(this)">');
  entry.push('<input value="Run Again" type="button" ');
  entry.push('onclick="testRunner.reRunTest(\'' + result.id + '\');" ');
  entry.push('id="btn' + result.id + '"/>');
  entry.push('</td>');
  newRow.innerHTML = entry.join('');

  for (var j = result.verifications.length - 1; j >= 0; j--) {
    this.addVerifications(result.verifications[j], result.test.id, hidden);
  }

  if (result.test.description) {
    this.addDescription(result.test, hidden);
  }
};

/**
 * Creates one HTML table row for the test description
 * @param {TestCase} test testcase whose description to be printed
 * @param {boolean} hidden whether the row to be displayed in the beginning
 */
HtmlLogger.prototype.addDescription = function(test, hidden) {
  var tableElem = document.getElementById('table_' + test.id);
  tableElem.insertRow(1);
  var newRow = tableElem.rows[1];
  if (hidden) {
    newRow.style.display = 'none';
  }

  newRow.className = 'verification';
  newRow.innerHTML = '<td><span class="desc">' +
      'Description> </span>' + test.description + '</td></tr>';
};

/**
 * Creates one HTML row for the result object (ResultValidation).
 * @param {ResultValidation} result to convert to HTML
 * @param {string} testId id of the test case
 * @param {boolean} hidden whether the row should be hidden or displayed.
 *     If set to true, the row will have display style none
 */
HtmlLogger.prototype.addVerifications = function(result, testId, hidden) {
  var tableElem = document.getElementById('table_' + testId);
  tableElem.insertRow(1);
  var newRow = tableElem.rows[1];
  if (hidden) {
    newRow.style.display = 'none';
  }
  var changeStatus = (result.delta) ? 'broken' : 'fixed';

  var className = 'verification';
  className += result.delta ? ('-changed-' + changeStatus) : '';
  newRow.className = className;

  var time = result.startTime && result.finishtime ?
      ' [' + (result.finishtime - result.startTime) + ' ms] ' : '';

  var actualValue = (typeof(result.actual) == 'object') ?
      Helper.getString(result.actual) : result.actual;

  var expectedValue = (typeof(result.expected) == 'object') ?
      Helper.getString(result.expected) : result.expected;

  var entry = [];
  entry.push('<td class="' + className + '">');
  entry.push('<span class="testname">');
  entry.push('<a name="' + result.id + '"></a>');
  entry.push('[' + result.id + '] ');
  entry.push(result.name);
  entry.push('</span>: ');
  entry.push('<span class="result ' +
      Result.severity.getString(result.severity).toLocaleLowerCase() +
      '">' + Result.severity.getString(result.severity) + '</span>');
  if (result.delta) {
    entry.push('<span class="result"><b>[CHANGED FROM LAST RUN]</b></span>');
  }
  entry.push(time);
  entry.push('<span class="result actual">: ');
  entry.push('(got \'');
  entry.push(actualValue);
  entry.push('\')</span>');
  if (result.severity > Result.severity.PASS) {
    entry.push(', expected <span class="result expected">\'');
    entry.push(expectedValue);
    entry.push('\'</span>');
  }
  entry.push('</td>');
  newRow.innerHTML = entry.join('');
};

/**
 * Updates test summary table for the test suite and gadget
 * @param {ResultGroup} result of the test
 */
HtmlLogger.prototype.updateSuiteSummary = function(result) {
  var resultText;
  if (result.severity == Result.severity.UNVERIFIED) {
    resultText = Result.severity.getString(result.severity).substr(0, 3);
  } else {
    resultText = Result.severity.getString(result.severity).substr(0, 4);
  }

  var total = 0;
  var found = false;
  var failCount = 0;
  var passCount = 0;
  var unvCount = 0;
  var warnCount = 0;

  for (var i = 0; i < this.testSuiteDirectory.suites.length; i++) {
    var suite = this.testSuiteDirectory.suites[i];
    var resultElem = document.getElementById(suite.id + '_' + resultText);
    var suiteTotalElem = document.getElementById(suite.id + '_TOT');
    var suiteTotal = 0;
    for (var j = 0; j < suite.tests.length; j++) {
      if (suite.tests[j].id == result.test.id) {
        var resCount = parseInt(resultElem.innerHTML, 10);
        resultElem.innerHTML = ++resCount;
        suiteTotal = parseInt(suiteTotalElem.innerHTML, 10);
        suiteTotalElem.innerHTML = ++suiteTotal;
        found = true;
        break;
      } else if (found) {
        break;
      }
    }

    // calculate counts for final summary
    var counterElem = document.getElementById(suite.id + '_PASS');
    passCount += parseInt(counterElem.innerHTML);
    counterElem = document.getElementById(suite.id + '_FAIL');
    failCount += parseInt(counterElem.innerHTML);
    counterElem = document.getElementById(suite.id + '_WARN');
    warnCount += parseInt(counterElem.innerHTML);
    counterElem = document.getElementById(suite.id + '_UNV');
    unvCount += parseInt(counterElem.innerHTML);

    total += parseInt(suiteTotalElem.innerHTML);
  }
  // Update final summary;
  var counterLocation = document.getElementById('TOTAL_PASS');
  counterLocation.innerHTML = passCount;
  counterLocation = document.getElementById('TOTAL_FAIL');
  counterLocation.innerHTML = failCount;
  counterLocation = document.getElementById('TOTAL_WARN');
  counterLocation.innerHTML = warnCount;
  counterLocation = document.getElementById('TOTAL_UNV');
  counterLocation.innerHTML = unvCount;
  counterLocation = document.getElementById('TOTAL_TOT');
  counterLocation.innerHTML = total;
};

/**
 * Updates the priority table with the details of priority tests.
 * @param {Array} priorityFailInfo information of failed cases on per
 *     priority basis
 * @param {status} status of the test execution
 */
HtmlLogger.prototype.updatePriorityTable = function(priorityFailInfo, status) {
  var priorityDescription = {};
  priorityDescription[Test.PRIORITY.P4] =
      'Visual cosmetic bugs. Or something like that. Lowest priority bug.' +
      'You can fix when you have nothing else to do.';
  priorityDescription[Test.PRIORITY.P3] =
      'But that causes some irritation but you can live with it most of the' +
      ' time.';
  priorityDescription[Test.PRIORITY.P2] =
      'Functional nice to have breaks. i.e Exceptions for undefined behavior';
  priorityDescription[Test.PRIORITY.P1] =
      'Important bugs not exactly falling in the P0s. But have negative impact';
  priorityDescription[Test.PRIORITY.P0] =
      'You are not doing something stated in the spec. You are not spec' +
      ' compliant';

  var prioritySummary = document.getElementById('priority-summary');
  for (var priority in priorityFailInfo) {
    if (priorityFailInfo[priority].length > 0) {
      var priorityDesc = [];
      for (var i = 0; i < priorityFailInfo[priority].length; ++i) {
        priorityDesc.push(priorityFailInfo[priority][i].name + ' :: ' +
                          priorityFailInfo[priority][i].id);
      }
      prioritySummary.insertRow(0);
      var row = prioritySummary.rows[0];
      row.innerHTML = '<td width="5%"> ' + priority + ' </td>' +
          '<td width="38%">' + priorityDescription[priority] + '</td>' +
          '<td width="7%"><span><b>' + priorityFailInfo[priority].length +
          '</span></b></td><td width="50%">' +
          priorityDesc.join('<br/>') + '</td>';
    }
  }

  if (prioritySummary.rows.length > 0) {
    prioritySummary.insertRow(0);
    var row = prioritySummary.rows[0];
    row.innerHTML = '<td align="center"><span><b>Priority</b></span></td>' +
                    '<td align="center"><span><b>Description</b></span></td>' +
                    '<td align="center"><span><b>Failures</b></span></td>' +
                    '<td align="center"><span><b>Info</b></span></td>';

    prioritySummary.className = status;
  }

  var priorityInfoDiv = document.getElementById('priority-fail-info');
  priorityInfoDiv.innerHTML = gadgets.json.stringify(priorityFailInfo);
};

/**
 * Adds error links for failed, unverified and warning results for each
 * suite. It will also make sure that suite button caption is correct.
 *
 *  @param {Array.<ResultSet>} resultSets array of resultSets each resultSet
 *      maps to testSuite in a directory
 */
HtmlLogger.prototype.updateSuitesHeader = function(resultSets) {
  var suiteResult = 0;

  for (var i = 0; i < resultSets.length; i++) {
    var id = resultSets[i].id;
    var failText = '';
    var warnText = '';
    var unvText = '';
    var warn = 0;
    var unv = 0;
    var fail = 0;

    for (var j = 0; j < resultSets[i].results.length; j++) {
      var result = resultSets[i].results[j];
      switch(result.severity) {
          case Result.severity.UNVERIFIED:
            unvText += '<a href="#' + result.test.id + '">' +
                result.test.id + '</a>, ';
            unv++;
            break;
          case Result.severity.FAIL:
            var link = '<a href="#' + result.test.id + '">' +
                result.test.id + '</a>, '
            failText += link;
            fail++;
            this.priorityFailInfo[
                result.test.priority ? result.test.priority : Test.PRIORITY.P2
                ].push({ name: resultSets[i].name, id: result.id });
            break;
          case Result.severity.WARNING:
            warnText += '<a href="#' + result.test.id + '">' +
                result.test.id + '</a>, ';
            warn++;
            break;
      }
    }

    suiteResult = (fail > 0) ? 3 : (warn > 0) ? 2 : (unv > 0) ? 1 : 0;

    if (suiteResult > this.summaryResult) {
      this.summaryResult = suiteResult;
    }

    var suiteTitle = document.getElementById(id + '_title');
    suiteTitle.className = this.statusClassName[suiteResult];

    var suiteTable = document.getElementById(id + '_table');
    suiteTable.className = this.statusClassName[suiteResult];

    var elem = document.getElementById(id + '_error_links');
    elem.innerHTML =
        (fail ? 'Failed: ' + failText : '') +
        (warn ? '<br/> Warnings: ' + warnText : '') +
        (unv ? '<br/> Unverifieds: ' + unvText : '');
  }
};

/**
 *  Updates main summary table after all tests are run
 *
 *  @param {Array.<ResultSet>} resultSets array of resultSets each resultSet
 *      maps to testSuite in a directory
 */
HtmlLogger.prototype.updateSummaryHeader = function(resultSets) {
  this.updatePriorityTable(this.priorityFailInfo,
      this.statusClassName[this.summaryResult]);

  if (resultSets.length == 1) {
    var suite_summary_row =
        document.getElementById(resultSets[0].id + '_summary_row');
    suite_summary_row.style.display = 'none';
  }

  var summary = document.getElementById('summary');
  summary.className = 'gadget-' + this.statusClassName[this.summaryResult];
  var summaryTable = document.getElementById('TOTAL_table');
  summaryTable.className = this.statusClassName[this.summaryResult];

  var executionStatus = document.getElementById('execution-status');
  if (this.testSuiteDirectory.suites.length === 1) {
    executionStatus.innerHTML = '<input value="Collapse" type="button" ' +
        'onClick="HtmlLoggerUtils.toggleHideTestSuites(this)">';
  } else {
    executionStatus.innerHTML = 'Finished';
    executionStatus.className = '';
  }
};

/**
 * Will convert results in simple Array and puts it in jason box for you to save
 * it as golden results. Golden results will be used as benchmark for later
 * test results.
 * @param {Array.<ResultSets>} resultSets The array of resultSets
 * @param {Object} goldenResults Jason representation of results
 */
HtmlLogger.prototype.updateResultBoxes = function(resultSets, goldenResults, context) {
  // golden results hidden box
  var goldenBoxHidden = document.getElementById('testArea');
  goldenBoxHidden.innerHTML = Helper.getString(
      Helper.convertResultsToMapExcludeVerifications(resultSets));

  HtmlLoggerUtils.setResultCollection(resultSets);

  // visible golden results
  var goldenBoxVisible = document.getElementById('goldenArea');
  goldenBoxVisible.innerHTML = Helper.stringify(goldenResults);

  XmlLoggerUtils.setResultSet(resultSets);
  XmlLoggerUtils.setContainer(testRunner.getContext().getContainer().name);
  XmlLoggerUtils.setContext(context);
  var elem = document.getElementById('xml-link');
  elem.style.display = "inline";


  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * HtmlLoggerUtils namespace.
 * This namespace holds many functions to aid the generated HTML from the
 * HtmlLogger class, contains methods to collapse/expand results, store result
 * json information for external calls, and handlers for the buttons generated
 * by the HtmlLogger.
 */
var HtmlLoggerUtils = function() {};

/** Text for collapse all button displayed at suite summary level */
HtmlLoggerUtils.COLLAPSE_ALL_TEXT = 'Collapse all sub results';

/** Text for expand all button displayed at suite summary level */
HtmlLoggerUtils.EXPAND_ALL_TEXT = 'Expand all sub results';

/**
 * Toggles the visibility of the sub-results of a single result in result table,
 * changes the text of the button that triggered this action
 * (parameter 'element') and finally if possible adjusts
 * the height of the gadget.<br>
 * This function searches for the parent table container of each result then
 * for every TR inside other than the result header, will change it display
 * property to 'none' or 'table-row' as necessary.<br>
 * Additionally at the end of the function if gadgets.window.adjustHeight
 * function is available it will be called to readjust the height of the gadget.
 * @param {Element} element The button that triggered the collapse/expand event.
 */
HtmlLoggerUtils.toggleHideSubResult = function(element) {
  // Change the button value accordingly
  if (element.tagName == 'INPUT') {
    if (element.value == 'Collapse') {
      element.value = 'Expand';
    } else {
      element.value = 'Collapse';
    }
  }

  var displayStyle;
  if (element.value == 'Expand') {
    displayStyle = 'none';
  } else {
    displayStyle = 'table-row';
  }

  // Find the table container
  while (element.tagName != 'TABLE') {
    element = element.parentNode;
  }
  // Find the contained elements to be hidden (the inner table)
  var tables = element.getElementsByTagName('TR');
  for (var i = 0; tables && i < tables.length; i++) {
    if (tables[i].className == 'verification') {
      tables[i].style.display = displayStyle;
    }
  }
  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * Toggles the visibility of the <b>sub-results for all tests in a suite</b>,
 * changes the text of the button that triggered this action
 * (parameter 'element') and finally if possible adjusts the height of the
 * gadget.<br>
 * This function searches for the parent row container of all results then
 * for every INPUT element in that row container, will check if it needs to be
 * toggled and call HtmlLoggerUtils.toggleHideSubResults.<br>
 * Additionally at the end of the function if gadgets.window.adjustHeight
 * function is available it will be called to readjust the height of the gadget.
 * @param {Element} element The button that triggered the collapse/expand event.
 */
HtmlLoggerUtils.toggleHideAllSubResults = function(element) {
  // Change the button value accordingly
  if (element.tagName == 'INPUT') {
    if (element.value == HtmlLoggerUtils.COLLAPSE_ALL_TEXT) {
      element.value = HtmlLoggerUtils.EXPAND_ALL_TEXT;
    } else {
      element.value = HtmlLoggerUtils.COLLAPSE_ALL_TEXT;
    }
  }
  var caption = element.value;

  // Find the table container
  while (element.tagName != 'TR' || element.className != 'test-summary') {
    element = element.parentNode;
  }

  // Find the contained elements to be hidden (the inner table)
  var inputs = element.getElementsByTagName('INPUT');
  for (var i = 0; inputs && i < inputs.length; i++) {
    if (inputs[i].value == 'Expand' &&
        caption == HtmlLoggerUtils.COLLAPSE_ALL_TEXT) {
      HtmlLoggerUtils.toggleHideSubResult(inputs[i]);
    } else if (inputs[i].value == 'Collapse' &&
        caption == HtmlLoggerUtils.EXPAND_ALL_TEXT) {
      HtmlLoggerUtils.toggleHideSubResult(inputs[i]);
    }
  }
  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * Toggles the visibility of results for all tests in a suite. It hides/shows
 * detailis of results of suite. It also toggles the name of the button that
 * triggered this action (parameter 'element') and finally if possible adjusts
 * the height of the gadget.<br>
 * This function searches for the parent table container for suite then
 * for every TR with class name 'test-summary', will change its display
 * property to 'none' or 'table-row' as necessary.<br>
 * Additionally at the end of the function if gadgets.window.adjustHeight
 * function is available it will be called to readjust the height of the gadget.
 * @param {Element} element The button that triggered the collapse/expand event.
 */
HtmlLoggerUtils.toggleHideTestSuiteResults = function(element) {
  // Change the button value accordingly
  if (element.tagName == 'INPUT') {
    if (element.value == 'Collapse') {
      element.value = 'Expand';
    } else {
      element.value = 'Collapse';
    }
  }
  // Find the table container
  while (element.tagName != 'TABLE' || element.className != 'suitesummary') {
    element = element.parentNode;
  }
  // Find the contained elements to be hidden (the inner table)
  var tables = element.getElementsByTagName('TR');
  for (var i = 0; tables && i < tables.length; i++) {
    if (tables[i].className == 'test-summary') {
      if (tables[i].style.display == 'none') {
        tables[i].style.display = 'table-row';
      } else {
        tables[i].style.display = 'none';
      }
    }
  }
  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * Toggles the visibility of details of all test suites. It hides/shows details
 * of results of all suites. It also toggles the name of the button that
 * triggered this action (parameter 'element') and finally if possible adjusts
 * the height of the gadget.<br>
 * This function finds all elements with class name 'suitesummary',
 * will change display property to 'none' or 'table-row' as necessary for all
 * matching elements.<br>
 * Additionally at the end of the function if gadgets.window.adjustHeight
 * function is available it will be called to readjust the height of the gadget.
 * @param {Element} element The button that triggered the collapse/expand event.
 */
HtmlLoggerUtils.toggleHideTestSuites = function(element) {
  // Change the button value accordingly
  if (element.tagName == 'INPUT') {
    if (element.value == 'Collapse') {
      element.value = 'Expand';
    } else {
      element.value = 'Collapse';
    }
  }
  // Find the contained elements to be hidden (the inner table)
  var tables = document.getElementsByTagName('TABLE');
  for (var i = 0; tables && i < tables.length; i++) {
    if (tables[i].className == 'suitesummary') {
      if (tables[i].style.display == 'none') {
        tables[i].style.display = 'table-row';
      } else {
        tables[i].style.display = 'none';
      }
    }
  }
  if (self.gadgets && gadgets.window && gadgets.window.adjustHeight) {
    // Do this only if dynamic height is supported.  This is useful when it's
    // run inside the orkut container, for example.
    gadgets.window.adjustHeight();
  }
};

/**
 * Stores the results for external static access.
 * @type {Array.<object>}
 */
HtmlLoggerUtils.resultCollection_ = [];

/**
 * Gets the results stored in the static context, this usually happens when the
 * logger renders its output.
 * @return {Array.<object>} An array with the results inside.
 */
HtmlLoggerUtils.getResultCollection = function() {
  return HtmlLoggerUtils.resultCollection_;
};

/**
 * Gets a JSON representation of the results stored in this static context.<br>
 * Note: for this function to work correctly the function gadgets.json.stringify
 * must be defined and work correctly.
 * @return {string} A json representation of the results stored in this static
 *     context.
 */
HtmlLoggerUtils.getResultJson = function() {
  return gadgets.json.stringify(HtmlLoggerUtils.resultCollection_);
};

/**
 * Stores the results array in this static context.<br>
 * This function is usually called by the HtmlLogger when the output is
 * displayed.
 * @param {Array.<object>} results The results to store in this static context.
 */
HtmlLoggerUtils.setResultCollection = function(results) {
  HtmlLoggerUtils.resultCollection_ = results;
};

/**
 * Saves the golden results inside a given TEXTAREA with the specified id.
 * Note that the TEXTAREA will be searched inside in the parent of the element
 * so it is important that both the element and the TEXTAREA to seek are at the
 * same DOM hierarchy level.
 * @param {Element} element The button that triggered the action.
 * @param {string} suiteId The suite id.
 * @param {string} elementId The id of the TEXTAREA that contains the golden
 *     results
 */
HtmlLoggerUtils.saveGoldenResults = function(element, suiteId, elementId) {
  element.disabled = true;
  var goldenContainer = document.getElementById(elementId);
  if (goldenContainer) {
    var goldenResults = gadgets.json.parse(goldenContainer.value);
    Helper.saveGoldenResults(suiteId, goldenResults, function () {
      element.disabled = false;
    });
  }
};
