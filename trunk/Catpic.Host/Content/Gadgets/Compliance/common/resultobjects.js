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
 * @fileoverview
 * Contains classes and methods to generate a Results and ResultSets.
 * <ul>
 * <li>Create a TestSuite object and add Test objects to its tests array or
 * using its methods.</li>
 * <li>Create an anonymous or custom class containing the same properties of the
 * TestSuite and Test classes.</li>
 * <ul>
 */

/**
 * Result set for particular suite. It contains all the results for a suite.
 * It's kind of mapping with TestSuite.tests
 *
 * @param {string} suiteId id of the suite
 * @param {string} suiteName name of the suite
 * @param {number} length of the suite
 * @constructor
 */
function ResultSet(suiteId, suiteName) {
  /**
   * The id of the suite whose results ResultSet stores
   * @type {string}
   */
  this.id = suiteId;

  /**
   * Name of the suite whose results ResultSet stores
   */
  this.name = suiteName;

  /**
   * Results list
   * @type {Array.ResultGroup}
   */
  this.results = null;

  /**
   * Start time of the suite
   * @type {number}
   */
  this.starttime = new Date().getTime();

  /**
   * Finish time for suite execution
   * @type {number}
   */
  this.finishtime = null;

  /**
   * severity of the set
   * @type {Result.severity}
   */
  this.severity = null;

  /**
   * Length of the suite. We will use this information to determine if result
   * is last one in the suite.
   * @type {number}
   */
  this.suiteLength = null;
}

// Namespace
var Result = { }

/**
 * Enum for severity values.
 * @enum {number}
 */
Result.severity = {
  /**
   * OFF is a Special level that can be used to turn off logging
   */
  OFF: -1,

  /**
   * FINEST indicates a highly detailed tracing message.
   */
  FINEST: 0,

  /**
   * FINER indicates a fairly detailed tracing message.
   */
  FINER: 1,

  /**
   * FINE is a message level providing tracing information.
   */
  FINE: 2,

  /**
   * INFO is a severity level for informational messages.
   */
  INFO : 3,

  /**
   * UNVERIFIED indicates test didn't perform any check.
   * i.e. Test exited because feature is not supported or test is manual.
   */
  UNVERIFIED : 4,

  /**
   * PASS indicates test performed exactly as designed
   */
  PASS : 5,

  /**
   * WARNING indicates test behaved as designed but didn't get
   * exact message or value for undefined area.<br>
   * i.e. exact error message on failure
   */
  WARNING : 6,

  /**
   * FAIL indicates test failed in verification
   */
  FAIL : 7,

  /**
   * Special level that can be used to turn off logging
   */
  ALL: 99,

  /**
   * String equivalent of
   */
  string_ : [ 'FINEST', 'FINER', 'FINE', 'INFO', 'UNVERIFIED',
              'PASS', 'WARNING', 'FAILED' ]
};

/**
 * Returns string equivalent for the string object
 * @param severity
 */
Result.severity.getString = function(severity) {
  return Result.severity.string_[severity] || 'INFO';
}

/**
 * Converts a ResultGroup or Array of ResultGroups to simple objects, this
 * objects only contains information useful for logging leaving outside their
 * meta-data information.<br>
 * The resulting object(s) are specially useful for JSON conversions since they
 * only hold relevant logging data that can be used latter.
 * @param {ResultGroup, Array.<ResultGroup>} results The ResultGroup or Array of
 *     ResultGroup objects to convert.
 * @return {object, Array.<object>} a simple object that contains only logging
 *     data or an array of converted results to simple objects.
 */
Result.convertToSimple = function(results) {
  if (results instanceof Array) {
    var simplifiedResults = [];
    for (var i = 0; i < results.length; i++) {
      simplifiedResults.push(results[i].getSimple());
    }
    return simplifiedResults;
  } else {
    return results.getSimple();
  }
};

