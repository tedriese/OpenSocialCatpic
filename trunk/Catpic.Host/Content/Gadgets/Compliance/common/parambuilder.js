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
 * @fileoverview Parameter builder.
 *
 * <p>A Testing Utility library that allows invoking a function using all its
 * posible parameters.</p>
 * <p>This file contains two classes the first being the container for the
 * parameters thus called ParamSet and the second being an iterator that will
 * be used to generate the different combinations thus called iterator.</p>
 * To use this utility we must do the following:<br>
 * <ul>
 *  <li>Create a ParamSet and fill the parameters in.</li>
 *  <li>Execute our function through the ParamSet.execEach() method.</li>
 *  <li>Create callbacks to control when the function finishes successfuly and
 *      when it fails due to an exception.</li>
 * </ul>
 * <p>In order to fill the ParamSet we need to take into account the order and
 * position of the parameters as they will be consistently used by the
 * ParamSet.
 * Look at the following function definition:
 * <code>function doSomething(a, b, c)</code>
 * Let's assume we want to test this class, the first parameter receives a
 * number, the parameter b receives a string, and the parameter c receives a
 * string or nothing (it is optional).<br>
 * First we need to know wich position do parameters have. The position is a
 * 0 based index that is just the order of the parameter declaration, that is
 * parameter a is in position 0, parameter b is in position 1 and parameter c is
 * in position 2.<br>
 * Now we need to fill the ParamSet.
 * <code>
 * var paramSet = new ParamBuilder.ParamSet();
 * paramSet.addParam(0, 20);
 * paramSet.addParam(0, 99);
 * paramSet.addParam(0, 0);
 * paramSet.addParam(1, 'Some string');
 * paramSet.addParam(1, 'Another string');
 * paramSet.addParam(2, null, 'NULL');
 * paramSet.addParam(2, 'The optional String');
 * </code>
 * </p>
 * <p>In this case the addParam function receives 3 parameters. First the
 * position of the parameter and second the value, the order in wich the
 * parameters are added defines the order in wich they will be iterated also,
 * and this order is known as index.<br>
 * The index is a 0 based number that is dependent on the position, this means,
 * there is an index per position. So in this case the parameter 'some string'
 * is in position 1 and index 0, the parameter 20 is in position 0 index 0,
 * the parameter 'The optional String' is in position 0 index 1.<br>
 * This is important since sometimes we want to put assertions or custom
 * behaivor depending on the parameter sent, so using the indices rather than
 * the values might be an easier and less expensive (comparing numbers instead
 * of comparing objects) aproach.<br>
 * The third and optional parameter opt_label for the addParam function just
 * gives an alias that will be used for logging purposes, It is posible to put
 * any string there and it will be used in the getStringAt method instead of the
 * actual object, This optional parameter does not have any impact in the
 * invocation of the function or the generation of the iterator. In the example
 * for the parameter in position 2, index 0 (null) it has a string alias of NULL
 * this alias will only be used when logging, and it comes in handy when using
 * large objects or complex structures as parameters as something more
 * descriptive can be logged instead of a simple "object" or "function"
 * string.</p>
 * <p>Now to do the actual invocation it is only neccesary to do:
 * <code>
 * paramSet.execEach(null, doSomething);
 * </code>
 * Aditionally if we want to know the state of the function after invoked we can
 * add two callbacks as parameters that will receive the result of the function
 * or the exception, as well as the indices of the positions used, for example:
 * <code>
 * paramSet.execEach(null, doSomething,
 * function success(resultValue, indexArray, paramSet) {
 * // The call succeded and we might have some result value
 * // The callback code here
 * },
 * function errorHappened(error, indexArray, paramSet){
 * // Something bad happened and an error was thrown.
 * // The error callback code here
 * });
 * </code>
 * The interesting part comes in the indexArray which is formed of the indices
 * from each parameter used. For example the first invocation will have an
 * indexArray of [0, 0, 0] while the last will have [2, 1, 1].<br>
 * So going back to the concepts of position and index, an array like [1, 0, 1]
 * means that for position 0 the index 1 was used, for position 1 the index 0
 * was used, for position 2 index 1 was used. this translates in the following
 * parameter values: 99, 'Some string', 'The optional String'.<br>
 * So the actual function call that will originate either of the callbacks was:
 * <code>doSomething(99, 'Some string', 'The optional String');</code>
 * Then from those indices we can know the values of the parameters used using
 * the paramSet, or if we did the call we might as well use the indices to
 * compare relevant data and do some interesting conditional behaivor.</p>
 * <p> Aditionally some manual operations can be done over this ParamSet using
 * the iterator, the possibilities can be for example dynamic object
 * generation.</p>
 */

