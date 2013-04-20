/**
 *
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
 *
 */

/**
 * @fileoverview Assertion utility functions that mirrors after JUnit, as well
 * as high-level assertion wrapper classes for verifying a person, friends
 * and activities etc...
 */
function Assert() {};

/**
 * Asserts the parameter is true.
 * @param {*} actual Parameter to be tested as true.
 * @return {boolean} true if parameter actual is true, false if not.
 */
Assert.assertTrue = function(actual) {
  return (actual);
};

/**
 * Asserts the parameter is false.
 * @param {*} actual Parameter to be tested as false.
 * @return {boolean} true if parameter actual is false.
 */
Assert.assertFalse = function(actual) {
  return (!actual);
};


/**
 * Useful to log exceptions.
 */
Assert.fail = function() {
  return false;
};

/**
 * Asserts the actual and expected parameters are equal.
 * @param {*} actual Parameter to be tested for equiality against expected.
 * @param {*} expected Parameter to be tested for equiality against actual.
 * @return {boolean} true if parameter actual and expected are equal, false if
 * they are not.
 */
Assert.assertEquals = function(actual, expected) {
  return (actual == expected);
};

/**
 * Asserts the actual is greater than expected parameters.
 * @param {*} actual Parameter to be tested for greater than against expected.
 * @param {*} expected Parameter to be tested.
 * @return {boolean} true if parameter actual is greater than expected, false
 * otherwise
 */
Assert.assertGreaterThan = function(actual, expected) {
  return (actual > expected);
};

/**
 * Asserts the actual and expected parameters are absolutely equal.
 * Given x=5: x===5 is true and  x==="5" is false.
 * @param {*} actual Parameter to be tested for equiality against expected.
 * @param {*} expected Parameter to be tested for equiality against actual.
 * @return {boolean} true if parameter actual and expected are equal, false if
 * they are not.
 */
Assert.assertAbsoluteEquals = function(actual, expected) {
  return (actual === expected);
};

/**
 * Asserts the actual and expected parameters are not equal.
 * @param {*} actual Parameter to be tested for non-equiality against expected.
 * @param {*} expected Parameter to be tested for non-equiality against actual.
 * @return {boolean} true if parameter actual and expected are not equal, false
 *     if they are.
 */
Assert.assertNotEquals = function(actual, expected) {
  return (actual != expected);
};

/**
 * Asserts the actual parameter is not undefined.
 * @param {*} actual Parameter that will be tested against undefined.
 * @return {boolean} true if actual is not undefined, false if it is.
 */
Assert.assertDefined = function(actual) {
  return (actual != undefined);
};

/**
 * Asserts the actual parameter is  undefined.
 * @param {*} actual Parameter that will be tested against undefined.
 * @return {boolean} true if actual is undefined, false if it is not.
 */
Assert.assertNotDefined = function(actual) {
  return (actual == undefined);
};

/**
 * Asserts the actual parameter is a defined object that is not empty.
 * @param {*} actual Parameter that will be tested.
 * @return {boolean} true if actual is existing and empty, false if it
 *     is undefined or empty.
 */
Assert.assertNotEmpty = function(actual) {
  var defined = (actual != undefined);
  var length = 0;
  if ((typeof(actual) == 'string') || (typeof(actual) == 'object')) {
    length = actual && actual.hasOwnProperty('length') ? actual.length : -1;
  }
  return Assert.assertTrue(defined && (length>0));
};

/**
 * Asserts the actual parameter is  a defined object that is empty.
 * @param {*} actual Parameter that will be tested.
 * @return {boolean} true if actual is defined and not empty, false if it is
 *     undefined or not empty.
 */
Assert.assertEmpty = function(actual) {
  var defined = (actual != undefined);
  var length = 0;
  if ((typeof(actual) == 'string') || (typeof(actual) == 'object')) {
    length = actual && actual.length != undefined ? actual.length : -1;
  }
  return Assert.assertTrue(defined && (length==0));
};

/**
 * Asserts the actual parameter is  a map which is defined and not null,
 * and that it has no keys.
 * @param {Map} actual Map that will be evaluated.
 *     @return {boolean} true if actual is empty or undefined, false if not.
 */
Assert.assertEmptyMap = function(actual) {
  if (!actual || actual.constructor != Object) {
    return false;
  }
  // If the object contains any key, return false.
  for (var key in actual) {
    return false;
  }
  return true;
};

/**
 * Asserts the actual parameter is not null.
 * @param {*} actual Parameter that will be tested.
 * @return {boolean} true if actual not null, false if it is.
 */
Assert.assertNotNull = function(actual) {
  return Assert.assertTrue(actual != null);
};

/**
 * Asserts the actual parameter is null.
 * @param {*} actual Parameter that will be tested.
 * @return {boolean} true if actual is null, false if it is not.
 */
Assert.assertNull = function(actual) {
  return Assert.assertTrue(actual == null);
};