/**
 * A Result object that's timed (i.e. contains start/end time as well as
 * a list of verification items that linked off from a test.  This is similar
 * to a test group, and it's used to chained together related verifications of
 * a given test.  In our case, it's used to describe the test result pertaining
 * to one data request/response.
 * @param {string} name name of the test.
 * @param {number} opt_severity the severity of the test.
 * @see {Result.severity}
 * @constructor
 */
function ResultGroup(test) {
  /**
   * The name of the test that created this result.
   * @type {string}
   */
  this.name = test.name;

  /**
   * The id of the test that created this result.
   * @type {string}
   */
  this.id = test.id;

  /**
   * The bugs assigned to the test that created this result.
   * @type {Array.<string>}
   */
  this.bugs = test.bugs;

  /**
   * The bugs assigned to the test that created this result.
   * @type {Array.<string>}
   */
  this.tags = test.tags;

  /**
   * The test object that created this result.
   * @type {object}
   */
  this.test = test;

  /**
   * Ending severity of this Result. null means not setted, undefined means
   * unverified, false means failed, true means success.
   * @type {(null, undefined, boolean)}
   */
//  this.success = null;

  /**
   * Start time of the test.
   * @type {number}
   */
  this.startTime = new Date().getTime();

  /**
   * Finish time of the test.
   * @type {(number, null)}
   */
  this.finishtime = null;

  /**
   * list of NonTimeResults representing the individual verifications.
   * @type {Array.<ResultValidations>}
   */
  this.verifications = [];

  /**
   * The severity of the result, used for logging and determine overall success.
   * @type {number}
   * @see {Result.severity}
   */
  this.severity = Result.severity.PASS;

  /**
   * A flag indicating that the result contains at least one subtest that
   * contains a warning
   */
  this.hasWarning = false

  /**
   * Flag that indicates that there is a delta severity for the verification.
   * @type {boolean}
   */
  this.delta = false;

  // If test case is given we add this result to it to have a 2 way
  // relationship beetween the two objects
  if (test) {
    test.result = this;
  }

  /**
   * Contains the result severity
   */
  this.severity = null;

  /**
   * Specifies the suite it belongs to. This parameter is used by validator
   * to identify the suite of the tests that returns after execution.
   */
  this.suite = null;

  /**
   * Tells if this is the last result of the suite. 
   */
  this.suiteDone = null;
}


/**
 * This is used for adding individual verifications to the current test group.
 * @param {ResultValidation} result the result item to be added to the
 * verifications array.
 */
ResultGroup.prototype.add = function(result) {
  if (result instanceof Array) {
    for (var i = 0; i < result.length; i++){
      this.add(result[i]);
    }
  } else {
    if (!result.id) {
      result.id = this.id + '.' + this.verifications.length;
    }
    this.verifications.push(result);
  }
};

/**
 * Adds a sub-result to this ResultGroup.
 * @param {string} text The text (name) of the sub-result
 * @param {function, boolean, null, undefined} assert The function to be used
 *     for assertions, the result as a boolean for the sub-result or null,
 *     undefined to specify an unverified test.
 * @param {*} actual The actual value obtained in the test.
 * @param {*} expected The expected value of the test.
 * @param {Result.severity} opt_severity The severity of the assertion of this
 *     result.
 */
ResultGroup.prototype.addSubResult = function(text, assert, actual
    , expected, opt_severity) {
  var severity = opt_severity;

  // highest secerity is warning for P3 and P4
  if (this.test.priority == Test.PRIORITY.P3 || this.test.priority == Test.PRIORITY.P4) {
    severity = Result.severity.WARNING;
  }
  var subtest = new ResultValidation(text, severity);
  if (typeof(assert) == 'function') {
    subtest.setResult(assert(actual, expected), actual, expected);
  } else if (typeof(assert) == 'string' && typeof(eval(assert)) == 'function') {
    var assertfunc = eval(assert);
    subtest.setResult(assertfunc(actual, expected), actual, expected);
  } else {
    subtest.setResult(assert, actual, expected);
  }
  this.add(subtest);
};