// Namespace
var ParamBuilder = {};

/**
 * Class ParamBuilder.ParamSet, Base class for creating sets of parameters,
 * controls the generation of parameter combinations and stores its values and
 * aliases (labels).<br>
 * Note that this ParamSet will not allow duplicates in the same position, this
 * means that a parameter cannot be added twice in the same position but might
 * be added in two different positions.
 * @constructor
 */
ParamBuilder.ParamSet = function() {
  /**
   * A two-dimensional array that contains the parameter sets.
   * @type {Array.<Array.<*>>}
   */
  this.params_ = [];

  /**
   * A two-dimensional array that stores the labels of the parameters. This is
   * used in order to have aliases for the parameters that can be logged easily.
   * @type {Array.<Array.<String>>}
   */
  this.labels_ = [];
};

/**
 * Adds a parameter value to the given position.
 * @param {number} position The position of the value.
 * @param {*} value The actual value of the parameter that can be used latter on
 *     to invoke the function.
 * @param {string} opt_label An optional label(alias) for this value.
 * @return {number} The added parameter index or -1 if the parameter couldn't be
 *     added.
 */
ParamBuilder.ParamSet.prototype.addParam = function(position, value,
    opt_label) {
  if (!this.params_[position]) {
    this.params_[position] = [];
    this.labels_[position] = [];
  }
  if (this.hasParam(position, value)) {
    return -1;
  }
  this.params_[position].push(value);
  this.labels_[position].push(opt_label);
  return this.params_[position].size - 1;
};

/**
 * Adds the contents of the array as parameters for the specified position.
 * If a value inside the array can't be added then none of the values are added.
 * If the operation fails (returns false) then it is recomended adding manually
 * (using addParam) the elements of the array.
 * @param {number} position The position of the parameter in the function.
 * @param {Array<*>} paramArray Array of parameters that will be added.
 * @return {boolean} True if all of the contents of the array were added, false
 *     if they couldn't be added.
 */
ParamBuilder.ParamSet.prototype.addAll = function(position, paramArray) {
  for (var i = 0; i < paramArray.length; i++) {
    if (this.hasParam(0, paramArray[i])) {
      return false;
    }
  }
  for (var i = 0; i < paramArray.length; i++) {
    this.addParam(position, paramArray[i]);
  }
  return true;
};

/**
 * Sets a parameter in the given position.
 * @param {number} position The position of the parameter.
 * @param {number} index The index of the parameter.
 * @param {*} value The value of the parameter.
 * @param {string} opt_label An optional label(alias) for this value.
 * @return {boolean} True if the value has been altered, false if not (an
 *     existing value was set).
 */
ParamBuilder.ParamSet.prototype.setParam = function(position, index, value,
    opt_label) {
  if (position >= this.params_.length || index >= this.params_[position].length
      || this.hasParam(position, value)) {
    return false;
  }
  this.params_[position][index] = value;
  this.labels_[position][index] = opt_label;
  return true;
};

/**
 * Removes the given value at the given position.
 * @param {number} position The position where the value is going to be removed.
 * @param {*} value The value of the element to be removed from the set.
 * @return {boolean} True if the value was removed, false if not (it didn't
 *     exist)
 */
