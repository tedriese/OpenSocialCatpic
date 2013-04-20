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
 * @fileoverview gadgets.util.* API tests.
 */
function UtilSuite() {
  this.name = 'gadgets.util.* Test Suite';
  this.id = 'UTIL';
  this.tests = [

   { name: 'gadgets.util.escapeString(String) String = " "<>\'\n\r\\  "',
     id: 'UTIL000',
     description: 'Test the gadgets.util escapeString(String) capabilities ' +
                  'with the potentially dangerous symbols \'" "<>\'\n\r\\  "\'',
     priority: Test.PRIORITY.P0,
     run: function(context, callback, result) {
       var expected = '&#34;&#60;&#62;&#39;&#10;&#13;&#92;';
       var escaped = gadgets.util.escapeString('"<>\'\n\r\\');
       result.setResult(Assert.assertEquals(escaped, expected), escaped,
           expected);
       callback(result);
     }
   },

   { name: 'gadgets.util.unescapeString(String) String="&#34;&#60;&#62;&#39;'+
         +'&#10;&#13;&#92;"',
     id: 'UTIL001',
     description: 'Test the gadgets.util unescapeString(String) capabilities ' +
                  'String=\'"&#34;&#60;&#62;&#39\' unescaping a String which ' +
                  'includes escaped dangerous symbols',
     priority: Test.PRIORITY.P0,
     run: function(context, callback, result) {
       var expected = '"<>\'\n\r\\';
       var unescaped = gadgets.util.unescapeString('&#34;&#60;&#62;&#39;&#10;'
           +'&#13;&#92;');
       result.setResult(Assert.assertEquals(unescaped, expected), unescaped,
           expected);
       callback(result);
     }
   },

   { name: 'gadgets.util.unescapeString(String) String="&#34;&#60;&#62;&#39;'+
         + '&#10;&#13;&#92" vs gadgets.util.escapeString'
         + ' String = " "<>\'\n\r\\"',
     id: 'UTIL002',
     description: 'Test the gadgets.util unescapeString(String) and ' +
                  'escapeString compatibility by escaping and then ' +
                  'unescaping a String and comparing it again',
     priority: Test.PRIORITY.P0,
     run: function(context, callback, result) {
       var original= '"<>\'\n\r\\';
       var escaped = gadgets.util.escapeString(original);
       var unescaped = gadgets.util.unescapeString(escaped);
       result.setResult(Assert.assertEquals(original, unescaped), original,
           unescaped);
       callback(result);
     }
   },

   { name: 'gadgets.util.hasFeature("required features")',
     id: 'UTIL003',
     description: 'Test the gadgets hasFeature capabilities with a known and ' +
                  'available feature.',
     priority: Test.PRIORITY.P0,
     run: function(context, callback, result) {
       var list = new Array(
           'setprefs', 'opensocial-0.7', 'opensocial-0.8', 'dynamic-height');
       for (var gadget in list){
         Helper.addSubResult(result,
             'gadgets.util.hasFeature('+list[gadget]+')', Assert.assertTrue,
             gadgets.util.hasFeature(list[gadget]), true, Result.severity.INFO);
       }
       callback(result);
     }
   },

   { name: 'gadgets.util.hasFeature(notSupportedFeature)',
     id: 'UTIL004',
     description: 'Test gadgets.util.hasFeature returns false for feature' +
                  ' that does not exists',
       bugs: ['1025579'],
       priority: Test.PRIORITY.P0,
       run: function(context, callback, result) {
         // Test a feature that isn't supposed to exist.
         Helper.addSubResult(result,
             'gadgets.util.hasFeature(nonexist4ntf3atur3nog0)',
             Assert.assertFalse,
             gadgets.util.hasFeature('nonexist4ntf3atur3nog0'), 'False')
         callback(result);
     }
    },

   { name: 'gadgets.util.hasFeature() - Empty case',
     id: 'UTIL005',
     description: 'ERROR HANDLING CASE: gadgets.util.hasFeature() without ' +
                  'parameters. Checks how the container handles a malformed ' +
                  'invoke on a method ideally it should return false or avoid' +
                  ' exceptions by returning null or undefined.',
     priority: Test.PRIORITY.P2,
       run: function(context, callback, result) {
         Helper.addSubResult(result, 'gadgets.util.hasFeature()',
             Assert.assertFalse, gadgets.util.hasFeature())
         callback(result);
     }
   },

   { name: 'gadgets.util.getFeatureParameters(Existing Feature that does not'
         + ' take parameters)',
     id: 'UTIL006',
     description: 'Test the gadgets.util getFeatureParameters capabilities ' +
                  'by setting as parameter a feature included in the app ' +
                  'which takes no parameters in its constructor An empty ' +
                  'object should be returned',
     priority: Test.PRIORITY.P2,
     run: function(context, callback, result) {
       if (gadgets.util.hasFeature("flash")) {
         var params = gadgets.util.getFeatureParameters("flash");
         result.setResult(Assert.assertEmptyMap(params), params, '{}');
       } else {
         Helper.addInfoSubResult(result, 'Not supprted', Assert.assertTrue,
             'Flash not supported', '');
       }
       callback(result);
       }
   },

   { name: 'gadgets.util.getFeatureParameters(Empty)',
     id: 'UTIL0007',
     description: 'Test the gadgets.util getFeatureParameters capabilities ' +
                  'by setting nothing as parameter Null should be returned',
     priority: Test.PRIORITY.P2,
     run: function(context, callback, result) {
       var expectedparams = null;
       var params = gadgets.util.getFeatureParameters();
       result.setResult(Assert.assertEquals(params, expectedparams), params,
           expectedparams);
       callback(result);
     }
   },

   { name: 'gadgets.util.getFeatureParameters(NonexistingFeature)',
     id: 'UTIL008',
     description: 'Test the gadgets.util getFeatureParameters capabilities ' +
                  'by setting a non-existing feature as parameter ' +
                  'Null should be returned',
     priority: Test.PRIORITY.P2,
     run: function(context, callback, result) {
       var expectedparams = null;
       var params = gadgets.util.getFeatureParameters("NonexistingFeature");
       result.setResult(Assert.assertEquals(params, expectedparams), params,
           expectedparams);
       callback(result);
     }
   },

   { name: 'gadgets.util.getUrlParameters() not null values ',
     id: 'UTIL009',
     description: 'Tests if getUrlParameters returns valid array of values',
     priority: Test.PRIORITY.P2,
     run: function(context, callback, result) {
       var params = gadgets.util.getUrlParameters();
       Helper.addSubResult(result, 'url is not null',
           Assert.assertNotNull, params['url'], 'defined');
       Helper.addSubResult(result, 'lang is not null',
           Assert.assertNotNull, params['lang'], 'defined');
       Helper.addSubResult(result, 'country is not null',
           Assert.assertNotNull, params['country'], 'defined');
       callback(result);
     }
   },

   { name: 'gadgets.util.getUrlParameters() correct URL ',
     id: 'UTIL010',
     description: 'Tests if getUrlParameters returns right URL',
     priority: Test.PRIORITY.P2,
     run: function(context, callback, result) {
       var params = gadgets.util.getUrlParameters();
       result.addSubResult('URL is', Assert.assertStringContains, params['url'],
           Config.rootPath);
       callback(result);
     }
   },

   { name: 'gadgets.util.makeclosure()',
     id: 'UTIL011',
     description: 'Checks if makeclosure calls closure method successfully.',
     priority: Test.PRIORITY.P0,
     run: function(context, callback, result) {
       // function that will become closure
       var myFunction = function(param1, param2){
         var answer = param1 + param2;
         result.setResult(Assert.assertEquals(8, answer), 'Succesful use of '
             +'the closure as a call back', 8);
         callback(result);
       }
       // variable named "myClosure" is the closure
       var myClosure = gadgets.util.makeClosure(null, myFunction, 3, 5);
       var url =
           Config.rootPath + 'suites/0.7/makerequest/content/htmlsample.html';
       // use this variable in a function that takes only callbacks
       gadgets.io.makeRequest(url, myClosure);
     }
   }
 ];
};