/**
 * Asserts the actual parameter is empty, undefined or with value ''. If the
 * parameter is an Object, it asserts its contents are empty and in the case of
 * an Array, it asserts all of its values are empty.
 * @param {*} actual Parameter that will be tested.
 * @return {boolean} true if the value of the object is empty or if its contents
 *     are empty for Arrays.
 */
Assert.assertDataEmpty = function(actual) {
  if (actual == undefined || actual == null || actual == '') {
    return true;
  }
  if (typeof(actual) == 'object') {
    for (var propertyName in actual) {
      if (propertyName && !Assert.assertDataEmpty(actual[propertyName])) {
        return false;
      }
    }
    return true;
  }
  if (actual instanceof Array) {
    for (var i = 0; i < actual.length; i++) {
      if (!Assert.assertDataEmpty(actual[i])) {
        return false;
      }
    }
    return true;
  }
  return false;
}

/**
 * Asserts the actual and expected parameter are equal, if expected is empty
 * @param {*} actual Parameter that will be compared to expected, if this is
 *     not empty.
 * @param {*} expected Parameter that, if empty, will be compared to actual, 
 * @return {boolean} true if expected is empty, or if actual and expected are
 *     equal.
 */
Assert.assertDataEquals = function(actual, expected) {
  if (Assert.assertDataEmpty(expected)) {
    return true;
  }
  return actual == expected;
}

/**
 * Asserts the substring parameter is contained within the actual parameter.
 * @param {String} actual Parameter that will be checked to assert that
 *     substring is contained in it.
 * @param {String} substring Parameter that will be searched within actual to
 *     assert it is contained in it.
 * @return {boolean} true if substring is contained in actual, false if
 *     it is not.
 */
Assert.assertStringContains = function(actual, substring) {
  return Assert.assertTrue((actual != undefined) &&
    (actual.indexOf(substring) >= 0));
};

/**
 * Asserts the actual string parameter ends with substring parameter.
 * @param {String} actual Parameter that will be checked to assert that
 *     it ends with substring.
 * @param {String} substring Parameter that will be searched within actual to
 *     assert it is in the end of actual string.
 * @return {boolean} true if actual string ends with substring, false if
 *     it is not.
 */
Assert.assertStringEndsWith = function(actual, substring) {
  return Assert.assertTrue((actual != undefined) &&
    (actual.indexOf(substring) + substring.length === actual.length));
};

/**
 * Asserts all the fields existing in actual also exist in expected, if these
 * fields are supported by the environment.
 * @param {Object} actual Parameter which fields will be compared with the
 *     fields in expected.
 * @param {Object} expected Parameter which fields will be compared with the
 *     fields in actual.
 * @return {boolean} true if all the fields in expected are also contained in
 *     actual, false if any field in actual is not contained in expected or if
 *     any field in expected is not supported by the environment.
 */
Assert.assertNameEquals = function(actual, expected) {
  var nameType = opensocial.Environment.ObjectType.NAME;
  for (var fieldName in expected) {
    if (opensocial.getEnvironment().supportsField(nameType, fieldName)) {
      if (!actual || !expected) {
        return false;
      }
      if (actual.getField(fieldName) != expected[fieldName]) {
        return false;
      }
    }
  }
  return true;
}

/**
 * Asserts the url parameter is a string in a valid url format.
 * @param {String} url Parameter that will be tested to verify it is a string
 *     in url format.
 * @return {boolean} true if url is a string in url format, false if it is not.
 */
Assert.assertValidUrl = function(url) {
      var match = /http:\/\/[A-Za-z0-9\.-]{3,}\.[A-Za-z]{3}/
      return match.test(url);
}


/**
 * Asserts the dataResponse has errors.
 * @param {Object} dataOutput dataOutput from makeRequest APIs.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {Array} errorsArray expected errors array.
 * @return {boolean} true for success, false for failure.
 */
Assert.assertDataContentHasError = function(dataOutput, result, errorsArray) {
  result.setResult(Assert.assertNotEmpty(dataOutput.errors),
      dataOutput.errors, 'errors should not be empty');

  Helper.addSubResult(result, 'dataResponse not null', Assert.assertNotNull,
      dataOutput, 'not null');

  Helper.addSubResult(result, 'data should be null', Assert.assertNull,
      dataOutput.data, 'data should be null');

  Helper.addSubResult(result, 'text should be null', Assert.assertNull,
      dataOutput.text, 'text should be null');

  var length = dataOutput.errors ? dataOutput.errors.length : 0;
  Helper.addSubResult(result, 'dataOutput.errors.length', Assert.assertEquals,
      length, errorsArray.length);

  for (var i = 0; i < length; i++) {
    var expected = errorsArray[i];
    Helper.addSubResult(result, 'errors[' + i + ']', Assert.assertEquals,
        dataOutput.errors[i], expected);
  }
  return result.success;
}

/**
 * Asserts the data content is of the type indicated in the contentype parameter
 * and if it is, asserts the data is the expected.
 * @param {Object} dataObject dataOutput from makeRequest APIs.
 * @param {gadgets.io.ContentType} contentType content type to check against.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {String} expectedText expected text property.
 * @param {Object} expectedData expected data property.
 * @return {boolean} true for success, false for failure.
 */
