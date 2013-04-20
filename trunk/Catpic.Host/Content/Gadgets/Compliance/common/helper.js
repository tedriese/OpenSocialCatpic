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
 * @fileoverview Contains static helper functions for the testing framework
 * common tasks.
 */

// Namespace
var Helper = {};

/**
 * Helper function that returns an iterable object from an array, defined here
 * because it is a pre-requisite for the bind function.
 * @param {*} iterable The object to be converted.
 * @return {Array.<*>} The converted object
 */
var $A = function(iterable) {
  if (!iterable) return [];
  if (iterable.toArray) return iterable.toArray();
  var length = iterable.length || 0, results = new Array(length);
  while (length--) results[length] = iterable[length];
  return results;
}

/**
 * Binds an object to a function so the next time the function is called the
 * object bound to the function act as the "this" object.
 */
Function.prototype.bind = function() {
  if (arguments.length < 2 && typeof arguments[0] == 'undefined') return this;
  var __method = this, args = $A(arguments), object = args.shift();
  return function() {
    return __method.apply(object, args.concat($A(arguments)));
  }
}

/**
 * Logs a message into the result object. This works by adding an extra
 * validation (ResultValidation) to the ResultGroup object with a severity INFO or
 * less.
 * @param {ResultGroup} result The result object that will hold the message.
 * @param {string} text The text to log to the result.
 * @param {*} actual The actual data obtained and that will be logged.
 * @param {*} expected The expected data desired and wished be logged.
 * @param {number} opt_logLevel The severity level or log level that is going to
 *     be used in the log message.
 */
Helper.logIntoResult = function(result, text, actual, expected, opt_logLevel) {
  var loglvl = Result.severity.INFO;
  if (opt_logLevel && opt_logLevel > Result.severity.UNVERIFIED) {
    loglvl = opt_logLevel;
  }
  result.addSubResult(text, null, actual, expected, loglvl);
};

/**
 * Adds an unverified ResultValidation to the ResultGroup.
 * @param {ResultGroup} result The result that will hold the unverified
 *     sub-result.
 * @param {string} text The text that will be logged for the sub-result.
 * @param {*} actual Optional actual data got in the verification.
 * @param {*} expected Optional expected data desired in the sub-result.
 */
Helper.addUnverifiedResult = function(result, text, actual, expected) {
  result.addSubResult(text, null, actual, expected
      , Result.severity.UNVERIFIED);
};

/**
 * Adds a sub-result assertion that if failed it will result in a warning instead
 * of a fail.
 * @param {ResultGroup} result The result to add the sub-result to.
 * @param {string} text The text that will be logged along with the assertion.
 * @param {null, undefined, boolean, function(*, *)} assert The assertion to be
 *     tested as a function or as a boolean value. If given a function it will
 *     be invoked using the actual and expected parameters.
 * @param {*} actual The actual value to assert.
 * @param {*} expected The expected data to be asserted.
 */
Helper.addUnsevereSubResult = function(result, text, assert, actual, expected) {
  result.addSubResult(text, assert, actual, expected, Result.severity.WARNING);
}

/**
 * Adds a sub-result assertion that if failed it will result in a info instead
 * of a fail.
 * @param {ResultGroup} result The result to add the sub-result to.
 * @param {string} text The text that will be logged along with the assertion.
 * @param {null, undefined, boolean, function(*, *)} assert The assertion to be
 *     tested as a function or as a boolean value. If given a function it will
 *     be invoked using the actual and expected parameters.
 * @param {*} actual The actual value to assert.
 * @param {*} expected The expected data to be asserted.
 */
Helper.addInfoSubResult = function(result, text, assert, actual, expected) {
  result.addSubResult(text, assert, actual, expected, Result.severity.INFO);
}

/**
 * Adds a sub-result assertion that if failed will result on a fail.
 * @param {ResultGroup} result The result to add the sub-result to.
 * @param {string} text The text that will be logged along with the assertion.
 * @param {null, undefined, boolean, function(*, *)} assert The assertion to be
 *     tested as a function or as a boolean value. If given a function it will
 *     be invoked using the actual and expected parameters.
 * @param {*} actual The actual value to assert.
 * @param {*} expected The expected data to be asserted.
 * @oaram {Result.severity} opt_severity optional severity level to log
 */
Helper.addSubResult = function(result, text, assert, actual, expected,
    opt_severity) {
  if (assert == undefined || assert == null) {
    Helper.addUnverifiedResult(result, text, actual, expected);
  } else {
    result.addSubResult(text, assert, actual, expected, opt_severity);
  }
};

