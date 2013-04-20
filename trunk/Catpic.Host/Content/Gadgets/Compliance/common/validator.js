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
 * @fileoverview Provides the infrastructure for running the tests. It loops
 * through the tests inside a given test suite, and calls each tests run method.
 * Upon finishing, the validator calls TestRunner's renderTestOutput method
 * with the results.
 */

/**
 * Validator provides the basic infrastructure for running the tests inside a
 * given test suite directory and upon finishing, the validator will call
 * finishCallback with the results collected.
 * @param {Context} context a context object which is used to determine the data
 *     for verifying the tests.
 * @param {TestSuiteDirectory} testSuiteDirectory a testsuiteDirectory object
  *    that has an array of testSuite objects.
 * @constructor
 */
function Validator(context, testSuiteDirectory) {
  /**
   * The context object that will be passed to the tests inside the TestSuite.
   * @type {context}
   */
  this.context_ = context;

  /**
   * The test directory to run.
   * @type {TestSuiteDirectory}
   */
  this.testSuiteDirectory_ = testSuiteDirectory;

  /**
   * A counter for all the test that are run in particular suite, this counter
   * one reaches the amount of total tests in a suite will tell this validator
   * that test suite execution is complete and time to move to another suite.
   * @type {number}
   */
  this.progressCounter_ = 0;

  /**
   * A counter for the current queue of tests that are running, this counter
   * will go from 0 to the number of tests running in the current queue, when
   * it reaches its maximum value it tells this validator to run the next queue
   * and then this counter will reset to 0 allowing the count again now against
   * the new queue size.
   * @type {number}
   */
  this.partialCounter_ = 0;

  /**
   * Object that maps suite id to results 
   * @type Object
   */
  this.resultsMap_ = {};

  /**
   * Object to map suite id to ResultSet
   * @type Object
   */
  this.resultSetsMap_ = {};

  /**
   * The number of the current queue running, this value increases until all
   * the queues have run. This is an index that points to the current test queue
   * that is running for instance if this value is 5 it means that the fifth
   * queue of tests is running, when all its tests finishes (the partial counter
   * reaches its maximum value) this counter advances to the next queue (6) and
   * so on until it has been through all the queue's indices.
   * @type {number}
   */
  this.currentQueue_ = 0;

  /**
   * The function that will be invoked when all tests have run.
   * @type {function(Array.<ResultGroup>)}
   */
  this.finishCallback_ = null; // final function to print output

  /**
   * A pointer to the setInterval function that is launched to check for timeout
   * test time. This is used solely for the purpose of stopping the setInterval
   * function upon the end of the tests.
   * @type {*}
   */
  this.timeoutProcess_ = null;

  /**
   * A flag that indicates if manual tests should be run.
   * @type {boolean}
   */
  this.runManualTest = false;

  /**
   * The golden results for this validator to use to calculate deltas.
   * @type {object}
   */
  this.goldenResults = {};

  /**
   * A function that will be called when a test finishes.
   * @type {null, Function(result, test)}
   */
  this.testFinishFunction = null;

  /**
   * A function that will be called when a suite finishes.
   * @type {null, Function(result, test)}
   */
  this.suiteFinishFunction = null;

}


/**
 * Runs the tests inside the test suite directory.
 * @param {Function} callback the function to call once tests are finished.
 */