Assert.assertDataContent = function(dataOutput, contentType, result,
    expectedText, expectedData) {
  result.setResult(Assert.assertEmpty(dataOutput.errors), dataOutput.errors,
      'no errors');

  Helper.addSubResult(result, 'dataResponse not null',
      Assert.assertNotNull, dataOutput, 'not null');

  Helper.addSubResult(result, 'dataResponse.data not null',
      Assert.assertNotNull, dataOutput.data, 'not null');

  Helper.addSubResult(result, 'dataResponse.text not null',
      Assert.assertNotNull, dataOutput.text, 'not null');

  if (dataOutput && dataOutput.data) {
    var dataTest;
    var textTest;
    switch(contentType) {
      case gadgets.io.ContentType.DOM:
        if (dataOutput.data) {
          var isDom = dataOutput.data.documentElement;
          Helper.addSubResult(result, 'Result is of ContentType ' + contentType,
              Assert.assertDefined, isDom, 'documentElement should exist');

          var trimmed = dataOutput.text.replace(/^\s+|\s+$/g, '');
          Helper.addSubResult(result, 'response.text', Assert.assertEquals,
              trimmed, expectedText);

          if (dataOutput.data.documentElement) {
            trimmed = dataOutput.data.documentElement.textContent.replace(
                /^\s+|\s+$/g, '');
            Helper.addSubResult(result, 'data.documentElement.textContent',
                Assert.assertEquals, trimmed, expectedData);
          }
        }
        break;
      case gadgets.io.ContentType.FEED:
        if (dataOutput.data) {
          var isJson = gadgets.json.stringify(dataOutput.data);
          Helper.addSubResult(result, 'Result is of ContentType ' + contentType,
              Assert.assertTrue, isJson,
              'gadgets.json.parse(data) should be true');
        }
        break;
      case gadgets.io.ContentType.JSON:
        if (dataOutput.data && dataOutput.text) {
          var processedText =
              gadgets.json.stringify(gadgets.json.parse(dataOutput.text));

          Helper.addSubResult(result, 'response.data', Assert.assertEquals,
              gadgets.json.stringify(dataOutput.data), processedText);

          Helper.addSubResult(result, 'response.text', Assert.assertEquals,
              processedText, expectedText);
        }
        break;
      case gadgets.io.ContentType.TEXT:
        if (dataOutput.data && dataOutput.text) {
          var isText = (typeof(dataOutput.data) == 'string');
          Helper.addSubResult(result, 'Result is of ContentType ' + contentType,
              Assert.assertTrue, isText, 'data should be string');

          if (isText) {
            // data and text property should be same for contentType == TEXT
            Helper.addSubResult(result, 'response.data', Assert.assertEquals,
                dataOutput.data, expectedText);

            Helper.addSubResult(result, 'response.text', Assert.assertEquals,
                dataOutput.text, expectedText);
          }
        }
        break;
      default:
        Helper.addSubResult(result, 'Result is of ContentType ' + contentType,
            Assert.fail, contentType, 'ContentType should not be Unkown');
        break;
    }
  }
  return result.success;
}

/**
 * Asserts whether the dataresponse had or didnt have errors in the defined
 * keys, according to what is defined in the isErrorExpected parameter.
 * @param {DataResponse} dataResponse response object.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {Array} keys Contains all key to ask for in the dataResponse.
 * @param {boolean} isErrorExpected true if an error is expected, false if
 *     it is not.
 * @param {Result.severity} specifies logging level
 * @return {boolean} true for success, false for failure.
 */
Assert.assertDataResponseHadError = function(dataResponse, result, keys,
    isErrorExpected, opt_severity) {
  var method = isErrorExpected ? 'Assert.assertTrue' : 'Assert.assertFalse';
  var error = dataResponse == undefined;
  result.addSubResult('dataResponse.hadError()', method,
      error ? error : dataResponse.hadError(), isErrorExpected, opt_severity);

  if (error) {
    return error;
  }

  if (isErrorExpected) {
    Assert.logResponseItems(dataResponse, result, keys);
  } else {
    if (keys && keys.length) {
      for (var i = 0; i < keys.length; i++) {
        var responseItem = dataResponse.get(keys[i]);
        if (responseItem && responseItem.hadError()) {
          Assert.assertDataResponseItemHadError(keys[i], responseItem, result,
              false);
        }
      }
    }
  }
  return dataResponse.hadError() === isErrorExpected;
};

/**
 * Asserts the dataresponse didnt have errors, only if the viewer or owner has
 * the authorization.
 * @param {DataResponse} dataResponse response object.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {Array} keys Contains all key to ask for in the dataResponse.
 * @param {Result.severity} severity logging level
 * @param {opensocial.DataRequest.PersonId} viwerOrOwner you can tell what to do
 * incase if owner or viewer is unavailable  
 * @return {boolean} true if the dataresponse didnt have errors and the viewer.
 *     is authorized, false if the viewer is not authorized or the dataresponse
 *     had errors.
 */