/**
 * Checks if object with given name exists on container. If not returns
 * available options else returns given string.
 * @param {string} apiObject object whose existence we are validating
 */
Helper.getObjectOptions = function(apiObject) {
  var splitNames = apiObject.split('.');
  var parentObject = window;
  var actualObject = [];
  for (var i = 0; i < splitNames.length; i++) {
    var childObject = parentObject[splitNames[i]];
    if (!childObject) {
      var availableObjects = [];
      for (var object in parentObject) {
        availableObjects.push(
            actualObject.join('.') + '.' + parentObject[object]);
      }
      return availableObjects.join(', ');
    } else {
      parentObject = childObject;
      actualObject.push(splitNames[i]);
    }
  }
  return actualObject.join('.');
};

/**
 * Converts a javascript error into an HTML version that can be used for
 * printing it in a HTML document.
 * @param {error} ex The error to be converted to HTML.
 * @return {string} The HTML representation of the javascript error.
 */
Helper.getExceptionHtml = function(ex) {
  ex = ex || {};
  var name = ex.name || '';
  var fileName = ex.fileName || '';
  var line = ex.lineNumber || '';
  var msg = ex.message || '';
  var stacktrace = ex.stack || '';
  while (stacktrace.indexOf('\n') > -1) {
    stacktrace = stacktrace.replace('\n','<br>');
  }
  return 'Exception: ' + name + ' - File: ' + fileName
      + ' - Line:(' + line + ') - Msg:' + msg  + '<br><span '
      + ' class="exceptionStack">Stacktrace: ' + stacktrace + '</span>';
}

/**
 * Add a sub-result containing an exception stack trace and an error to the test
 * result object.
 * @param {ResultGroup} result The result to add the exception stack trace to.
 * @param {Test, null} test The test object to use its id for logging, or null
 *     to if unknown or not specified.
 * @param ex The error thrown.
 */
Helper.logException = function(result, test, ex) {
  var exception =  Helper.getExceptionHtml(ex);
  result.addSubResult('EXCEPTION THROWN AT:' + (test ? test.id : ''), false,
      exception, 'No Exception');
};

/**
 * Creates a callback that will run after the validation has been verified, this
 * method should be called if the pre-validation -> run -> [responseKeys]
 * -> validation test approach is used. This method is usually called at the
 * end of the run method in order to check the responseKeys upon the end
 * of the callback and to call after the validation method in the test object.
 * @param {Test} test The test that we are adding the callback to.
 * @param {Function} resultsCallback The callback being used for the results.
 * @param {Context} context The context object that is going to be used in the
 *     call.
 * @param {opt_params} optional parameters to pass
 * @return {Function} Returns the validation function bound to the test object.
 */
Helper.createValidationCallback = function(test, resultsCallback, context,
    result, opt_params) {
  // Create and return the generic validation callback.
  var validation_callback = function(dataResponse) {
    try {
      // Log keys.
      if (test.responseKeys) {
        Assert.logResponseItems(dataResponse, result, test.responseKeys);
      }
      // Call test custom validation callback.
        test.validation(
            dataResponse, result, context, resultsCallback, opt_params);
    } catch(e) {
      Helper.logException(result, test, e);
    }
      resultsCallback(result);
  }
  return validation_callback.bind(test);
};

/**
 * Compares two results returning an object with the differences found in them.
 * This function also goes over the verifications of the test object to spot
 * differences inside.
 * @param {ResultGroup} previous A result object to be compared.
 * @param {ResultGroup} current The other result object to compare.
 * @return {object} A map containing the differences between both results.
 */
Helper.compareResults = function(previous, current) {
  var comparisonMap = {};
  for (var test in previous) {
    if (current[test].success != previous[test].success) {
      // Difference in head test
      comparisonMap[test] = true;
    }
    if (previous[test].verifications.length
        != current[test].verifications.length) {
      // Difference in verifications count
      comparisonMap[test] = true;
    }
    for (var i = 0; i < previous[test].verifications.length; i++) {
      if (previous[test].verifications[i] != current[test].verifications[i]) {
        comparisonMap[test] = true;
      }
    }
  }
  return comparisonMap;
}

/**
 * Gets the golden results flag from the configuration for the given suite.
 * @param {string} suiteId The id of the suite.
 * @return {boolean} True if there is a flag for this suite false otherwise.
 */
Helper.getGoldenResultsFlag = function(suiteId) {
  return 'ALL' == Config.saveGoldenResultsForSuite
      || suiteId == Config.saveGoldenResultsForSuite;
}