Validator.prototype.runTests = function(callback) {
  this.finishCallback_ = callback;

  this.testsToRun = [];
  this.testToRunCount = 0;
  var include = Config.include;
  var exclude = Config.exclude;

  var len = 0;
  // iterating over each suite
  for (var i = 0; i < this.testSuiteDirectory_.suites.length; i++) {
    var testSuite = this.testSuiteDirectory_.suites[i];
     // iterating over each test in a suite
    // prepare result suite where results will be stored
    this.resultsMap_[testSuite.id] = [];
    this.resultSetsMap_[testSuite.id] =
        new ResultSet(testSuite.id, testSuite.name);
    var testsToRunCount = 0;
    for (var j = 0; j < testSuite.tests.length; j++) {
      var tags;
      var test = null;
      if (testSuite.tests[j] && testSuite.tests[j].tags) {
        tags = testSuite.tests[j].tags;
      } else {
        tags = [];
      }
      // Look for tests to include.
      if (include != undefined && include.length > 0) {
        var k = 0;
        if (tags != undefined) {
          for (k = 0; k < tags.length; k++) {
            if (this.contains(include, tags[k])) {
              test = testSuite.tests[j];
              break;
            }
          }
        }
      } else {
        test = testSuite.tests[j];
      }

      // Look for tests to exclude.
      if (exclude != undefined && exclude.length > 0) {
        if (tags != undefined) {
          for (k = 0; k < tags.length; k++) {
            if (this.contains(exclude, tags[k])) {
              test = null;
              break;
            }
          }
        }
      }

      if (test) {
        var insertIndex = 5;
        if (test['queue']) {
          insertIndex = test['queue'];
        }
        if (!this.testsToRun[insertIndex]) {
          this.testsToRun[insertIndex] = [];
        }
        this.testsToRun[insertIndex].push(test);
        var result = new ResultGroup(test);
        result.suite = testSuite.id;
        test.result = result;
        this.progressCounter_++;
        testsToRunCount++;
      }
    }
    this.resultSetsMap_[testSuite.id].suiteLength = testsToRunCount;
    //var suiteToRun = new TestSuite(testSuite.id, testSuite.name);
    if (testSuite.suiteSetup) {
      this.suiteSetup.push(testSuite.suiteSetup);
    }
    if (testSuite.tearDown) {
      this.suiteTearDown(testSuite.suiteTearDown);
    }

  }
  if (this.suiteSetup && this.suiteSetup.length > 0) {
    for (var i ; i < this.suiteSetup.length; i++) {
      this.suiteSetup[i](this.context_);
    }
  }
  for (var cq = 0; cq < this.testsToRun.length; cq++) {
    if (this.testsToRun[cq] && this.testsToRun[cq].length > 0) {
      this.currentQueue_ = cq;
      break;
    }
  }
  this.timeoutProcess_ = setInterval('ValidatorUtils.checkTimeouts()', 5000);
  ValidatorUtils.validator_ = this;
  this.runQueue_();
};

/**
 * Runs all the test suites in queue. It will pop one suite at a time, initialize
 * monitoring parameters for the current queue and call runQueue_ for it.
 * @private
 */
Validator.prototype.runQueues_ = function() {
  var suiteToRun = this.suitesToRun.pop();
  if (suiteToRun != undefined) {
    this.testsToRun = suiteToRun.tests;
    for (var cq = 0; cq < this.testsToRun.length; cq++) {
      if (this.testsToRun[cq] && this.testsToRun[cq].length > 0) {
        this.currentQueue_ = cq;
        break;
      }
    }

    // initialize parameters befor starting execution
    this.timeoutProcess_ = setInterval('ValidatorUtils.checkTimeouts()', 5000);
    this.resultSet = new ResultSet(suiteToRun.id, suiteToRun.name);
    this.results_ = [];
    this.suiteSetup = suiteToRun.suiteSetup;
    this.suiteTearDown = suiteToRun.suiteTearDown;
    this.progressCounter_ = suiteToRun.testToRunCount;
    ValidatorUtils.validator_ = this;
    // here we go...
    this.runQueue_();
  }
}

/**
 * Runs the current queue (this.currentQueue) tests.
 * @private
 */
Validator.prototype.runQueue_ = function() {
  if (this.currentQueue_ >= this.testsToRun.length) {
    return;
  }
  var queue = this.testsToRun[this.currentQueue_];
  this.partialCounter_ = queue.length;
  for (var i = 0; i < queue.length; i++) {
    var test = queue[i];
    test.result['timeout'] = test['timeout'];
    if (!test.result['timeout']) {
      test.result['timeout'] = Config.defaultTimeout;
    }
    test.result['timeoutTime_'] = new Date().getTime() + test.result['timeout'];
    test.result['status'] = 'running';
    try {
      // Add auto-generated id if not provided.
      if (!test.id) {
        test.id = new Date().getTime() + ' (test id missing)';
      }
      // Initialize temporal test data container.
      test.data = {};

      // If test is marked as manual, do not run it unless runManualTest is on.
      if (test.manual && !this.runManualTest) {
        if (typeof(test.manual) == 'function') {
          test.manual(test.result);
        }
        test.result.addSubResult('Skipped. Manual test.', true, null, null,
            Result.severity.INFO);
        this.captureResult(test.result);
        continue;
      }
      // If a precondition is defined, call the function passing the
      // current context to it.
      if (test['precondition'] && !test.precondition(this.context_)) {
        test.result.addSubResult(
            'Skipped. Precondition returned false: <br>'
            + test.precondition.toString(), true, null, null,
            Result.severity.INFO);
        this.captureResult(test.result);
        continue;
      }
      // Run the test.
      ValidatorUtils.pendingResults_.push(test.result);
      test.run(this.context_, this.captureResult.bind(this), test.result);
    } catch (ex) {
      Helper.logException(test.result, test, ex);
      this.captureResult(test.result);
    }
  }
};