Assert.assertDataResponseShouldContinue = function(dataResponse, result
    , context, keys, opt_severity, opt_viewerOrOwner) {
  if (context.isNotAvailable(opt_viewerOrOwner)) {
    // should get unauthorized error
    Helper.addInfoSubResult(result,
        'Expecting error', Assert.fail, dataResponse.hadError(), true);
    Assert.assertDataResponseHadError(
        dataResponse, result, null, true, opt_severity);
    if (keys) {
      for (var i = 0; i < keys.length; i++) {
        Assert.assertDataResponseItemHadError(keys[i],
            dataResponse.get(keys[i]), result, true);
      }
    }
    return false;
  } else {
    var loggingLevel = opt_severity ? opt_severity : Result.severity.INFO;
    return Assert.assertDataResponseHadError(dataResponse, result, null,
        false, loggingLevel);
  }
}

/**
 * Iterate and log every items specified at keys.
 * @param {DataResponse} dataResponse response object.
 * @param {ResultGroup} result object for setting the test result.
 * @param {Array} keys Contains all key to ask for in the dataResponse.
 */
Assert.logResponseItems = function(dataResponse, result, keys) {
  if (keys) {
    var responseItem;

    // Loop thru each responseItem
    for (var i = 0; i < keys.length; i++) {
      responseItem = dataResponse.get(keys[i]);
      if (responseItem) {
        if (dataResponse.hadError()) {

          Helper.logIntoResult(result,
              'ResponseItem[' + keys[i] + '].hadError()',
              responseItem.hadError(), '') ;

          if (!responseItem.hadError() && responseItem.getData()) {
            Helper.logIntoResult(result,
                'ResponseItem[' + keys[i] + '].getData()',
                responseItem.getData(), '');
          }

          if (responseItem.getErrorCode()) {
            Helper.logIntoResult(result,
                'ResponseItem[' + keys[i] + '].getErrorCode()',
                responseItem.getErrorCode(), '');
          }
          // Log ErrorMessage if it exists.
          if (responseItem.getErrorMessage()) {
            Helper.logIntoResult(result, keys[i] + ' - ErrorMessage: ',
                responseItem.getErrorMessage(), '');
          }
        } else {
          Helper.logIntoResult(result, 'dataResponse.get(' + keys[i] + ')',
              responseItem.getData(), 'some value');
        }
      } else {

        Helper.logIntoResult(result, 'dataResponse.get(' + keys[i] + ')',
            'undefined', '');
      }
    }
  }
};