/**
 * Saves the golden results and calls the callback function upon finish.
 * @param {string} suiteId The suite id.
 * @param {object} resultsMap The resultMap that will be saved.
 * @param {function} callback The function to be called once the results are
 *     saved
 */
Helper.saveGoldenResults = function(suiteId, resultsMap, callback) {
  var resultsMapAsString = gadgets.json.stringify({suiteResults: resultsMap});
  var goldenResultsKey = suiteId + '_results_' + Config.goldenResultsLabel;
  var req = opensocial.newDataRequest();
  req.add(req.newUpdatePersonAppDataRequest(
      opensocial.DataRequest.PersonId.VIEWER,
      goldenResultsKey, resultsMapAsString, 'updateResults'));
  req.send(function(dataResponse) {
    if (dataResponse.hadError()) {
      alert('Unable to save results: ' + dataResponse.getErrorMessage());
    }
    if (callback) {
      callback();
    }
  });
}

/**
 * Converts an Array of ResultGroups to a simple object containing the results
 * of all the Results and its validations in a simple format that could be used
 * for serialization. This is simpler version of {@link #Helper.convertResultsToMap}
 * which excludes minute verification details.
 * @param {Array.<ResultGroup>} results The results to convert.
 * @return {Object} A map containing all the results in a simpler version.
 */
Helper.convertResultsToMapExcludeVerifications = function(resultSets) {
  var resultSetsMap = {};
  for (var i = 0; i < resultSets.length; i++) {
    resultSetsMap[resultSets[i].id] = {};
    var results = resultSets[i].results;
    var resultsMap = resultSetsMap[resultSets[i].id];
    for (var j = 0; j < results.length; j++) {
      resultsMap[results[j].id] = {};
      resultsMap[results[j].id].result = Result.severity.getString(results[j].severity);
      if (results[j].delta == true) {
        resultsMap[results[j].id].delta = results[j].delta;
      }
    }
  }
  return resultSetsMap;
}

/**
 * Converts an Array of ResultGroups to a simple object containing the results
 * of all the Results and its validations in a simple format that could be used
 * for serialization.
 * @param {Array.<ResultGroup>} results The results to convert.
 * @return {Object} A map containing all the results in a simpler version.
 */
Helper.convertResultsToMap = function(results) {
  var resultsMap = {};
  for (var i = 0; i < results.length; i++) {
    resultsMap[results[i].id] = {};
    resultsMap[results[i].id].success = results[i].success;
    resultsMap[results[i].id].verifications = [];
    for (var j = 0; j < results[i].verifications.length; j++) {
      var success = results[i].verifications[j].success;
      if (typeof(success) == 'boolean') {
        resultsMap[results[i].id].verifications.push(success);
      } else {
        resultsMap[results[i].id].verifications.push('ignored');
      }
    }
  }
  return resultsMap;
}

/**
 * Searches an array for a given object and returns its index inside the array.
 * @param {Array.<*>} theArray The array to search.
 * @param {*} theObj The object or value to look for.
 * @return {number} The index of the object in the array.
 */
Helper.indexOf = function(theArray, theObj) {
  if (theArray != undefined && theArray instanceof Array) {
    for (var i=0; i < theArray.length; i++) {
      if (theArray[i] == theObj) {
        return i;
      }
    }
  }
  return -1;
}

/**
 * Create a zero padded string. This is specially useful to create zero leading
 * numbers for example to create the string 003 then this function need only to
 * receive parameters (3, 3) the first one is the value to be converted to a
 * string the second one is the size of the string, so the number 3 will be
 * padded until its size is 3 characters.
 * Note that if the string representation of original is already of the target
 * size no padding will occur.
 * @param {*} original The original value to pad.
 * @param {number} size The targeted size of the string.
 * @return {string} The zero padded string of the original value.
 */
Helper.padZeros = function(original, size) {
  original = '' + original;
  while (original.length < size) {
    original = '0' + original;
  }
  return original;
};

/**
 * Filter the unsupported fields from an object using the
 * environment.supportsField function if a field is not supported then its not
 * added to the resulting object. The fields that are supported will be included
 * in the resulting object.
 * @param {Object} object The object that will be filtered.
 * @param {String} type The type that will be used as the first argument for the
 *     environment.supportsField function, refer to the
 *     environment.supportsField documentation for the possible values for this
 *     parameter.
 * @return {object} A clonned and filtered object containing only the supported
 *     fields.
 */
