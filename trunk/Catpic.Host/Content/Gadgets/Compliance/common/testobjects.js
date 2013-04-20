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
 * Contains classes and methods to generate a TestSuite and Tests inside.
 * This file documents and extends the functionality of this classes.
 * In order to create a test 2 approaches can be taken:
 * <ul>
 * <li>Create a TestSuite object and add Test objects to its tests array or
 * using its methods.</li>
 * <li>Create an anonymous or custom class containing the same properties of the
 * TestSuite and Test classes.</li>
 * <ul>
 */

/**
 * The testSuiteDirectory is collection of test suites to be tested against.
 * Specifically used when you want to execute multiple suites in single gadget
 * @param {string} id of directory
 * @param {string} name of directory
 * @constructor
 */
function TestSuiteDirectory(id, name) {

  /**
   * The Id of the suite directory
   * @type {String}
   */
  this.id = id;

  /**
   * The name of the suite directory
   * @type {String}
   */
  this.name = name;

  /**
   * The suites that belong to this direcotry
   * @type {Array.<TestSuite>}
   */
  this.suites = [];
}

/**
 * The testSuite that contains the set of tests to be tested against.
 * @param {string} id the id of the suite.
 * @param {string} name the name of the suite.
 * @constructor
 */
function TestSuite(id, name) {

  /**
   * the Id of the suite
   * @type {string}
   */
  this.id = id;

  /**
   * The name of the suite
   * @type {string}
   */
  this.name = name;

  /**
   * A method that will run only once before the test suite is run.
   * @type {null, function(Context)}
   */
  this.suiteSetup = null;

  /**
   * A method that will run only once after the entire suite has run.
   * @type {null, function(Context, Array.<ResultGroups>)}
   */
  this.suiteTearDown = null;

  /**
   * The tests that belongs to this suite.
   * @type {Array.<Test>}
   */
  this.tests = [];
}

/**
 * Adds a new test to the suite
 * @param {string} id the id of the test.
 * @param {string} name the name of the test.
 * @param {Object} opt_params optional parameters that will be used to construct
 *     this test. It is an object composed by any of the attributes of this test
 *     object.
 * @return {Test} the test object that has been added to the suite.
 */
TestSuite.prototype.addNewTest = function(id, name, opt_params) {
  return this.addTest(new Test(id, name, opt_params));
};

/**
 * A test object containing all test information.
 * @param {Test} test The test to be added.
 * @return {Test} the test object that has been added to the suite.
 */
TestSuite.prototype.addTest = function(test){
  for (var i = 0; i < this.tests.length; i++) {
    if (this.tests[i].id == test.id){
      throw 'CONSTRAINT ERROR: id ' + test.id + 'NOT UNIQUE IN SUITE';
    }
  }
  this.tests.push(test);
  return test;
};

/**
 * A test object containing all test information.
 * @param {string} id the id of the test.
 * @param {string} name the name of the test.
 * @param {Object} opt_params optional parameters that will be used to construct
 *     this test. It is an object composed by any of the attributes of this test
 *     object.
 * @constructor
 */
function Test(id, name, opt_params) {
  /**
   * The id of the test
   * @type {string}
   */
  this.id = id;

  /**
   * The name of the test
   * @type {string}
   */
  this.name = name;

  /**
   * The main function of this test
   * @type {(null, function(ResultGroup, object, function()) : boolean)}
   */
  this.run = opt_params && opt_params.run || opt_params.run;

  /**
   * The description of the test
   * @type {string}
   */
  this.description =
      opt_params && opt_params.description || opt_params.description;

  /**
   * The number of milliseconds that the run function will have to finish before
   * it has a timeout error. by default is 60000 milliseconds (1 minute)
   * @type {number}
   */
  this.timeout = (opt_params && opt_params.timeout) ? opt_params.timeout :
      Config.defaultTimeout;

  /**
   * The testing queue this test belong to, by default is 6.
   * @type {number}
   */
  this.queue = (opt_params && opt_params.queue) ? opt_params.queue : 6;

  /**
   * The tags this test has.
   * @type {Array.<String>}
   */
  this.tags = (opt_params && opt_params.tags) ? opt_params.tags : [];

  /**
   * The bugs associated to this test.
   * @type {Array.<String>}
   */
  this.bugs = (opt_params && opt_params.bugs) ? opt_params.bugs : [];

  /**
   * A flag indicating the test is manual and should not be executed
   * automatically.
   * @type {boolean}
   */
  this.manual = (opt_params && opt_params.manual) ? opt_params.manual : false;

  /**
   * Specifies a priority for the test.
   * @type {Test.PRIORITY}
   */
  this.priority = Test.PRIORITY.P2;

  /**
   * Aditional user data should be stored here.
   * @type {object}
   */
  this.data = {};
}

/**
 * Enumeration for the priority of the tests. Allows the user to establish a
 * priority in a test by test basis that can be used for logging or sorting the
 * results.
 */
Test.PRIORITY = {
  /**
   * Highest priority
   */
  P0: 'P0',

  /**
   * High Priority
   */
  P1: 'P1',

  /**
   * Normal Priority
   */
  P2: 'P2',

  /**
   * Low Priority
   */
  P3: 'P3',

  /**
   * Lowest priority
   */
  P4: 'P4'
};