/**
 * Asserts whether a specific item of the dataresponse had or didnt have an
 * error according to what is defined in the isErrorExpected parameter.
 * @param {DataResponse} dataResponse response object.
 * @param {String} alias name of the item to check.
 * @param {Object} responseItem Item that will be checked for errors.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {boolean} isErrorExpected true if an error is expected, false if
 *     it is not.
 * @param {String} errorCode code of the error expected, if an error is expected
 *     if not specified the method will only check if it is defined.
 * @param {String} errorMesg message of the error expected, if an error
 *     is expected, if not specified the method will only check if it is defined
 * @param {Result.severity} opt_severity specifies the severity with which you
 *     want to log results
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertDataResponseItemHadError = function(alias, responseItem, result,
    isErrorExpected, errorCode, errorMesg, opt_severity) {
    Helper.addSubResult(result, 'responseItem[' + alias + '].is defined',
        Assert.assertDefined, responseItem, 'defined');

  if (responseItem) {
    var outcomeHadError = (responseItem.hadError() == isErrorExpected);
    var outcomeGetData = isErrorExpected ?
        Assert.assertNotDefined(responseItem.getData()) :
        Assert.assertDefined(responseItem.getData());
    var expectedGetData = isErrorExpected ? 'undefined' : 'defined';

    result.addSubResult('responseItem[' + alias + '].hadError', outcomeHadError,
        responseItem.hadError(), isErrorExpected, opt_severity);

    result.addSubResult('responseItem[' + alias + '].getData', outcomeGetData,
        gadgets.json.stringify(responseItem.getData()),
        expectedGetData, opt_severity);

    if (isErrorExpected) {
      var methodErrorCode = (errorCode) ? Assert.assertEquals :
          Assert.assertDefined;
      var methodErrorMesg = (errorMesg) ? Assert.assertEquals :
          Assert.assertDefined;
      var expectedErrorCode = (errorCode) ? errorCode : 'defined';
      var expectedErrorMesg = (errorMesg) ? errorMesg : 'defined';

      result.addSubResult('responseItem[' + alias + '].getErrorCode',
          methodErrorCode, responseItem.getErrorCode(), expectedErrorCode,
          opt_severity);
    }
  }
  return result;
}

/**
 * Asserts the actual and expected objects are equal in the fields specified by
 * fieldsArray or in all the fields if fieldsArray is not specified.
 * @param {Object} actual Object to be compared against expected for equality
 *     of fields.
 * @param {String} objectAlias name of the item to check.
 * @param {Array} fieldsArray the fields to be checked for equality between
 *     actual and expected.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {Object} expected Object to be compared against actual for equality
 *     of fields.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertObjectEquals = function(objectAlias, actual, fieldsArray,
    expected, result) {
  var fieldValue;
  var currentDefined = false;
  var opening = (fieldsArray) ? ".getField('" : ".";
  var closing = (fieldsArray) ? "')" : "()";
  var iterator = (fieldsArray) ? fieldsArray : actual;
  for (var prop in iterator) {
    if (fieldsArray == null) {
      var current = actual[prop];
      if ((current.length == 0) && (typeof(current) == "function")) {
        fieldValue = actual[prop]();
        currentDefined = true;
      }
    } else {
      prop = iterator[prop];
      currentDefined = true;
      fieldValue = actual && actual.getField &&
        (typeof(actual.getField) == 'function') ?
        actual.getField(prop) : undefined;
    }
    if (currentDefined) {
      if ((expected != undefined) && (expected[prop] !== undefined)) {
        var method = 'Assert.assertEquals';
        if (expected['__rules__'] && expected['__rules__'][prop]) {
          method = expected['__rules__'][prop];
        }
        Helper.addSubResult(result, objectAlias + opening
            + prop + closing, method, fieldValue,
            expected[prop]);
      } else {
        Helper.addUnverifiedResult(result, objectAlias + opening
            + prop + closing, fieldValue);
      }
      currentDefined = false;
    }
  }
  return result;
};

/**
 * Asserts the actual parameter's friends are the same as the expected friends.
 * @param {String} objectAlias name of the object to check.
 * @param {Object} actual Object which friends will be checked against
 *     the expectedfriends.
 * @param {Object} expectedFriends Object containing a list of the friends
 *     that will be compared against actual parameter's friends.
 * @param {Array} personFields contains the fields that will be checked to test
 *     equality between the friend Objects.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {function} comparator Used to sort the friends in the order needed.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertFriendsEquals = function(objectAlias, actual, expectedFriends,
    personFields, comparator, result) {
  if (typeof(expectedFriends) == 'object') {
    Helper.addSubResult(result, objectAlias + ".length", Assert.assertEquals,
        actual.length, expectedFriends.length);
    if (!comparator) {
      expectedFriends && expectedFriends.sort(Assert.Comparator.IdComparator);
      actual && actual.sort(Assert.Comparator.IdComparator);
    } else {
      expectedFriends && expectedFriends.sort(comparator);
    }
    for (var i = 0; i < expectedFriends.length; i++) {
      var testName = objectAlias + "[" + i + "]";
      Assert.assertObjectEquals(testName, actual[i], null, expectedFriends[i],
          result);
      Assert.assertObjectEquals(testName, actual[i], personFields,
          expectedFriends[i], result);
    }
    if (actual.length > expectedFriends.length) {
      Helper.addSubResult(result, 'Unexpected Friend',
          Assert.fail, actual.getField('id'), 'Should not be returned');
    }
  } else {
    for (var i=0; i < actual.length; i++) {
      var testName = objectAlias + "[" + i + "]";
      Assert.assertObjectEquals(testName, actual[i], undefined, null, result)
      Assert.assertObjectEquals(testName, actual[i], personFields,
          undefined, result)
    }
  }
  return result;
};

/**
 * Shows whether or not the fields in actual are supported by the environment
 * and if they are checks them for equality against the expected parameter
 * fields. If the expected parameter fields are not supported. The method checks
 * only if the actual parameter's fields are defined.
 * @param {Object} actual Object which fields will be checked against
 *     the expected fields after checking if the environment supports them.
 * @param {Object} expected Object containing the fields the actual parameter
 *     object is expected to have.
 * @param {Array} allFields contains the fields that will be checked to test
 *     equality between actual and expected.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {String} objectType Used to sort the friends in the order needed.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertSupportedObjectFields = function(result, actual, expected,
    allFields, objectType) {
  if (actual == undefined) {
    Helper.addUnsevereSubResult(result, 'Undefined: ' + objectType,
        Assert.fail, actual, 'defined');
    return result;
  }
  for (var i = 0; i < actual.length; i++) {
    for (var j = 0; j < allFields.length; j++) {
      if (opensocial.getEnvironment().supportsField(objectType, allFields[j])) {
        var data = actual[i] ? actual[i].getField(allFields[j])
            : undefined;

        if (expected && expected[i] && expected[i][allFields[j]]){
          Helper.addSubResult(result, 'object[' + allFields[j] + ']',
              Assert.assertEquals, data, expected[i][allFields[j]]);
        } else {
          Helper.addUnsevereSubResult(result, 'object[' + allFields[j] + ']',
              Assert.assertDefined, data, 'Defined');
        }
      } else {
        // unsupported field should return undefined
        var data = actual[i] ? actual[i].getField(allFields[j])
            : undefined;
        var outcome = Assert.assertNotDefined(data);
        Helper.addSubResult(result, 'Nonsupported Field - ' + allFields[j],
            Assert.assertNotDefined, data, 'undefined');
      }
    }
  }
  return result;
}

/**
 * Asserts that for a person object, expected fields should not be null and
 * should match the actual person object's field value, all other fields should
 * be NULL.
 * @param {Object} actual Object which fields will be checked against
 *     the expected fields.
 * @param {Object} expected Person object.
 * @param {Array} expectedFields contains the fields that will be checked
 *     to test equality between actual and expected.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {Array} allFields contains all the fields that should be tested.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertPersonFields = function(result, actual, expected, expectedFields,
    allFields) {
  for (var field in allFields) {
    var method = null;
    // basic fields
    if (expectedFields && Assert.arrayContains(expectedFields, field)) {
      var title = (actual && actual.getId) ?
          actual.getId() + ' ' + field : field;
      var fieldValue = (actual && actual.getField) ?
          actual.getField(field) : undefined;
      var expectedValue = (expected && expected[field]) ? expected[field] :
          null;
      if (typeof(fieldValue) == 'object' && fieldValue != null) {
        Helper.addSubResult(result, title, Assert.assertNotNull,
            fieldValue, expectedValue ? expectedValue : 'some value');
      } else {
        if (expected && expected["__rules__"] && expected["__rules__"][field]) {
          method = expected["__rules__"][field];
       }
        Helper.addSubResult(result, title, method, fieldValue,
            expectedValue);
      }
    } else {
      // extended fields, should be null
      var title = (actual && actual.getId()) ?
          actual.getId() + ' Nonaccessible Field ' + field
          : ' Nonaccessible Field ' + field;
      try {
        var fieldValue = actual ? actual.getField(field) : undefined;
        Helper.addInfoSubResult(result, title, Assert.assertDataEmpty,
            fieldValue, 'null or empty');
      } catch (ex) {
        Helper.addSubResult(result, title, false, Helper.getExceptionHtml(ex),
            'No Exception');
      }
    }
  }
  return result;
}




/**
 * Asserts that for a person object's friends who has the people app installed,
 * expected fields should not be null and should match the actual friend
 * object's field value.  Other fields not included in fieldsToVerify are not
 * checked.
 * @param {Object} actual Object which fields will be checked against
 *     the expectedPersonObject fields.
 * @param {Object} expectedPersonObject The expected person object.
 * @param {Array} fieldsToVerify contains the fields that will be checked
 *     to test equality between actual and expected.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {function} comparator Used to sort the friends in the order needed.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertHasAppFriendFields = function(result, actual, expectedPersonObject,
    fieldsToVerify, comparator) {
  // hasPeopleApp should be true for all of the friends fetched
  var expectedCloned;
  expectedCloned = Helper.cloneObject(expectedPersonObject);
  var expectedFriends = expectedCloned &&
                        expectedCloned['__friends__'];
  var expectedHasAppFriends = expectedFriends ? [] : undefined;
  if (expectedFriends) {
    for (var i = 0; i < expectedFriends.length; i++) {
      // friends with app installed
      if (expectedFriends[i]['hasPeopleApp']) {
        expectedHasAppFriends.push(expectedFriends[i]);
      }
    }
  }
  // another test will verify the default sorting
  if (!comparator) {
    expectedHasAppFriends &&
        expectedHasAppFriends.sort(Assert.Comparator.IdComparator);
    actual & actual.sort(Assert.Comparator.IdComparator);
  } else {
    expectedFriends && expectedFriends.sort(comparator);
  }

  if (expectedHasAppFriends) {
    Helper.addSubResult(result, "hasAppFriends.length", Assert.assertEquals,
        actual.length, expectedHasAppFriends.length);
    for (var i = 0; i < expectedHasAppFriends.length; i++) {
      Assert.assertPersonFields(result, actual[i], expectedHasAppFriends[i],
          fieldsToVerify, opensocial.getEnvironment().supportedFields
          [opensocial.Environment.ObjectType.PERSON]);
    }
  } else {
    // just log the results
    for (var i = 0; i < actual.length; i++) {
      Assert.assertPersonFields(result, actual[i], undefined,
          undefined, opensocial.getEnvironment().supportedFields
          [opensocial.Environment.ObjectType.PERSON]);
    }
  }
  return result;
}

/**
 * For ACL testing.  Asserts that when profile details all is used, for a person
 * object's friends who has the people app installed, all supportedPersonFields
 * should not be null and should match the actual friend object's field value.
 * For friends who doesn't have the app installed, basicPersonFields should not
 * be null and should match the expected friend's basicPersonFields' values.
 * @param {Object} actual Object that will be used to check the personObject
 *     object's fields match it.
 * @param {Object} personObject The expected person object.
 * @param {Array} basicPersonFields contains the fields conseidered basic
 *     that will be checked.
 * @param {Array} supportedPersonFields contains the fields supported.
 * @param {ResultGroup} result result object to append verifications to.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertFriendProfileFields = function(result, actual, personObject,
    basicPersonFields, supportedPersonFields) {
  var expectedFriends = personObject && personObject['__friends__'] ?
      personObject['__friends__'] : [];
  var expectedFields = [];
  for (var field in supportedPersonFields) {
    expectedFields.push(field);
  }
  for (var i = 0; i < expectedFriends.length; i++) {
    // friends with app installed. only the expected has the 'hasPeopleApp' flag
    // the actual may contain only friends with App if hasApp filter is used,
    // thus may contain fewer members than expected
    if (expectedFriends[i]['hasPeopleApp']) {
      for (var j = 0; j < actual.length; j++) {
        if (actual[j].getId() == expectedFriends[i]['id']) {
          Assert.assertPersonFields(result, actual[j], expectedFriends[i],
              expectedFields, supportedPersonFields);
        }
      }
    } else {
      // friends without app installed
      for (var j = 0; j < actual.length; j++) {
        if (actual[j].getId() == expectedFriends[i]['id']) {
          Assert.assertPersonFields(result, actual[j], expectedFriends[i],
              basicPersonFields, supportedPersonFields);
       }
      }
    } // end else
    return result;
  } // end for
}

/**
 * Asserts that the activity object in actual is defined and equal to the
 * activityData activity object. It also asserts that media items field in
 * actual and expected match.
 * @param {Object} actual Activity object which fields will be checked against
 *     the activityData fields.
 * @param {Object} activityData Object with the expected activity data.
 * @param {Array} activityFields contains the activity fields that will be
 *     checked to test equality between actual and activityData.
 * @param {Array} mediaItemFields contains the media item fields that will be
 *     checked to test equality between actual and activityData.
 * @param {ResultGroup} result result object to append verifications to.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertActivityEquals = function(actual, activityData,
    activityFields, mediaItemFields, result) {
    // verifying attributes
  // 0 as sub-index is the last activity created.
  Assert.assertObjectEquals('activity', actual, null, activityData, result);
  Helper.addSubResult(result, 'Activity is Defined', Assert.assertDefined,
      actual, 'Defined');
  if (actual) {
    Assert.assertObjectEquals('activity', actual, activityFields,
        activityData, result);
    var mediaItems = actual.getField('mediaItems');
    Helper.addSubResult(result, 'Media Items defined:', Assert.assertDefined,
        mediaItems, 'Defined');
    var expectedMediaItems = activityData['mediaItems'];
    Helper.addSubResult(result, 'Media Items', Assert.assertTrue,
        mediaItems.length == expectedMediaItems.length, 'true');
    if (mediaItems) {
      for (var i = 0; i < mediaItems.length; i++) {
        Assert.assertObjectEquals('mediaItemField', mediaItems[i],
            mediaItemFields, expectedMediaItems[i], result);
      }
    }
  }
  return result;
};


/**
 * Assertion for view object to check for equality against the expected view
 * object. If there is no expected parameter just log the getName and the
 * isOnlyVisibleGadget results.
 * @param {Object} actual view object to compare with expected.
 * @param {Object} expected view object to compare the actual object against.
 * @param {ResultGroup} result result object to append verifications to.
 */