Helper.filterSupportedFields = function(object, type) {
  if (opensocial) {
    var result = {};
    var env = opensocial.getEnvironment();
    for (var propertyName in object){
      if (env.supportsField(type, propertyName)) {
        result[propertyName] = object[propertyName];
      }
    }
    return result;
  } else {
    return object;
  }
}

/**
 * Checks if an object is empty, an object is considered empty if it doesn't
 * have any property, or all the properties values are empty.
 * @param {object} object The object to check if it is empty or not.
 * @return {boolean} True if the object is empty false if it has at least one
 *     property with a significant value
 */
Helper.isObjectEmpty = function(object) {
  if (object) {
    for (var propertyName in object) {
      if (object[propertyName]) {
        return false;
      }
    }
  }
  return true;
}

/**
 * Merges an array of objects into a single object that contains the union of
 * all the properties inside. If two or more objects has the same property name
 * then the values will be overwritten.
 * @param {Array.<object>} objectArray The objects that are going to be merged.
 * @return {object} A single object containing all the properties from the
 *     objects inside the array.
 */
Helper.mergeObject = function(objectArray) {
  var result = {};
  for (var i = 0; i < objectArray.length; i++) {
    var obj = objectArray[i];
    if (obj) {
      for (var propertyName in obj) {
        result[propertyName] = obj[propertyName];
      }
    }
  }
  return result;
}

/**
 * Clones an object removing all the functions inside leaving only the values.
 * @param {object} obj the object to be clonned.
 * @result {object} An object with all the functions stripped.
 */
Helper.cloneObjectNoFunctions = function(obj) {
  if (obj && typeof(obj) == 'object') {
    if (obj instanceof Array) {
      var result = [];
      for (var i = 0; i < obj.length; i++){
        result[i] = obj[i];
      }
      return result;
    } else {
      var result = {};
      for (var propertyName in obj) {
        if (typeof(obj[propertyName]) == 'function') {
          continue;
        }
        result[propertyName] = obj[propertyName];
      }
      return result;
    }
  } else {
    return obj;
  }
}

/**`
 * Clones an object recursively.
 * @param {Object} x Object to clone.
 * @return {Object} the cloned object.

 */
Helper.cloneObject = function(x) {
  if ((typeof x) == "object" && x != null) {
    var y = [];
    for (var i in x) {
      y[i] = Helper.cloneObject(x[i]);
    }
    return y;
  }
  return x;
}

/**
 * Gets a string representation of a value.
 * @param {*} value.
 * @return {string} The string representation of the value.
 */
Helper.getString = function(value) {
  if (typeof(value) == 'function') {
    return 'function ' + value.name;
  }
  if (typeof(value) == 'object') {
    if (self.gadgets && gadgets.json && gadgets.json.stringify) {
      return gadgets.json.stringify(value);
    }
    if (self.JSON) {
      return JSON.stringify(value);
    }
    if (value.toString) {
      return value.toString();
    }
  }
  return '' + value;
};

/**
 * Gets a string representation of a value.
 * @param {*} value.
 * @return {string} The string representation of the value.
 */
Helper.stringify = function(value) {
  if (self.gadgets && gadgets.json && gadgets.json.stringify) {
    return gadgets.json.stringify(value);
  }
  if (self.JSON) {
    return JSON.stringify(value);
  }
  if (value.toString) {
    return value.toString();
  }
  return '' + value;
};

/**
 * Gets a simple representation of a value.
 * @param {*} value.
 * @return {string} The string representation of the value.
 */
Helper.getSimpleString = function(value) {
  if (value == undefined) {
    return 'UNDEFINED';
  }
  if (value == null) {
    return 'NULL';
  }
  if (value.toString) {
    return value.toString();
  }
  return '' + value;
};

/**
 * Creates a new FunctionRunner object.
 * A function runner object is an utility object that can be used to invoke
 * various functions sequentially, additionally this functions called might have
 * callbacks that when called indicates that the function has finished, this
 * kind of behavior allows the FunctionRunner to run asynchronous functions
 * sequentially only when they end by calling the callback.<br>
 * To better understand how this works imagine the case where you want to invoke
 * the following function twice and synchronously:<br>
 * <code>function doAjax(url, callback)</code>
 * This call will receive a URL and then after it gets a response it will call a
 * callback.<br>
 * if we call this function two times:<br>
 * <code>
 * doAjax('firstURL', function(){});
 * doAjax('secondURL', function(){});
 * </code>
 * The order of the callbacks are not assured, so we cannot rely on the order of
 * the callbacks. How can we test this behavior then?<br>
 * The simplest way is to use the sequencer inside this class.<br>
 * <code>
 * var runner = new FunctionRunner();
 * runner.add(doAjax, ['firstURL', runner.getSequencer]);
 * runner.add(doAjax, ['secondURL', runner.getSequencer]);
 * runner.run();
 * </code>
 * Although the runner.run method might return almost immediately something has
 * changed, both doAjax calls are made synchronously. This is because instead
 * of having a dummy callback function the callback function that is used is the
 * sequencer, every time the sequencer is called the next function in the runner
 * is run.<br>
 * If the sequencer is not in the parameter list of a function then it is
 * assumed the call is synchronous by nature and then automatically moves to the
 * next function when it ends.<br>
 * Note that the functionRunner is a "one use: object this means that calling
 * the run method twice will do nothing and can actually harm the object, so
 * if calling the run method more than one time is needed then it is recommended
 * to clone the object and running the run method again from the clone.
 * @constructor
 */
