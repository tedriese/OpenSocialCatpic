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
 * @fileoverview Testing gadget controller. Provides a way to
 * include multiple javascript testing suites in a single gadget definition.
 */

var Master = {};

/**
 * This array contains the list of the suites to be run by the master gadget.
 * To add a suite object here, the corresponding js file should be
 * included in the master.xml file
 */
Master.suites = [
  new PeopleSuite0_8(),
  new ActivitySuite0_8(),
  new AppDataSuite0_8(),
  new IoSuite(),
  new PrefsSuite(),
  new UtilSuite(),
  new EnvironmentSuite(),
  new MakeRequestSuite(),
  new MiscSuite0_8()
];

var testRunner = null;
/**
 * Initiates the test execution.<br>
 * Creates TestSuiteDirectory from suites array and pass it to testRunner.
 */
Master.start = function(opt_sequential) {
  var complianceSuiteDirectory =
      new TestSuiteDirectory('CMP0.8', 'Compliance Tests OpenSocial 0.8');
  complianceSuiteDirectory.suites = Master.suites;
  testRunner = new OpenSocialTestRunner(
      complianceSuiteDirectory, opt_sequential);
  testRunner.skipSetupTests = true;
  testRunner.run();
};