Assert.assertViewEquals = function(result, actual, expected) {
  Helper.addSubResult(result, 'getCurrentView()', Assert.assertDefined, actual
      , 'defined');
  if (actual) {
    if (expected) {
      Helper.addSubResult(result, 'view.getName()', Assert.assertEquals
          , actual.getName(), expected['getName']);
      Helper.addSubResult(result, 'view.isOnlyVisibleGadget()',
          Assert.assertDefined, actual.isOnlyVisibleGadget(), 'defined');
    } else {
      Helper.addUnverifiedResult(result, 'view.getName()', actual.getName());
      Helper.addUnverifiedResult(result, 'view.isOnlyVisibleGadget()',
          actual.isOnlyVisibleGadget());
    }
  }
};


/**
 * Assertion for environment object to check for equality against the
 * expected environment object. For the supported views in the environment
 * asserts for equality against the expected environment views.
 * @param {Object} actual environment object to compare with expected.
 * @param {Object} expected environment object to compare the actual object
 *     against.
 * @param {ResultGroup} result result object to append verifications to.
 * @param {String} objectAlias name for the subresults.
 */
Assert.assertEnvironmentEquals = function(result, objectAlias, actual,
    expected) {
  Helper.addSubResult(result, 'environment is defined', Assert.assertDefined
      , actual, 'defined');

  if (actual) {
    if (expected && expected['domain']) {
      Helper.addSubResult(result, 'environment.getDomain()', Assert.assertEquals
        , actual.getDomain(), expected['domain']);
    } else {
      Helper.addSubResult(result, 'environment.getDomain()', Assert.assertDefined
        , actual.getDomain(), 'defined');
    }

    if (gadgets.util.hasFeature('views')) {
      var supportedViews = gadgets.views.getSupportedViews();
      Helper.addSubResult(result, 'views.getSupportedViews()'
          , Assert.assertDefined, supportedViews, 'defined');

      var count = 0;
      if (supportedViews) {
        for (var viewName in supportedViews) {
          var view = supportedViews[viewName];
          Helper.addSubResult(result, 'views :' + view
              , Assert.assertDefined, view, 'defined');
          count++;
        }
        Helper.addSubResult(result, 'views.getSupportedViews() count'
            , Assert.assertGreaterThan, count, 0);
      }
    }
  }
};