Helper.FunctionRunner = function() {
  this.functions = [];
  this.index = 0;
  this.sequenceFunct_ = null;
  this.lastResult = null;
}

/**
 * Checks if there are functions inside this runner.
 * @return {boolean} True if there are no functions inside, false if there is
 * at least one function inside.
 */
Helper.FunctionRunner.prototype.isEmpty = function() {
  return this.functions.length == 0;
}

/**
 * Runs all the functions inside this runner.
 */
Helper.FunctionRunner.prototype.run = function() {
  this.index = 0;
  this.sequencer();
}

/**
 * Gets the sequencer function for this function runner.
 */
Helper.FunctionRunner.prototype.getSequencer = function() {
  if (!this.sequenceFunct_) {
    this.sequenceFunct_ = this.sequencer.bind(this);
  }
  return this.sequenceFunct_;
}

/**
 * Sequencer function, this function launches the next function in the queue.
 */
Helper.FunctionRunner.prototype.sequencer = function() {
  if (this.index < this.functions.length) {
    var funct = this.functions[this.index];
    this.index++;
    /*
     * If sequencer is in the parameter list it means it is an async call and
     * most likely this function will wait or call it at the end.
     */
    if (funct.hasParam(this.getSequencer())) {
      this.lastResult = funct.run();
    } else {
      /*
       * The function doesn't have the sequencer inside indicating it is just
       * a normal function (does not have the sequencer callback),
       * so we call the sequencer right away after the function ends to continue
       * the chained invocations or end the sequence right away.
       */
      this.lastResult = funct.run();
      this.sequencer();
    }
  }
}

/**
 * Adds a function to this function runner.
 * @param {Function} funct The function to add to the function runner.
 * @param {Array.<*>} params The parameters to use while invoking this function.
 * @param {object, null} opt_thisObj An optional object to use as 'this'
 *     reference inside the function call.
 */
Helper.FunctionRunner.prototype.add = function(funct, params, opt_thisObj) {
  this.functions.push(
      new Helper.FunctionRunner.FunctionData(funct, params, opt_thisObj));
}

/**
 * Clones this function runner object with all its contents.
 * @return {Helper.FunctionRunner} A clone of the original FunctionRunner.
 */
Helper.FunctionRunner.prototype.clone = function() {
  var clone = new Helper.FunctionRunner();
  for (var i = 0; i < this.functions.length; i++) {
    clone.add(this.functions[i].funct, this.functions[i].params
        , this.functions[i].thisObj);
  }
  return clone;
}

/**
 * Function data object, used to store the data required to run the function.
 * @param {Function} funct The function to be stored and called.
 * @param {Array.<*>} params The parameters that are going to be used in the
 *     invocation.
 * @param {object, null} opt_thisObj The 'this' object used in the invocation of
 *     the function
 * @constructor
 */
Helper.FunctionRunner.FunctionData = function (funct, params, opt_thisObj) {
  this.funct = funct;
  this.params = params;
  this.thisObj = null;
  if (opt_thisObj) {
    this.thisObj = opt_thisObj;
  }
}

/**
 * Checks if the function has a parameter inside.
 * @param {*} param The parameter to look up.
 * @return {boolean} True if the parameter is inside the parameter array of the
 *     function.
 */
Helper.FunctionRunner.FunctionData.prototype.hasParam = function(param) {
  for (var i = 0; i < this.params.length; i++) {
    if (this.params[i] == param) {
      return true;
    }
  }
  return false;
}

/**
 * Runs the function with the parameters stored in this object and the this
 * reference given.
 */
Helper.FunctionRunner.FunctionData.prototype.run = function () {
  return this.funct.apply(this.thisObj, this.params);
}