ParamBuilder.ParamSet.prototype.removeParam = function(position, value) {
  return this.removeParamAt(position, this.indexOf(position, value));
};

/**
 * Removes a parameter from a specific position and index.
 * @param {number} position The position where the element is.
 * @param {number} index The index of the element to be removed.
 * @return {boolean} True if the value was removed, false if not (it didn't
 *     exist)
 */
ParamBuilder.ParamSet.prototype.removeParamAt = function(position, index) {
  if (position < this.params_.length && index < this.params_[position].length
      && index >= 0 && position >= 0) {
    this.params_[position].splice(index, 1);
    this.labels_[position].splice(index, 1);
    return true;
  }
  return false;
};

/**
 * Gets all the values mapped to the given indices.
 * @param {Array.<number>} indexArray An array containing the indices of the
 *     parameters at their positions.
 * @param {Array.<*>} opt_valueArray An optional array that will be used to
 *     store the values instead of creating a new array each time.
 * @return {Array.<*>} An array containing the values of the parameters mapped
 *     to their respective indices.
 */
ParamBuilder.ParamSet.prototype.getValues = function(indexArray,
    opt_valueArray) {
  var paramValues = opt_valueArray || [];
  for (var i = 0; i < indexArray.length; i++) {
    paramValues[i] = this.getValueAt(i, indexArray[i]);
  }
  return paramValues;
};

/**
 * Gets a string representation of the values in the given indices, this
 * representation is similar of how the parameters are actually used, the
 * resulting string is like: (param1, param2, param3, ...).
 * @param {Array.<number>} indexArray the array of the indices of the values
 *     inside this paramSet.
 * @return {string} A single string representing the values of the parameters
 */
ParamBuilder.ParamSet.prototype.indicesToString = function(indexArray) {
  return '(' + this.getStrings(indexArray).join(', ') + ')';
};

/**
 * Gets an array containing the string representations of the values inside
 * this paramSet at the given indices.
 * @param {Array.<number>} indexArray the array of the indices of the values
 *     inside this paramSet.
 * @param {Array.<string>} opt_stringArray An optional string array to store the
 *     labels of the parameters instead of creating a new one each time this
 *     function is called.
 * @return {Array.<string>} An array containing the string representations of
 *     the values inside this paramSet at the given indices.
 */
ParamBuilder.ParamSet.prototype.getStrings = function(indexArray,
    opt_stringArray) {
  var str = opt_stringArray || [];
  for (var i = 0; i < indexArray.length; i++) {
    str[i] = this.getStringAt(i, indexArray[i]);
  }
  return str;
};

/**
 * Gets a single value at the given position and index.
 * @param {number} position The given position to retrieve the value.
 * @param {number} index The index of the value.
 * @return {*} Gets a value stored in the specified position, index.
 */
ParamBuilder.ParamSet.prototype.getValueAt = function(position, index) {
  return this.params_[position][index];
};

/**
 * Gets a string representation of a value in the given position, index.
 * @param {number} position The given position to retrieve the value.
 * @param {number} index The index of the value.
 * @return {string} The string representation of the value stored at a given
 *     position and index, if the value has a label(alias) assigned then that
 *     label is returned.
 */
ParamBuilder.ParamSet.prototype.getStringAt = function(position, index) {
  if (this.labels_[position][index]) {
    return this.labels_[position][index];
  }
  if (typeof(this.labels_[position][index]) == 'function') {
    return 'function ' + this.labels_[position][index].name;
  }
  if (self.gadgets && gadgets.json && gadgets.json.stringify) {
    return gadgets.json.stringify(this.params_[position][index]);
  }
  if (self.JSON) {
    return JSON.stringify(this.params_[position][index]);
  }
  if (this.params_[position][index].toString) {
    return this.params_[position][index].toString();
  }
  return this.params_[position][index];
};