/**
 * <i>Utility method.</i><br>
 * Look for the value in the array.
 * @param {Array} elementArray of element.
 * @param {String} value to look for in the array.
 * @return {boolean} true if it contains the element, false otherwise
 */
Validator.prototype.contains = function(elementArray, value) {
  for (var i = 0; i < elementArray.length; i++){
    if (elementArray[i] == value) {
      return true;
    }
  }
  return false;
};

Validator.prototype.getResultSets = function() {
  var resultSets = [];
  for (var suite in this.resultSetsMap_) {
    resultSets.push(this.resultSetsMap_[suite]);
  }
  return resultSets;
}


/**
 * This method is called when each test finishes. It keeps track of the
 * progress and calls the final callback method with the results once all
 * the tests are run.
 * @param {ResultGroup} result the result of the current test that will be
 * appended to the final results array for final callback.
 */
Validator.prototype.captureResult = function(result) {
  if (result['status'] == 'finished') {
    return;
  }
  result['status'] = 'finished';
  result.finishtime = new Date().getTime();
  this.resultsMap_[result.suite].push(result);

  if (result.test && result.test.onFinish) {
    result.test.onFinish(result, result.test);
  }
  // Sets the result status and sub-result delta flag.
  result.autoSetResult(this.goldenResults);
  if (this.testFinishFunction) {
    this.testFinishFunction(result);
  }

  // check if this is the last result of the suite
  if (this.resultsMap_[result.suite].length ==
      this.resultSetsMap_[result.suite].suiteLength) {
    result.suiteDone = true;
    this.resultSetsMap_[result.suite].results = this.resultsMap_[result.suite];
    if (this.suiteFinishFunction) {
      this.resultSetsMap_[result.suite].finishtime = new Date().getTime();
      this.suiteFinishFunction([this.resultSetsMap_[result.suite]]);
    }
  }

  this.progressCounter_--;
  // check if current suite is done
  if (this.progressCounter_ == 0) {
    if (this.timeoutProcess_) {
      clearInterval(this.timeoutProcess_);
    }
    if (this.suiteTearDown && this.suiteTearDown.length > 0) {
      for (var i = 0; i < this.suiteTearDown.length; i++) {
        this.suiteTearDown[i](this.context_, this.results_);
      }
    }
    this.finishCallback_(this.getResultSets());
  }
  this.partialCounter_--;
  if (this.partialCounter_ == 0) {
    this.currentQueue_++;
    while (this.currentQueue_ < this.testsToRun.length &&
        (!this.testsToRun[this.currentQueue_] ||
         this.testsToRun[this.currentQueue_].length == 0)) {
      this.currentQueue_++;
    }
    this.runQueue_();
  }
};

/**
 * Static utility context, used to keep track of timeout functions.
 */
var ValidatorUtils = {
  /**
   * The results that are pending and doesn't have finished.
   */
  pendingResults_: [],

  /**
   * The validator to check the results that have finished and force the capture
   * result callback on those that exceed their timeout time.
   */
  validator_: null
};

/**
 * Function that checks on the tests that are in progress and checks for them
 * to be inside their timeout time, if they exceed this time they are terminated
 * adding a failure (timeout error) and calling the CaptureResult function on
 * the validator.<br>
 * This function is run using the setInterval method.
 * @this The static ValidatorUtils object
 */
ValidatorUtils.checkTimeouts = function() {
  if (this.pendingResults_.length > 0 && this.validator_) {
    var now = new Date().getTime();
    for (var result, i = 0; result = this.pendingResults_[i]; i++) {
      if (result['status'] == 'running' && result['timeoutTime_'] &&
          result['timeoutTime_'] < now) {
        result.addSubResult('TEST TIMED OUT AFTER ' + result['timeout'] +
            'ms', false, 'timeout', 'Should finish within timeout');
        this.validator_.captureResult(result);
      }
      if (result['status'] == 'finished') {
        this.pendingResults_.splice(i, 1);
      }
    }
  }
};
