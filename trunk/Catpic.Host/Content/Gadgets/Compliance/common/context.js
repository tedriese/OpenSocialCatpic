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
 * @fileoverview Contains the logic for calculating the data used for verifying
 * the results based on the current context of the run environment. (e.g.
 * the viewer, the owner, the container).  More verifications will be done
 * for several known users.  For unknown users, there will be limited or no
 * verification.
 */

function Context(viewer, owner, domain) {

  // Emulate OpenSocial isViewer/isOwner interface.
  this.viewer = viewer ? viewer :
      { isOwner: function() {return false;},
        isViewer: function() {return true;}
      };
  this.owner = owner ? owner :
      { isOwner: function() {return true;},
        isViewer: function() {return false;}
      };

  // Mode is: is testuser, is testfriend, is anonymous.
  this.__isOwner = owner && viewer &&
      (owner.getDisplayName() == viewer.getDisplayName());
  this.__isUnitTestUser = false; // Tom
  this.__isDevUnitTestUser = false; // Frank
  this.__isTestUserFriend = false; // Sam or Sandy
  this.__isUnknownUser = false; // Unknown viewer

  // Create the corresponding data instance, based on the domain.
  var data;
  var dataConstructor = Config.domainToDataContructor[domain];
  if (dataConstructor) {
    data = new dataConstructor();
  } else {
    data = (typeof TestData.CustomData == 'function')
        ? new TestData.CustomData()
        : new TestData.DefaultData(domain);
  }
  this.__data = data;

  this.viewerName = viewer ? viewer.getDisplayName() : 'viewerWithoutApp';
  this.ownerName = owner ? owner.getDisplayName() : 'anonymousOwner';

  // Set the expected viewer and owner for verification.
  for (var key in data.users) {
    if (this.viewerName == data.users[key]['getDisplayName']) {
      this.__expectedViewer = data.users[key];
      this.__expectedViewer.isViewer = true;
      this.__expectedViewer.isOwner =
          this.__expectedViewer['getDisplayName'] == this.ownerName;
    }
  }

  if (!this.__expectedViewer) {
    this.__expectedViewer =
       data && data.users['testUserData'] ? data.users['testUserData'] : {};
  }

  for (var key in data.users) {
    if (this.ownerName == data.users[key]['getDisplayName']) {
      this.__expectedOwner = data.users[key];
      this.__expectedOwner.isOwner = true;
      this.__expectedOwner.isViewer =
          this.__expectedOwner['getDisplayName'] == this.viewerName;
    }
  }

  if (!this.__expectedOwner) {
    this.__expectedOwner =
        data && data.users['testUserData'] ? data.users['testUserData'] : {};
  }
  // Set the isOwner flag for owner's and viewer's friends.
  var ownerFriends = this.__expectedOwner["__friends__"] || [];
  for (var i = 0; i < ownerFriends.length; i++) {
    if (ownerFriends[i]['getDisplayName'] == this.ownerName) {
      ownerFriends[i]['isOwner'] = true;
    }
  }
  var viewerFriends = this.__expectedViewer["__friends__"] || [];
  for (var i = 0; i < viewerFriends.length; i++) {
    if (viewerFriends[i]['getDisplayName'] == this.ownerName) {
      viewerFriends[i]['isOwner'] = true;
    }
  }
};

// actual viewer
Context.prototype.getViewer = function() {
  return this.viewer;
};

// actual owner
Context.prototype.getOwner = function() {
  return this.owner;
};

Context.prototype.isNotAvailable = function(viewerOrOwner) {
  if (viewerOrOwner == Config.VIEWER) {
    return this.viewer ? false : true;
  } else if (viewerOrOwner == Config.OWNER) {
    return this.owner ? false : true;
  } else {
    return this.isViewerWithoutApp();
  }
};

/*
 * Returns a merge of data.users and data.fields to keep the interface 
 * that the current tests are assuming.
 */  
Context.prototype.getData = function() {
  var mergedData = {};
  for (var user in this.__data.users) {
    mergedData[user] = this.__data.users[user];
  }
  for (var field in this.__data.fields) {
    mergedData[field] = this.__data.fields[field];
  }
  return mergedData;
};

Context.prototype.getIsOwner = function() {
  return this.__isOwner;
};

Context.prototype.getExpectedViewer = function() {
  return this.__expectedViewer;
};

Context.prototype.getExpectedViewerFriends = function() {
  var friends = this.__expectedViewer ? this.__expectedViewer['__friends__'] :
    undefined;
  return friends;
};

Context.prototype.getExpectedOwner = function() {
  return this.__expectedOwner;
};

Context.prototype.getExpectedOwnerFriends = function() {
  var friends = this.__expectedOwner ? this.__expectedOwner['__friends__'] :
    undefined;
  return friends;
};

Context.prototype.getEnvironment = function() {
  var envData = this.__data.environment;
  if (document && document.referrer) {
    if (document.referrer.indexOf('Profile') > 0) {
      envData.view = envData.profileView;
    } else if (document.referrer.indexOf('Application') > 0) {
      envData.view = envData.canvasView;
    } else {
      envData.view = envData.defaultView;
    }
  }
  return envData;
}

Context.prototype.isViewerWithoutApp = function() {
  // undefined or viewName matches
  return (!this.viewerName) || (this.viewerName == 'viewerWithoutApp');
}

Context.prototype.getInfo = function() {
  var info = "Running tests as: Viewer='" + this.viewerName;
  info += "' , Owner='" + this.ownerName + "'";

  // Add included and excluded tags info.
  if (Config.include != undefined && Config.include.length > 0) {
    info += '<br>Tags Included: ' + Config.include.toString();
  } else {
    info += '<br>Tags Included: all';
  }
  if (Config.exclude != undefined && Config.exclude.length > 0) {
    info += '<br>Tags Excluded: ' + Config.exclude.toString();
  } else {
    info += '<br>Tags Excluded: none';
  }
  return info;
};

Context.prototype.getContainer = function() {
  return this.__data.container;
}