/**
 * Asserts the environment object env supports the fields specified in fields
 * parameter, also asserts fields that are not supposed to be supported are not.
 * @param {Object} env environment.
 * @param {String} objType Object Type to be verified.
 * @param {Array} fields Object's Fields to verify.
 * @param {ResultGroup} result result object to append verifications to.
 * @return {ResultGroup} result result object with appended verifications.
 */
Assert.assertEnvironmentSupportedFieldsEquals = function(env, objType, fields,
                                                         result) {
  for (var i = 0 ; i < fields.length ; i++) {

    if (env.supportsField(objType,fields[i])) {
      Helper.addSubResult(result, 'Field: ' + fields[i] + ' suported ?',
          Assert.assertTrue, env.supportsField(objType, fields[i]), 'True');
    } else {
      Helper.addSubResult(result, 'Field: ' + fields[i] + ' suported ?',
          Assert.assertFalse, env.supportsField(objType, fields[i]), 'False');
    }

  }
  // this field must not be supported because it doesnt exist.

  Helper.addSubResult(result, 'Field: NOT_EXIST_FIELD suported ?',
      Assert.assertFalse, env.supportsField(objType, 'NOT_EXIST_FIELD'),
      'false');


  return result;
};


/**
 * Wrapper assertion method for asserting the activity is defined.
 * @param {DataResponse} dataResp response.hadError() value.
 * @param {String} key for fetching activities from response.
 * @param {ResultGroup} result object for setting the test result.
 * @return {boolean} true for success, false for failure.
 */