/**
 * Gets the global result for a test case.
 * @return {Result.severity}
 */
ResultGroup.prototype.getGlobalResult = function() {
  return this.severity;
};

/**
 * Sets the result on this ResultGroup and also sets the delta severity of the
 * subResults as well only if a golden result is given.
 * In order to clear the delta severity an empty object should be sent as the
 * opt_goldenResult parameter, else the delta severity will be kept.
 * @param {*} opt_goldenResult The golden result to compare the subResults to.
 */
ResultGroup.prototype.autoSetResult = function(opt_goldenResult) {
  var goldenResultEntry = null;
  if (opt_goldenResult) {
    var suite = opt_goldenResult[this.suite];
    if (suite != null) {
      goldenResultEntry = suite[this.id];
    }
  }
  var hasValidSubResults = false;
  for (var i = 0; i < this.verifications.length; i++) {
    if (goldenResultEntry && goldenResultEntry.verifications) {
      var goldenResult = goldenResultEntry.verifications[i];
      var verificationResult = this.verifications[i].success || false;
      if (goldenResult == undefined) {
        this.verifications[i].delta = true;
      } else {
        this.verifications[i].delta = typeof(verificationResult) == 'boolean'
            && typeof(goldenResult) == 'boolean'
            && verificationResult != goldenResult;
      }
    } else if (opt_goldenResult) {
      this.verifications[i].delta = false;
    }
    this.delta = this.delta || this.verifications[i].delta;
    if (this.verifications[i].severity >= Result.severity.WARNING) {
      hasValidSubResults = true;
    }
    if (this.verifications[i].severity > this.severity) {
      this.severity = this.verifications[i].severity;
    }
  }

  if (goldenResultEntry != null && goldenResultEntry.result) {
    this.delta = this.delta ||
        goldenResultEntry.result != Result.severity.getString(this.severity);
  }
};

/**
 * Helper method for setting the result.
 * @param {boolean} success boolean value that's set to true to indicate
 * successful, false to indicate failure, undefined to indicate
 * unknown/unverified result.
 * @param {Object} actual the actual result.
 * @param {Object} expected the expected result.
 * @param {number} time finish time (in ms) of the test.
 * @deprecated calling this method is equivalent to adding one sub-test to the
 * test so that approach is preferred
 */
ResultGroup.prototype.setResult = function(success, actual, expected, time) {
  this.addSubResult(this.name, success, actual, expected);
};

/**
 * Gets the totals for this ResultGroup.
 * The totals object obtained by this function contains the number of passed,
 * failed and unverified sub-results.
 * @return {ResultTotals} The totals object for this ResultGroup
 */
ResultGroup.prototype.getTotal = function() {
  var total = new ResultTotals();
  for (var i = 0; i < this.verifications.length; i++) {
    total.addResult(this.verifications[i]);
  }
  return total;
};

/**
 * Gets a simple representation of this ResultGroup without methods and
 * without the reference to the test object. The resulting object is a simple
 * clone with functions and meta-data stripped that might be used to output
 * results, json, or serialization.
 * @return {object} A simple version of this ResultGroup
 */
ResultGroup.prototype.getSimple = function() {
  var simpleResult = {}
  simpleResult.name = this.name;
  simpleResult.id = this.id;
  simpleResult.bugs = this.bugs;
  simpleResult.tags = this.tags;
  simpleResult.time = this.finishTime - this.startTime;
  simpleResult.verifications = [];
  simpleResult.severity = Result.severity.getString(this.severity);
  this.hasWarning = this.hasWarning;

  for (var i = 0; i < this.verifications.length; i++) {
    simpleResult.verifications.push(this.verifications[i].getSimple());
  }
  return simpleResult;
};

/**
 * A result object that doesn't have the timing info.  It represents an
 * individual item in the ResultGroup's verification's array.
 * @param {string} name name of the test or verification item.
 * @param {number} opt_severity the severity of the test.
 * @see {Result.severity}
 * @constructor
 */
