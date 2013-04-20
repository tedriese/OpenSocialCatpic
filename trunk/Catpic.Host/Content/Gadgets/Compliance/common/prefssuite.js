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
 * @fileoverview gadget prefs tests
 */
function PrefsSuite() {
  this.name = 'gadget.Prefs Test Suite';
  this.id = 'PREF';
  this.tests = [
    { name: 'gadget.Prefs - module info',
      id : 'PREF000',
      bugs: ['1108776'],
      priority: Test.PRIORITY.P0,
      description: '<precondition> This test depends on container. It assumes' +
                   ' container has Language english and country as US ' +
                   '</precondition>',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyValue(result, 'getLang()', prefs.getLang(), 'en', Assert.assertNotNull);
        PrefsSuite.verifyValue(result, 'getCountry()', prefs.getCountry(),
            'US', Assert.assertNotNull);
        PrefsSuite.verifyValue(result, 'getModuleId()', prefs.getModuleId(),
            'not null', Assert.assertNotNull);
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getInt()',
      id : 'PREF010',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P0,
      description: 'Tests if getInt returns right value for integer preference',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyValue(result, 'getInt(myCounter)',
            prefs.getInt('myCounter'), 100);
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getInt() - non integer preferences as int',
      id : 'PREF011',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can get string, float, object, undefined ' +
                   'as integer. Checks if it throws any exception doing that.',
      run: function(context, callback, result) {
        // userPrefs
        var userPrefs = ['myString', 'myUndefined',
            'myBool', 'myTestArray', 'myObject', 'myFloat'];

        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        try {
          for (var i = 0; i < userPrefs.length; i++) {
            result.addSubResult('getInt(' + userPrefs[i] + ')',
                Assert.assertEquals, prefs.getInt(userPrefs[i]),
                prefs.getInt(userPrefs[i]));
          }
        } catch (ex) {
          result.addSubResult('getInt(' + userPrefs[i] + ')',
              Assert.assertFalse, ex.toString(), 'No Exception');
        }
        PrefsSuite.verifyValue(result, 'getInt()', prefs.getInt("size"),
            2);
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getFloat()',
      id : 'PREF020',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P0,
      description: 'Tests if getFloat returns right value for float preference',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyValue(result, 'getFloat(myFloat)',
            prefs.getFloat('myFloat'), 3.43);
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getFloat() - non float preferences as float',
      id : 'PREF021',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can get string, int, object, undefined,' +
                   'as Float. Checks if it throws any exception doing that.',
      run: function(context, callback, result) {
        // userPrefs
        var userPrefs = ['myString', 'myUndefined',
            'myBool', 'myTestArray', 'myObject', 'myFloat'];

        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        try {
          for (var i = 0; i < userPrefs.length; i++) {
            result.addSubResult('getFloat(' + userPrefs[i] + ')',
                Assert.assertEquals, prefs.getFloat(userPrefs[i]),
                prefs.getFloat(userPrefs[i]));
          }
        } catch (ex) {
          result.addSubResult('getFloat(' + userPrefs[i] + ')',
              Assert.assertFalse, ex.toString(), 'No Exception');
        }
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getString()',
      id : 'PREF030',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P0,
      description: 'Tests if getString returns right value for string' +
                   ' preference',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyValue(result, 'getString(myString)',
            prefs.getString('myString'), 'This is my string.');
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getString() - non string preferences as string',
      id : 'PREF0031',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can get string, float, array, undefined ' +
                   'and boolean as String. Checks if it throws any exception ' +
                   'doing that.',
      run: function(context, callback, result) {
        // userPrefs
        var userPrefs = ['myString', 'myUndefined',
            'myBool', 'myTestArray', 'myObject', 'myFloat'];

        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        try {
          for (var i = 0; i < userPrefs.length; i++) {
            result.addSubResult('getString(' + userPrefs[i] + ')',
                Assert.assertEquals, prefs.getString(userPrefs[i]),
                prefs.getString(userPrefs[i]));
          }
        } catch (ex) {
          result.addSubResult('getString(' + userPrefs[i] + ')',
              Assert.assertFalse, ex.toString(), 'No Exception');
        }
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getBool()',
      id : 'PREF040',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P0,
      description: 'Tests if getBool returns right value for boolean' +
                   ' preference',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyValue(result, 'getBool(myBool)',
            prefs.getBool('myBool'), true);
        callback(result);
      }
    },

    { name: 'gadget.Prefs - getBool() - non bool preference',
      id : 'PREF041',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can get float, object, undefined and int as' +
                   'boolean. Checks if it throws exception.',
      run: function(context, callback, result) {
        // userPrefs
        var userPrefs = ['myString', 'myUndefined',
            'myBool', 'myTestArray', 'myObject', 'myFloat'];

        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        try {
          for (var i = 0; i < userPrefs.length; i++) {
            result.addSubResult('getString(' + userPrefs[i] + ')',
                Assert.assertEquals, prefs.getBool(userPrefs[i]),
                prefs.getBool(userPrefs[i]));
          }
        } catch (ex) {
          result.addSubResult('getString(' + userPrefs[i] + ')',
              Assert.assertFalse, ex.toString(), 'No Exception');
        }
        callback(result);
      }
    },


    { name: 'gadget.Prefs - getArray(key)',
      id : 'PREF050',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P0,
      description: 'Tests if getArray returns right value for Array preference',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        PrefsSuite.verifyList(result, 'getArray(myTestArray)',
            prefs.getArray('myTestArray'), ['zdnet', 'pc', 'Apple Insider']);
        callback(result);
        }
      },

    { name: 'gadget.Prefs - getArray(key)',
      id : 'PREF051',
      bugs: ['1055527'],
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can retrieve float, object, undefined and int' +
                   ' as Array. Checks if it throws exception.',
      run: function(context, callback, result) {
        // userPrefs
        var userPrefs = ['myString', 'myUndefined',
            'myBool', 'myTestArray', 'myObject', 'myFloat'];

        var prefs = new gadgets.Prefs();
        PrefsSuite.verifyPref(result, prefs);
        try {
          for (var i = 0; i < userPrefs.length; i++) {
            PrefsSuite.verifyList(result, 'getArray(' + userPrefs[i] + ')',
                prefs.getArray(userPrefs[i]), prefs.getArray(userPrefs[i]));
          }
        } catch (ex) {
          result.addSubResult('getString(' + userPrefs[i] + ')',
              Assert.assertFalse, ex.toString(), 'No Exception');
        }
        callback(result);
        }
      },

      { name: 'gadget.Prefs - prefs.set - String as data',
        id : 'PREF060',
        bugs : ['1111587', '1055605', '1033859'],
        priority: Test.PRIORITY.P0,
        description : '<precondition> The gadget requires the feature' +
                      ' setprefs.</precondition> Tests if we can set string, ' +
                      'float, int and boolean. Try to get them as other ' +
                      'formats i.e float as string, int, boolean and so on.',
        run: function(context, callback, result) {
          var prefs = new gadgets.Prefs();
          var key = 'myKey';
          var string = 'setprefsValue ' + new Date().getTime();
          // input value set
          var testInput = [ string, 5.5, 8888, true ];
          var getMethod = [
              prefs.getString, prefs.getFloat, prefs.getInt, prefs.getBool];

          // going through data set
          for (var i = 0; i < 4; i++) {
            prefs.set(key, testInput[i]);
            PrefsSuite.verifyValue(result, '', getMethod[i](key), testInput[i]);
          }

          var data = '<a>http://www.google.com</a>';
          prefs.set(key, data);
          PrefsSuite.verifyValue(result,  'set(' + key + ', ' + data + ')',
              gadgets.util.unescapeString(prefs.getString(key)), data);
          callback(result);
        }
      },

    { name: 'gadget.Prefs - prefs.set - set string with pipe as array',
      id : 'PREF061',
      bugs : ['1111587', '1055605', '1033859'],
      priority: Test.PRIORITY.P0,
      description : '<precondition> The gadget requires the feature' +
                    ' setprefs.</precondition> Tests if we can set string, ' +
                    'with | as an array and retrieve value using getArray.',
      run: function(context, callback, result) {
        var prefs = new gadgets.Prefs();
        prefs.set('myArray', 'one|two|three');
        PrefsSuite.verifyList(result, 'set(\'myArray\', \'one|two|three\'',
            prefs.getArray('myArray'), ['one', 'two', 'three']);
        callback(result);
      }
    },

      { name: 'gadget.Prefs - negative tests set array using prefs.set',
        id : 'PREF007',
        bugs : ['1111587', '1055605', '1033859'],
        priority: Test.PRIORITY.P2,
        description : '<precondition> The gadget requires the feature ' +
                      'setprefs.</precondition>.Test the gadgets.Prefs set() ' +
                      'capabilities with negative scenarios.',
        run: function(context, callback, result) {
          var prefs = new gadgets.Prefs();
          var testInput = [['one', 'two', 'three'], ['\u597D'], []];
          var arrayKey = 'myKey';
          for (var i = 0; i < testInput.length; i++) {
            try {
              prefs.set(arrayKey, testInput[i]);
              PrefsSuite.verifyList(result,
                  'set(' + arrayKey + ', ' + testInput[i] + ')',
                  prefs.getArray(arrayKey), testInput[i]);
            } catch (ex) {
              var stacktrace = ex.stack || '';
              var exception =  'Exception: ' + ex.name +
                  ' - File: ' + ex.fileName +
                  ' - Line:(' + ex.lineNumber + ') - Msg:' +
                  ex.message + '\nStacktrace: ' + stacktrace;
              result.addSubResult('set(arrayKey, ' + testInput[i] + ')',
                  Assert.fail, exception, 'No Exception',
                  Result.severity.WARNING);
            }
          }
          callback(result);
        }
      },

      { name: 'gadget.Prefs - prefs.setArray',
        id : 'PREF070',
        bugs : ['1111587', '1055605', '1033859'],
        priority: Test.PRIORITY.P2,
        description : '<precondition> The gadget requires the feature' +
                      'setprefs.</precondition>. Tests if we can set array' +
                      ' using setArray().',
        run: function(context, callback, result) {
          var prefs = new gadgets.Prefs();
          var testInput = [['one', 'two', 'three'], ['\u597D']];
          var arrayKey = 'myArray';

          for (var i=0; i < testInput.length; i++) {
            try {
              prefs.setArray(arrayKey, testInput[i]);
              PrefsSuite.verifyList(result,
                  'setArray(' + arrayKey + ', ' + testInput[i] + ')',
                  prefs.getArray(arrayKey), testInput[i]);
            } catch (ex) {
              var stacktrace = ex.stack || '';
              var exception =  'Exception: ' + ex.name +
                  ' - File: ' + ex.fileName +
                  ' - Line:(' + ex.lineNumber + ') - Msg:' +
                  ex.message + '\nStacktrace: ' + stacktrace;
              result.addSubResult('set(arrayKey, ' + testInput[i] + ')',
                  Assert.fail, exception, 'No Exception');
            }
          }
          callback(result);
        }
      },

      { name: 'gadget.Prefs - prefs.setArray - negative case',
        id : 'PREF008',
        bugs : ['1111587', '1055605', '1033859'],
        priority: Test.PRIORITY.P2,
        description : '<precondition> The gadget requires the feature' +
                      'setprefs.</precondition>. Tests if we can set string ' +
                      'array,float, int and boolean as an input to setArray().',
        run: function(context, callback, result) {
          var prefs = new gadgets.Prefs();
          var key = 'myKey';
          var testInput = [true, 5.5, 8888, 'test', 'one|two', undefined, []];
          var arrayKey = 'myArray';

          for (var i=0; i < testInput.length; i++) {
            try {
              prefs.setArray(arrayKey, testInput[i]);
              PrefsSuite.verifyList(result,
                  'setArray(' + arrayKey + ', ' + testInput[i] + ')',
                  prefs.getArray(arrayKey), testInput[i]);
            } catch (ex) {
              var stacktrace = ex.stack || '';
              var exception =  'Exception: ' + ex.name +
                  ' - File: ' + ex.fileName +
                  ' - Line:(' + ex.lineNumber + ') - Msg:' +
                  ex.message + '\nStacktrace: ' + stacktrace;
              result.addSubResult('set(arrayKey, ' + testInput[i] + ')',
                  Assert.fail, exception, 'No Exception',
                  Result.severity.WARNING);
            }
          }
          callback(result);
        }
      },

      { name: 'gadget.Prefs - getMsg',
        id : 'PREF009',
        priority: Test.PRIORITY.P0,
        description: 'Tests if we can get an unformatted message from an ' +
                     'undefined,string and empty space',
        run: function(context, callback, result) {
          var prefs = new gadgets.Prefs();
          var msg = prefs.getMsg('hello_world');
          PrefsSuite.verifyValue(result, 'getMsg(\'hello_world\')', msg,
              'Hello World.', Assert.assertStringContains);
          PrefsSuite.verifyValue(result, 'getMsg()', prefs.getMsg("undefined"),
              "");
          PrefsSuite.verifyValue(result, 'getMsg()', prefs.getMsg(""),
              "");

          callback(result);
        }
      }
  ]
};

PrefsSuite.verifyPref = function(result, prefs) {
  result.setResult(Assert.assertNotNull(prefs), prefs, 'not null');
  return result.success;
}

PrefsSuite.verifyValue = function(result, name, actual, expected, assert) {
  var assertFunc = assert || Assert.assertEquals;
  Helper.addSubResult(result, name, assertFunc, actual, expected);
  return result.success;
}

PrefsSuite.verifyList = function(result, name, actual, expected) {
  var outcome = Assert.assertNotNull(actual) && Assert.assertNotNull(expected);
  Helper.addSubResult(result, name + ' is not null', outcome, actual, expected);
  if (outcome) {
    Helper.addSubResult(result, name + ' length',
        Assert.assertEquals, actual.length, expected.length);
    for (var i = 0; i < actual.length; i++) {
      outcome = (actual[i] && expected[i]) &&
          Assert.assertEquals(actual[i], expected[i]);
      Helper.addSubResult(result, name + '[' + i + ']', outcome, actual[i],
          expected[i]);
    }
  }
  return result.success;
}