/**
 * Seeks the value in the given position.
 * @param {number} position The position to search.
 * @param {*} value The value to search.
 * @return {boolean} True if the value was found in the position, false
 *     otherwise. 
 */
ParamBuilder.ParamSet.prototype.hasParam = function(position, value) {
  return this.indexOf(position, value) >= 0;
};

/**
 * Gets the index of the value for the given position or -1 if it was not found.
 * @param {number} position The position to search.
 * @param {*} value The value to search for.
 * @return {number} The index of the value or -1 if none was found.
 */
ParamBuilder.ParamSet.prototype.indexOf = function(position, value) {
  // Old school approach is used since there might actually be a null inside the
  // parameter collection.
  var values = this.params_[position];
  if (!values) {
    return -1;
  }
  for (var i = 0; i < values.length; i++) {
    if (values[i] == value) {
      return i;
    }
  }
  return -1;
};

/**
 * Executes the given function for all the possible permutations of parameters.
 * This function also has the ability to invoke two callbacks upon two possible
 * events. When the function finishes without errors the opt_successCallback is
 * called, this callback receives three parameters, the result of the
 * invocation, the indexArray of the parameters and the ParamSet that was used.
 * If there is an error this function invokes another callback
 * opt_exceptionCallback, this receives three parameters as well, the error
 * thrown, the indexArray of the parameters and the ParamSet that was used.
 * @param {object} thisObj The object that will be used as a THIS reference
 *     inside the function call.
 * @param {Function} funct The function to be invoked.
 * @param {Function(*, Array<number>, ParamBuilder.ParamSet)}
 *     opt_successCallback A callback invoked when the function ends,
 *     the parameters for the callback are: the result of the invocation, the
 *     indexArray and ParamSet that were used in the call.
 * @param {Function(Error, Array<number>, ParamBuilder.ParamSet)}
 *     opt_exceptionCallback A callback invoked when the function ends with an
 *     exception, the parameters for the callback are: the exception thrown,
 *     the indexArray and ParamSet that were used in the call.
 */
ParamBuilder.ParamSet.prototype.execEach = function(thisObj, funct,
    opt_successCallback, opt_exceptionCallback) {
  var iterator = this.iterator();
  var indexArray = [];
  var valuesArray = [];
  while (iterator.next(indexArray)) {
    var success = false;
    var functionResult = null;
    try {
       functionResult = funct.apply(thisObj, this.getValues(indexArray,
           valuesArray));
       success = true;
    } catch(exception) {
      if (opt_exceptionCallback) {
        opt_exceptionCallback(exception, indexArray, this);
      }
    }
    if (opt_successCallback && success) {
      opt_successCallback(functionResult, indexArray, this);
    }
  }
};

/**
 * Gets an iterator to get all possible combinations over the lenght of all the
 * sets inside each position of this ParamSet
 * @return {ParamBuilder.ParamIterator_} an Iterator to get all possible
 *     combinations.
 */
ParamBuilder.ParamSet.prototype.iterator = function() {
  var maxValues = [];
  for (var i = 0; i < this.params_.length; i++) {
    maxValues.push(this.params_[i].length - 1);
  }
  return new ParamBuilder.ParamIterator_(maxValues);
};

