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
 * @fileoverview Defines TestData namespace and contains constructors
 * for the different data entities. It also contains static Helper methods.
 */

var TestData = function() {};

/**
 * Creates a Person object, that represents a user. Contains the basic fields
 * that are common to all containers, and a collection of the user's friends.
 *
 * @param {string} Person's display name.
 * @constructor
 */
TestData.Person = function(displayName) {
  this.getDisplayName = displayName;
  this.name = {
    familyName : '',
    givenName : '',
    unstructured : displayName
  };
  this.id = '';
  this.getId = '';
  this.profileUrl = '';
  this.thumbnailUrl = undefined;
  this.isViewer = false;
  this.isOwner = false;
  this.__friends__ = undefined;

  // Has people.xml installed.
  this.hasPeopleApp = false;
  // Has appData.xml installed.
  this.hasAppDataApp = false;
  
  // 1 for best friend, 2 for good friend, 3 for friend, 4 for acquaintance
  // 5 for stranger.  used for sorting according to TOP_FRIENDS
  this.topFriendOrder = 5;

  // Assertion methods to use for verification.
  this.__rules__ = {'name' : 'Assert.assertNameEquals' ,
    'profileUrl' : 'Assert.assertValidUrl',
    'id' : 'Assert.assertDataEquals',
    'thumbnailUrl':'Assert.assertValidUrl'
  };
}

/**
 * Creates a Container object. Defines the container's name and contains a
 * collection of the features that it supports.
 *
 * @param {string} The Container's name.
 * @contructor
 */
TestData.Container = function(name) {
  this.name = name;
  this.supportedFeatures = {};
  this.supportsFeature = function(feature) {
    if (this.supportedFeatures[feature]) {
      return this.supportedFeatures[feature];
    }
    return false;
  }
}

/**
 * Creates a Fields collection. Contains several fields definitions from the
 * OpenSocial spec, as well as activities and media items fields collections.
 * It also creates a collection of the fields supported by the current
 * container, with the data retrieved by the environment.
 *
 * @constructor
 */
TestData.Fields = function() {
  this.basicPersonFields = ['id', 'name'];
  this.personFields = TestData.Helper.getObjectFields(opensocial.Person.Field);
  this.supportedPersonFields = TestData.Helper.getSupportedPersonFields();

  this.addressFields =
      TestData.Helper.getObjectFields(opensocial.Address.Field);
  this.bodyTypeFields =
      TestData.Helper.getObjectFields(opensocial.BodyType.Field);
  this.organizationFields =
      TestData.Helper.getObjectFields(opensocial.Organization.Field);
  this.emailFields = TestData.Helper.getObjectFields(opensocial.Email.Field);
  this.phoneFields = TestData.Helper.getObjectFields(opensocial.Phone.Field);
  this.urlFields = TestData.Helper.getObjectFields(opensocial.Url.Field);
  this.nameFields = TestData.Helper.getObjectFields(opensocial.Name.Field);

  this.address = TestData.Helper.getEmptyObject(this.addressFields);
  this.bodyType = TestData.Helper.getEmptyObject(this.bodyTypeFields);
  this.organization = TestData.Helper.getEmptyObject(this.organizationFields);

  this.activityFields = TestData.Helper.getObjectFields(
      opensocial.Activity.Field);

  /**
   * Sets the viewer depending on the opensocial version it's running
   */
  if (gadgets && gadgets.util.hasFeature('opensocial-0.7')) {
    this.mediaItemFields = TestData.Helper.getObjectFields(
        opensocial.Activity.MediaItem.Field);
  } else if (gadgets && gadgets.util.hasFeature('opensocial-0.8')) {
    this.mediaItemFields = TestData.Helper.getObjectFields(
        opensocial.MediaItem.Field);
  }
}

/**
 * Creates the Environment object, that contains specific information about
 * domain and views.
 *
 * @constructor
 */
TestData.Environment = function() {
  this.defaultView = {
    "getName" : "default",
    "isOnlyVisibleGadget" : false
  };
  this.canvasView = {
    "getName" : "canvas",
    "isOnlyVisibleGadget" : true
  };
  this.profileView = {
    "getName" : "profile",
    "isOnlyVisibleGadget" : false
  };
  this.fullPageView = this.canvasView;
  this.dashBoardView = this.defaultView;
}

/**
 * Creates the Activity object, that contains the basic fields needed to
 * create an OpenSocial activity.
 *
 * @constructor
 */
TestData.Activity = function() {
  this.title = 'created: activity1';
  this.url = Config.rootPath;
  this.body = 'activity1 body';
  this.id = 'non-empty id';
  this.postedTime = 'non-empty timestamp';

  var imageItemData = new MediaItemData(
      'gif', 'common/content/orkut20x20.gif', 'image', {'type':'image'});
  var audioItemData = new MediaItemData(
      'wav', 'common/content/beep.wav', 'audio', {'type':'audio'});
  var videoItemData = new MediaItemData(
      'asf', 'common/content/eddie.asf', 'video', {'type':'video'});
  this.mediaItems = [imageItemData, audioItemData, videoItemData];

  this.customValues = {'appleK1':'appleV1', 'appleK2':'appleV2'};
  this.userId = '{viewerId}';
  this.appId = '{appId}';
  this.streamTitle = 'apple stream';
  this.streamUrl = Config.rootPath;
  this.streamSourceUrl = 'source.stream.com';
  this.streamFaviconUrl = Config.rootPath + 'common/content/orkut20x20.gif';
  this.getId = 'non-empty id';
  this.__rules__ = {
    id : 'Assert.assertNotEmpty',
    userId : 'Assert.assertNotEmpty',
    appId : 'Assert.assertNotEmpty',
    getId : 'Assert.assertNotEmpty',
    customValues : 'Assert.assertNotEmpty',
    toServerJson : 'Assert.assertNotNull',
    postedTime : 'Assert.assertNotEmpty'
  }
}

/**
 * Creates an instance of a MediaItemData object.
 *
 * @constructor
 */
TestData.MediaItemData = function(mimeType, url, type, opt_params) {
  this.mimeType = mimeType;
  this.url = Config.rootPath + url;
  this.type = type;
  this.opt_params = opt_params;
}

/**
 * Namespace to group all helper methods used for the test data manipulation.
 */
TestData.Helper = function() {
};

/**
 * Returns an array with all the Person object's fields supported by the
 * current container.
 */
TestData.Helper.getSupportedPersonFields = function() {
  var supportedPersonFields = [];
  for (var field in opensocial.Person.Field) {
    if (opensocial.getEnvironment().supportsField(
        opensocial.Environment.ObjectType.PERSON,
        opensocial.Person.Field[field])) {
      supportedPersonFields.push(opensocial.Person.Field[field]);
    }
  }
  return supportedPersonFields;
}

/**
 * Returns an array with all the fields defined for the object
 * passed as paramater.
 *
 * @param {Object} the object to extract the fields from.
 */
TestData.Helper.getObjectFields = function(objName) {
  var fields = [];
  if (typeof(objName) != 'object')
    return fields;
  for (var field in objName) {
    fields.push(objName[field]);
  }
  return fields;
};

/**
 * Creates and returns an object that includes fields with empty values.
 * The fields to be included in the object are specified in the array passed
 * as parameter.
 *
 * @param {Array} list of strings containing the field names to be added to
 * the object being created.
 */
TestData.Helper.getEmptyObject = function(fieldsArray) {
  var obj = {};
  if (!(fieldsArray && fieldsArray.length)) {
    return obj;
  }
  for (var i = 0; i < fieldsArray.length; i++) {
    obj[fieldsArray[i]] = '';
  }
  return obj;
};