function ResultValidation(name, opt_severity) {
  /**
   * The name of the result.
   * @type {string}
   */
  this.success = true;

  /**
   * The expected value of the result.
   * @type {null, undefined, Object}
   */
  this.name = name;

  /**
   * The id of the result, it is set when added to a ResultGroup.
   * @type {null, undefined, Object}
   */
  this.id = null;

  /**
   * The actual result of the test, used for logging.
   * @type {null, undefined, Object}
   */
  this.actual = null;

  /**
   * a flag indicating if the test succeded or not.
   * @type {boolean}
   */
  this.expected = null; // expected value - for logging purposes

  /**
   * The severity of the result.
   * @type {number}
   */
  this.severity = opt_severity ? opt_severity : null;

  /**
   * Flag that indicates that there is a delta severity for the verification.
   * @type {boolean}
   */
  this.delta = false;
}

/**
 * Gets a simple representation of this ResultValidation without methods and
 * without the reference to the test object. The resulting object is a simple
 * clone with functions and meta-data stripped that might be used to output
 * results, json, or serialization.
 * @return {object} A simple version of this ResultValidation
 */
ResultValidation.prototype.getSimple = function() {
  var result = {};
  result.success = this.success;
  result.name = this.name;
  result.id = this.id;
  result.severity = Result.severity.getString(this.severity);
  result.delta = this.delta;

  if (this.actual instanceof Array) {
    result.actual = '[ARRAY]';
  } else if (typeof(this.actual) == 'function') {
    result.actual = 'FUNCTION ' + this.actual.name;
  } else if (typeof(this.actual) == 'object') {
    result.actual = 'OBJECT';
  } else {
    result.actual = '' + this.actual;
    if (result.actual.length > 100) {
      result.actual = result.actual.substr(0, 100) + '...';
    }
  }
  if (this.expected instanceof Array) {
    result.expected = '[ARRAY]';
  } else if (typeof(this.expected) == 'function') {
    result.expected = 'FUNCTION ' + this.expected.name;
  } else if (typeof(this.actual) == 'object') {
      result.expected = 'OBJECT';
  } else {
    result.expected = '' + this.expected;
    if (result.expected.length > 100) {
      result.expected = result.expected.substr(0, 100) + '...';
    }
  }
  return result;
};

/**
 * Helper method for setting the result.
 * @param {boolean} success boolean value that's set to true to indicate
 * successful, false to indicate failure, undefined to indicate
 * unknown/unverified result.
 * @param {Object} actual the actual result.
 * @param {Object} expected the expected result.
 */
ResultValidation.prototype.setResult = function(success, actual, expected) {
  if (success) {
    this.severity = (this.severity && this.severity != Result.severity.PASS) ?
                    Result.severity.INFO : Result.severity.PASS;
  } else {
    this.severity = (this.severity) ? this.severity : Result.severity.FAIL;
  }
  this.success = success;
  this.actual = (actual != undefined) ? actual : 'undefined';
  if (!success) {
    this.expected = (expected != undefined) ? expected : 'undefined';
  }
};

/**
 * ResultTotals constructor. A class that contains the number and id's of the
 * results stored by final severity. This class holds a count of how many tests
 * passed, failed, had warnings and were unverified as well as its id's
 * Additionally it has information on the maximum time taken for all the tests,
 * the less time taken and the total time of all the tests implied.
 * @constructor
 */
