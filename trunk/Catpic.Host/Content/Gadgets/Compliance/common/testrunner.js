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
 * @fileoverview Runner of the tests.  It's used as a base class for other test
 * runners. This class provides basic methods for running the tests as well as
 * stubs to methods that if extended can customize the behavior of the runner.
 * In order to customize this test runner to fit all specific needs some methods
 * can be overridden, These methods are:<br>
 * <ul>
 * <li><b>setupTests</b> - Override this method in order to add some setup code
 *     that will run before the tests are run, here context and golden results
 *     can be set, also we can add custom generated tests </li>
 * <li><b>getContext</b> - Override this method to provide tests with a context
 *     object, if this method is not overridden then an empty object is passed
 *     as context for the tests.</li>
 * <li><b>getGoldenResults</b> - Override this method to give the tests golden
 *     results to compare in order to generate deltas. If this method is not
 *     overridden then there will be no delta comparisson.</li>
 * <li><b>onTestFinish</b> - Override this method to manage results as they
 *     finish, this method is called every time a test finishes and has a
 *     result ready.</li>
 * <li><b>onAllFinish</b> - This method should always be overridden in order to
 *     display or manage the results from the tests. This method is called
 *     when all tests have finished and there are results ready.</li>
 * </ul>
 */

/**
 * Constructs a new empty TestRunner, in order to run the tests the variable
 * testSuite must be set with a valid TestSuite.
 * @constructor
 */
function TestRunner() {
  /**
   * A flag indicating if manual tests should be run.
   * @type {boolean}
   */
  this.runManualTest = false;
}

/**
 * Runs setup of the test environment before tests are run.
 * To add some custom behavior this method should be overridden.
 * @param {Function()} finishCallback The callback to call once the setup is
 *     complete. Is important to call this method always and only once at the
 *     end of the setup process.
 */
TestRunner.prototype.setupTests = function(finishCallback) {
  finishCallback();
};

/**
 * Gets the context for the tests.
 * To add some custom behavior this method should be overridden.
 * @return {object} The context object.
 */
TestRunner.prototype.getContext = function() {
  return {};
};

/**
 * Gets the golden result to compare against the results and generate delta
 * information
 * To add some custom behavior this method should be overridden.
 * @return {object} The golden results object.
 */
TestRunner.prototype.getGoldenResults = function() {
  return {};
};

/**
 * Runs every time a single test finish.
 * To add some custom behavior this method should be overridden.
 * @param {ResultGroup} result The result generated by the test.
 * @param {Test} test The test that finished its execution.
 */
TestRunner.prototype.onTestFinish = function(result, test) { };

/**
 * Runs when all the pending tests have finished.
 * To add some custom behavior this method should be overridden.
 * @param {Array.<ResultGroup>} results The results generated by the tests.
 */
TestRunner.prototype.onAllFinish = function(results) { };

/**
 * Runs the tests.
 */
TestRunner.prototype.run = function() {
  var runner = new Helper.FunctionRunner();
  runner.add(this.setupTests, [runner.getSequencer()], this);
  runner.add(this.executeTests, [], this);
  runner.run();
};

/**
 * Gets a function that is bound to this testRunner and therefore can be used
 * out of this context to run the tests.
 * @return {function} The bound function that can be used to run the tests.
 */
TestRunner.prototype.getRun = function() {
  return this.run.bind(this);
};

/**
 * Executes all the tests in the suite.
 */
TestRunner.prototype.executeTests = function() {
  var validator = new Validator(this.getContext(), this.testSuite);
  validator.goldenResults = this.getGoldenResults();
  validator.testFinishFunction = this.onTestFinish.bind(this);
  validator.runManualTest = this.runManualTest;
  validator.runTests(this.onAllFinish.bind(this));
};

/**
 * Clears the test suite array and adds only the specified test to it.
 * @param {String} testId Id of the test to add.
 */
TestRunner.prototype.setSingleTestAsSuite = function(testId) {
  var testSuite = this.testSuite.tests;
  for (var i = 0; i < testSuite.length; i++) {
    if (testSuite[i].id == testId) {
      this.testSuite.tests = [];
      this.testSuite.tests.push(testSuite[i]);
      return;
    }
  }
};

/**
 * Re-runs only the specified test.
 * @param {String} testId Id of the test to re-run.
 */
TestRunner.prototype.reRunTest = function(testId) {
  this.setSingleTestAsDirectory(testId);
  this.runManualTest = true;
  this.run();
};