/**
 * Class ParamBuilder.ParamIterator_ This class basically generates an array for
 * every iteration containing an increasing counter value that increases from
 * right to left to a maximum amount given in the constructor's parameter
 * maxValues.
 * For example to iterate through numbers 0 to 999 the maxValues array must
 * contain [9, 9, 9]. This means the iterator will go through ALL the numbers
 * of the last index of the array, 0 through 9, then it will increase
 * the value of the following index (from right to left), and so on.<br>
 * So the progression we have at the end is as follows:
 * [0, 0, 0] [0, 0, 1] [0, 0, 2] [0, 0, 3] [0, 0, 4] [0, 0, 5] ... [0, 0, 9]<br>
 * [0, 1, 0] [0, 1, 1] [0, 1, 2] [0, 1, 3] [0, 1, 4] [0, 1, 5] ... [0, 1, 9]<br>
 * [0, 2, 0] [0, 2, 1] [0, 2, 2] [0, 2, 3] [0, 2, 4] [0, 2, 5] ... [0, 2, 9]<br>
 * ...<br>
 * [0, 9, 0] [0, 9, 1] [0, 9, 2] [0, 9, 3] [0, 9, 4] [0, 9, 5] ... [0, 9, 9]<br>
 * [9, 0, 0] [9, 0, 1] [9, 0, 2] [9, 0, 3] [9, 0, 4] [9, 0, 5] ... [9, 0, 9]<br>
 * ...<br>
 * [9, 9, 0] [9, 9, 1] [9, 9, 2] [9, 9, 3] [9, 9, 4] [9, 9, 5] ... [9, 9, 9]<br>
 * While this might be achieved using a simple counter the real usefulness of
 * this iterator is that it can calculate non-standard progressions with
 * irregular max values, this means: instead of having maximum values of
 * [9, 9, 9] we can have [4, 12, 3] and this will go through the values
 * [0, 0, 0] to [4, 12, 3] increasing values from right to left when the
 * rightmost value reaches 3, the second rightest reaches 12 and stop when the
 * leftmost reaches 4 (and the others have reached their max as well).<br>
 * The resulting progression is:<br>
 * [0, 0, 0] [0, 0, 1] [0, 0, 2] [0, 0, 3]<br>
 * [0, 1, 0] [0, 1, 1] [0, 1, 2] [0, 1, 2]<br>
 * ...<br>
 * [0, 11, 0] [0, 11, 1] [0, 11, 2] [0, 11, 3]<br>
 * [0, 12, 0] [0, 12, 1] [0, 12, 2] [0, 12, 3]<br>
 * [1, 0, 0] [1, 0, 1] [1, 0, 2] [1, 0, 3]<br>
 * ...<br>
 * [4, 11, 0] [4, 11, 1] [4, 11, 2] [4, 11, 3]<br>
 * [4, 12, 0] [4, 12, 1] [4, 12, 2] [4, 12, 3]<br>
 * @param {Array.<number>} maxValues The maximum values of the indices to go
 *     through.
 */
ParamBuilder.ParamIterator_ = function(maxValues) {
  this.maxValues_ = maxValues;
  this.currentIndices_ = [];
  for (var i = 0; i < maxValues.length; i++) {
    this.currentIndices_[i] = 0;
  }
};

/**
 * Gets the next element in the collection and advances the iteration pointer by
 * one.
 * @param {Array.<number>} opt_array Optional array to store use instead of
 *     creating a new array.
 * @return {Array.<number>} An array containing the resulting index combination.
 */
ParamBuilder.ParamIterator_.prototype.next = function(opt_array) {
  if (!this.hasNext()) {
    return null;
  }
  var indices = opt_array || [];
  for (var i = 0; i < this.currentIndices_.length; i++) {
    indices[i] = this.currentIndices_[i];
  }
  // Move the pointer forward
  var lastArrayIndex = this.currentIndices_.length - 1;
  this.currentIndices_[lastArrayIndex]++;
  for (var i = lastArrayIndex; i > 0; i--) {
    if (this.currentIndices_[i] > this.maxValues_[i]) {
      this.currentIndices_[i] = 0;
      this.currentIndices_[i-1]++;
    } else {
      break;
    }
  }
  return indices;
};

/**
 * Checks if there are more elements remaining in the collection to be iterated
 * and returns true if they are and false if there are no more elements ahead.
 * @return {boolean} true if there are more elements, false if not.
 */
ParamBuilder.ParamIterator_.prototype.hasNext = function() {
  return this.currentIndices_[0] <= this.maxValues_[0];
};
