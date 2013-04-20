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
 * @fileoverview Runner of the restful tests. Subclass of opensocialtestrunner.
 */

/**
 * Creates a new OpenSocial Test Runner. It takes TestSuiteDirectory as input.
 * If TestSuite is given as input it converts it into TestSuiteDirectory for
 * backward compatibility with existing cases.
 * @param {TestSuiteDirectory/TestSuite} object The directory or test suite to
 *     test.
 * @constructor
 */
function RestfulTestRunner(object) {
  /**
   * The test suite directory that will be run
   * @type {TestSuiteDirectory}
   */
  this.testSuiteDirectory = null;

  if (object.suites) {
    this.testSuiteDirectory = object;
  } else {
    var directory = new TestSuiteDirectory(object.id, object.name);
    directory.suites.push(object);
    this.testSuiteDirectory = directory;
  }

  /**
   * Logger used for displaying test report
   * @type {ComplianceHtmlLogger}
   */
  this.logger = null;

  /**
   * Context in which we are running the tests
   * @type {Context}
   */
  this.context = null;

  /**
   * flag to check whether to skip set up tests or not
   * @type {boolean}
   */
  this.skipSetupTests = false;
}


/**
 * Inherit testRunner
 */
RestfulTestRunner.prototype = new TestRunner;

/**
 * Setups this TestRunner objects preparing it to run the tests and verifies
 * the existence of viewer and owner objects as well as its correctness.
 * @param {Function()} finishCallback The callback to be invoked when the setup
 *     is finished.
 * @override
 */
RestfulTestRunner.prototype.setupTests = function(finishCallback) {
  var req = opensocial.newDataRequest();
  var suiteResultsKey = this.testSuiteDirectory.id + '_results_'
      + Config.goldenResultsLabel;
  req.add(req.newFetchPersonRequest('VIEWER'), 'viewer');
  req.add(req.newFetchPersonRequest('OWNER'), 'owner');
  req.add(req.newFetchPersonAppDataRequest(Config.VIEWER, suiteResultsKey),
      'fetchResults');
  var that = this;
  req.send(function(dataResponse) {
    var viewer;
    var owner;
    var data;

    var preTest = new Array();
    if (dataResponse.hadError() && !that.skipSetupTests) {
      preTest.push({
          name: 'Setup testing suite. Getting owner/viewer data.',
          id: '0_SETUP000',
          dataResp: dataResponse,
          run: function(context, callback, result) {
            Helper.addSubResult(result, 'WARNING:', true,
                'Response: ' + gadgets.json.stringify(dataResponse), '');
            callback(result);
          }
      });
    }
    if (dataResponse.get('viewer') && !dataResponse.get('viewer').hadError()) {
      viewer = dataResponse.get('viewer').getData();
    }
    if (dataResponse.get('owner') && !dataResponse.get('owner').hadError()) {
      owner = dataResponse.get('owner').getData();
    }

    if (viewer && owner) {
      // Get golden results.
      if (dataResponse.get('fetchResults') &&
          !dataResponse.get('fetchResults').hadError()) {
        data = dataResponse.get('fetchResults').getData();
        if (data && data[viewer.getId()]) {
          var jsonString = data[viewer.getId()][suiteResultsKey];
          if (jsonString == undefined) {
            that.goldenResult = undefined;
          } else {
            var unescapedJsonString = gadgets.util.unescapeString(jsonString);
            var json = gadgets.json.parse(unescapedJsonString);
            that.goldenResults = json.suiteResults;
          }
        }
      }
    }
    if (!owner && !that.skipSetupTests) {
      preTest.push({
          name: 'Setup testing suite. OWNER IS NULL, UNDEFINED OR EMPTY.',
          id: '0_SETUP001',
          ownerData: owner,
          run: function(context, callback, result) {
            Helper.addSubResult(result, 'FATAL ERROR:', false,
                'OWNER is:' + gadgets.json.stringify(this.ownerData),
                'OWNER must never be null, undefined or empty object');
            callback(result);
          }
      });
    }
    if (!viewer && !that.skipSetupTests) {
      preTest.push({
          name: 'Setup testing suite. Viewer is null, undefined or empty.',
          id: '0_SETUP002',
          viewerData: viewer,
          run: function(context, callback, result) {
            Helper.addSubResult(result, 'WARNING:', true,
                'viewer is:' + gadgets.json.stringify(this.viewerData), '',
                Result.severity.WARNING);
            callback(result);
          }
      });
    }
    if (preTest && (preTest.length > 0)) {
      if (that.testSuiteDirectory.length == 1) {
        that.testSuiteDirectory.suites[0].tests = Array.concat(preTest,
            that.testSuiteDirectory.suites[0]);
      }
    }
    var env = opensocial.getEnvironment();
    var domain = env && env.getDomain();
    var context = new Context(viewer, owner, domain);
    that.context = context;
    that.renderTestGrid("Running restful API test", finishCallback);

  });
};