function ResultTotals() {
  /**
   * The count of passed results.
   * @type{number}
   */
  this.passed = 0;

  /**
   * The count of failure results.
   * @type{number}
   */
  this.failed = 0;

  /**
   * The count of unverified tests.
   * @type{number}
   */
  this.unverified = 0;

  /**
   * The count of warnings.
   * @type{number}
   */
  this.warnings = 0;

  /**
   * The total time the tests took.
   * @type{number, null}
   */
  this.totalTime = null;

  /**
   * The worst (maximum) time of a single test in the entire suite.
   * @type{number, null}
   */
  this.maxTime = null;

  /**
   * The best time (minimum) time of a single test in the entire suite.
   * @type{number, null}
   */
  this.minTime = null;

  /**
   * An array of ids of all the tests that failed.
   * @type{Array.<string>}
   */
  this.failedDetail = [];

  /**
   * An array of ids of all the tests that had warnings.
   * @type{Array.<string>}
   */
  this.warningsDetail = [];

  /**
   * An array ids of all the tests that were unverified.
   * @type{Array.<string>}
   */
  this.unverifiedDetail = [];

  /**
   * An array of ids of all the manual tests.
   * @type{Array.<string>}
   */
  this.manualDetail = [];
}

/**
 * Add a result to the count storing its ID in its respective array.
 * @param {ResultValidation} result The result to add to the count.
 */
ResultTotals.prototype.addResult = function(result) {
  if (result.severity < Result.severity.INFO) {
    if (result.success == null || result.success == undefined ||
        result.severity == Result.severity.UNVERIFIED) {
      this.unverified++;
      this.unverifiedDetail.push(result.id);
    } else {
      if (result.success) {
        this.passed++;
      } else {
        if (result.severity == Result.severity.WARNING) {
          this.warnings++;
          this.warningsDetail.push(result.id);
        } else {
          this.failed++;
          this.failedDetail.push(result.id);
        }
      }
    }
  }
};

/**
 * Adds a ResultGroup to the count inside this ResultTotals, this function is
 * used with ResultGroups that have ResultGroups inside, it will then take the
 * ResultValidation and put its ID inside the correct array and add it to the
 * correct count.<br>
 * Note that this only adds once to the count, it will not add the sub-results
 * to the count or to the arrays.In other words: <b>Children are not added.</b>
 * @param {ResultGroup} result The ResultGroup to add to the count;
 */
ResultTotals.prototype.addResultGroup = function(result) {
  if (result.startTime) {
    var time = (result.finishTime - result.startTime);
    if (this.maxTime == null || this.maxTime < time) {
      this.maxTime = time;
    }
    if (this.minTime == null || this.minTime > time) {
      this.minTime = time;
    }
    if (this.totalTime == null) {
      this.totalTime = 0;
    }
    this.totalTime += time;
    if (result.test && result.test.manual) {
      this.manualDetail.push(result.id);
    }
    if (result.success == null) {
      result.autoSetResult();
    }
    if (result.success == null) {
      this.unverified++;
      this.unverifiedDetail.push(result.id);
    } else {
      if (result.success) {
        if (result.hasWarning) {
          this.warnings++;
          this.warningsDetail.push(result.id);
        }
        this.passed++;
      } else {
        this.failed++;
        this.failedDetail.push(result.id);
      }
    }
  } else {
    this.addResult(result);
  }
};

/**
 * Add a ResultTotals to this total adding all its numerical counts only.
 * The optional resultId is used if we want to add something to the arrays, if
 * we want to also modify the arrays the optional parameter will be added to the
 * arrays only if the fail count or the warning count of the added total is
 * greater than 0.
 * @param {ResultTotals} total The total to add to this count.
 * @param {string} opt_resultId An optional id to add to the arrays.
 */
ResultTotals.prototype.addTotal = function(total, opt_resultId) {
  this.passed += total.passed;
  this.failed += total.failed;
  this.unverified += total.unverified;
  this.warnings += total.warnings;
  if (opt_resultId) {
    if (total.failed > 0) {
      this.failedDetail.push(opt_resultId);
    }
    if (total.warnings > 0) {
      this.warningsDetail.push(opt_resultId);
    }
  }
};

/**
 * Gets the total count of verifications inside this total.
 * @return {number} The total count.
 */
ResultTotals.prototype.getTotal = function() {
  return this.passed + this.failed + this.unverified + this.warnings;
};