Assert.assertActivityDefined = function(dataResp, key, result) {
  Assert.assertDataResponseHadError(dataResp, result, null, false);

  if (!dataResp.hadError()) {
    var actual = dataResp.get(key).getData()[key];
    Helper.addSubResult(result, 'Activities', Assert.assertDefined, actual,
        'defined');
  }
  return result.success;
};

/**
 * Asserts the actual and expected JSON Objects are equal.
 * @param {String} actual JSON String to be compared against expected.
 * @param {String} expected JSON String to be compared against actual.
 * @return {boolean} true if they are equal, false if they are not.
 */
Assert.assertJsonEquals = function(actual, expected) {
  for (var key in actual) {
    var result = false;
    if (expected[key]) {
      if (typeof actual[key] == 'string') {
        result = actual[key] == expected[key];
      } else {
        result = Assert.assertJsonEquals(actual[key], expected[key]);
      }
    }
    if (!result) {
      return false;
    }
  }
  return true;
}


/**
 * Assertion method for asserting if an API object is defined.
 * @param {String} apiObject Contains the API object name, like "opensocial".
 * @return {boolean} true if the API obejct is defined, false otherwise.
 */
Assert.apiObjectDefined = function(apiObject) {
  var splitNames = apiObject.split('.');
  var parentObject = window;
  for (var i = 0; i < splitNames.length; i++) {
    var childObject = parentObject[splitNames[i]];
    if (!childObject) {
      return false;
    } else {
      parentObject = childObject;
    }
  }
  return true;
};

/**
 * Asserts if an array contains a specific object.
 * @param {Array} array The array to be searched in order to find the object.
 * @param {Object} Object The object that will be searched for in the array.
 * @return {boolean} true if the object is in the array, false if it is not.
 */
Assert.arrayContains = function(array, object) {
  if (array) {
    for (var i = 0; i < array.length; i++) {
      if (array[i] == object) {
        return true;
      }
    }
  }
  return false;
}

/**
 * Comparator methods for verifying sorting.
 */
Assert.Comparator = {
  IdComparator : function(a, b) {
    aname = a.hasOwnProperty('getId') ? a['getId'] : a.getId();
    bname = b.hasOwnProperty('getId') ? b['getId'] : b.getId();
    return (aname > bname) ? 1 : (aname < bname) ? -1 : 0;
  },

  NameComparator : function(a, b) {
    aname = a.hasOwnProperty('getDisplayName') ?
        a['getDisplayName'] : a.getDisplayName();
    bname = b.hasOwnProperty('getDisplayName') ?
        b['getDisplayName'] : b.getDisplayName();
    return (aname > bname) ? 1 : (aname < bname) ? -1 : 0;
  },

  TopFriendsComparator : function(a,b) {
    if (!a || !b) {
      return 0;
    }
    aname = a.hasOwnProperty('topFriendOrder') ?
           a['topFriendOrder'] : a.getDisplayName();
    // TODO (pechague) see what to do in case of no propertie.
    bname = b.hasOwnProperty('topFriendOrder') ?
           b['topFriendOrder'] : b.getDisplayName();
    return (aname > bname) ? 1 : (aname < bname) ? -1 : 0;
  }
};
