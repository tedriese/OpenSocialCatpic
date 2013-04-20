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
 * @fileoverview Navigation, request and permission tests.
 */
function NavigationSuite() {
  this.name = 'Navigation Test Suite';
  this.id = 'NAV';
  var currentView = gadgets.views.getCurrentView().getName();
  this.tests = [
    { name: 'gadgets.views.getParams() - Use this to test if params are ' +
            'propagated.',
      id : 'GVP000',
      priority: Test.PRIORITY.P0,
      description: 'This suite is collection of autoamtically generated ' +
                   'manual tests. Use this test to see if parameters are ' +
                   'propagated successfully betwen navigation from one view' +
                   'to another. Test name reflect what is being tested. i.e.' +
                   'NAV-PROFILE-DASHBOARD suggests navigation from profile ' +
                   'view (typically current view for gadget) to dashboard view',
      run: function(context, callback, result) {
        var params = gadgets.views.getParams();
        Helper.addSubResult(result, 'gadgets.views.getParams() must be defined',
            Assert.assertDefined, params, 'defined');
        Helper.addSubResult(result,
            'gadgets.views.getParams() must not be null', Assert.assertNotNull,
            params, 'not null');
        if (params) {
          for (var paramName in params) {
            Helper.logIntoResult(result, paramName, params[paramName]);
          }
        }
        NavigationSuite.getViewInfoClosure(result)
        callback(result);
      }
    },

    /**
     * API for navigation between views:
     * gadgets.views.requestNavigateTo(...)
     */

    { name: 'gadgets.views.requestNavigateTo() -  empty view',
      id : 'NAV-' + currentView + '-[empty]',
      manual: true,
      run: function(context, callback, result) {
        var error = null;
        try {
          gadgets.views.requestNavigateTo();
        } catch (ex) {
          error = ex;
        }
        result.setResult(Assert.assertNotNull(error), 'Exception',
              'Exception thrown');
        callback(result);
      }
    },

    { name: 'gadgets.views.requestNavigateTo(null) -  invalid view',
      id : 'NAV-' + currentView + '-[null]',
      manual: true,
      run: function(context, callback, result) {
        var error = null;
        try {
          gadgets.views.requestNavigateTo(null);
        } catch (ex) {
          error = ex;
        }
        result.setResult(Assert.assertNotNull(error), 'Exception',
              'Exception thrown');
        callback(result);
      }
    }
  ];

  var views = gadgets.views.getSupportedViews();
  var parameterOptions = [];
  parameterOptions[0] = {
    userName : 'testuser',
    userData : 'data'
  };
  parameterOptions[1] = {
    userName : 'testuser',
    userData : 'friends&family'
  };
  parameterOptions[2] = {
    userName : 'testuser',
    userData : 'What?now'
  };
  parameterOptions[3] = {
    userName : 'testuser',
    userData : 'some spaced parameter that should be parsed correctly'
  };
  for (var viewid in views) {
    var testNumber = 1;
    var test = {};
    test['view'] = views[viewid];
    test['name']
        = 'gadgets.views.requestNavigateTo(' + viewid + ') No parameters';
    test['id'] = 'NVP-' + currentView + '-' + viewid + '-'
        + Helper.padZeros(testNumber, 3);
    test['manual'] = true;
    test['run'] = function(context, callback, result) {
      gadgets.views.requestNavigateTo(this.view);
      NavigationSuite.getViewInfoClosure(result);
      callback(result);
    }
    this.tests.push(test);
    testNumber++;
    test = {};
    test['view'] = views[viewid];
    test['name'] = 'gadgets.views.requestNavigateTo(' + viewid + ', params)';
    test['id'] = 'NVP-' + currentView + '-' + viewid + '-'
        + Helper.padZeros(testNumber, 3);
    test['manual'] = true;
    test['run'] = function(context, callback, result) {
      var views = gadgets.views.getSupportedViews();
      var currentViewName = gadgets.views.getCurrentView().getName();
      var params = {comingFromTest: this.id, comingFromView: currentViewName};
      gadgets.views.requestNavigateTo(this.view, params);
      NavigationSuite.getViewInfoClosure(result);
      callback(result);
    }
    this.tests.push(test);
    testNumber++;
    for (var i = 0, param; param = parameterOptions[i]; i++) {
      test = {};
      test['view'] = views[viewid];
      test['params'] = param;
      test['name'] = 'gadgets.views.requestNavigateTo(' + viewid + ','
          + gadgets.json.stringify(param) + ')';
      test['id'] = 'NVP-' + currentView + '-' + viewid + '-'
          + Helper.padZeros(testNumber, 3);
      test['manual'] = true;
      test['run'] = function(context, callback, result) {
        gadgets.views.requestNavigateTo(this.view, this.params);
        NavigationSuite.getViewInfoClosure(result);
        callback(result);
      }
      this.tests.push(test);
      testNumber++;
    }
  }
};


NavigationSuite.getViewInfoClosure = function(result) {
  var currentViewName = gadgets.views.getCurrentView().getName();
  Helper.logIntoResult(result, 'Current View', currentViewName);
  var views = gadgets.views.getSupportedViews();
  Helper.logIntoResult(result, 'Supported Views',
      gadgets.json.stringify(views));
  var params = gadgets.views.getParams();
  Helper.logIntoResult(result, 'Params', gadgets.json.stringify(params));
}