/**
 * Prints a preview table to monitor test execution.
 * @param {string} header An optional header to be printed at the top of the
 *     test output.
 */
RestfulTestRunner.prototype.renderTestGrid = function(header, finishCallback) {
  this.logger = new HtmlLogger(
      document.getElementById('testResults'), header, this.testSuiteDirectory);
  this.logger.restfulPreTestDialog(finishCallback);
};

/**
 * Gets the context object for the tests.
 * @return {object} The context generated for the tests.
 * @override
 */
RestfulTestRunner.prototype.getContext = function() {
  return this.context;
};

/**
 * Gets the golden results for the tests to compare their results against it.
 * @return {object} The golden results to compare against the test results.
 * @override
 */
RestfulTestRunner.prototype.getGoldenResults = function() {
  return this.goldenResults;
};

/**
 * Saves the golden results and then prints the output on the gadget canvas when
 * all tests have finished.
 * @param {Array.<ResultSet>} resultSets The result sets of the test suites.
 * @override
 */
RestfulTestRunner.prototype.onAllFinish = function(resultSets) {
  var runner = new Helper.FunctionRunner();
  runner.add(this.logger.updateSummaryHeader, [resultSets], this.logger);
  runner.add(this.logger.updateResultBoxes,
      [resultSets, this.goldenResults], this.logger);
  runner.run();
};

/**
 * Updates suite header when suite has finished
 * all tests have finished.
 * @param {Array.<ResultSet>} resultSets The result sets of the test suites.
 * @override
 */
RestfulTestRunner.prototype.onSuiteFinish = function(resultSets) {
  var runner = new Helper.FunctionRunner();
  runner.add(this.logger.updateSuitesHeader, [resultSets], this.logger);
  runner.run();
};

/**
 * Change the status of a particular test when it finishes its execution
 * changing the displayed value to the right of the id.
 * @param {ResultGroup} result The result of the test.
 * @param {Test} test The test that generated the result.
 * @override
 */
RestfulTestRunner.prototype.onTestFinish = function(result, test) {
  var runner = new Helper.FunctionRunner();
  runner.add(this.logger.updateTestResult, [result, test], this.logger);
  runner.add(this.logger.updateSuiteSummary, [result, test], this.logger);
  runner.run();
};

/**
 * Executes all the tests in the suite.
 */
RestfulTestRunner.prototype.executeTests = function() {
  var validator = new Validator(this.getContext(), this.testSuiteDirectory);
  validator.goldenResults = this.getGoldenResults();
  validator.testFinishFunction = this.onTestFinish.bind(this);
  validator.suiteFinishFunction = this.onSuiteFinish.bind(this);
  validator.runManualTest = this.runManualTest;
  validator.runTests(this.onAllFinish.bind(this));
};

/**
 * Clears the test suite directory array and adds only the specified test
 * as suite to it.
 * @param {String} testId Id of the test to add.
 */
RestfulTestRunner.prototype.setSingleTestAsDirectory = function(testId) {
  var directory = this.testSuiteDirectory;
  for (var i = 0; i < directory.suites.length; i++) {
    var testSuite = directory.suites[i];
    for (var j = 0; j < testSuite.tests.length; j++) {
      if (testSuite.tests[j].id == testId) {
        this.testSuiteDirectory.suites = [];
        var suite = new TestSuite();
        suite.tests.push(testSuite.tests[j]);
        suite.id = testSuite.id;
        suite.name = testSuite.name;
        this.testSuiteDirectory.suites.push(suite);
        return;
      }
    }
  }
};

/**
 * Re-runs only the specified test suite.
 * @param {String} suiteId Id of the testSuite to re-run.
 */
RestfulTestRunner.prototype.reRunSuite = function(suiteId) {
  this.setSingleSuiteAsDirectory(suiteId);
  this.runManualTest = true;
  this.run();
};


/**
 * Clears the test suite array and adds only the specified test to it.
 * @param {String} suiteId Id of the test to add.
 */
RestfulTestRunner.prototype.setSingleSuiteAsDirectory = function(suiteId) {
  var directory = this.testSuiteDirectory;
  for (var i = 0; i < directory.suites.length; i++) {
    if (directory.suites[i].id == suiteId) {
        this.testSuiteDirectory.suites = [directory.suites[i]];
        return;
    }
  }
};
