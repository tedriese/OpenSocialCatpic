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
 * @fileoverview Activity tests for MakaMaka.
 *
 * Activities Fetching - not supporting containers - ACT000
 * Activities Fetching - supporting containers - ACT600
 * Activities Fetching - Error Handling - ACT400

 * Activities Creation - Basic - ACT100
 * Activities Creation - Fields - ACT200
 * Activities Creation - Media Items - ACT300
 * Activities Creation - Error Handling - ACT500
 * Activities Creation - Fetch Create - ACT600
 */
function ActivitySuite0_8() {
  this.name = 'Activity Test Suite';
  this.id = 'ACT';
  this.tests = [


   /**
    * Activities Fetching
    */
    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(VIEWER)',
      id: 'ACT000',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a VIEWER, and ' +
                   'tests that the dataResponse is not null',
      run: function(context, callback, result) {
        ActivitySuite0_8.testNewFetchActivityRequestFor(Config.VIEWER,
            callback, result, context, Assert.assertTrue, Config.VIEWER);
      }
    },


    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(VIEWER_FRIENDS)',
      id: 'ACT001',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a ' +
                   'VIEWER_FRIENDS, and tests that the dataResponse is not ' +
                   'null',
      run: function(context, callback, result) {
        ActivitySuite0_8.testNewFetchActivityRequestFor(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            callback, result, context, Assert.assertTrue, Config.VIEWER);
      }
    },


    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(OWNER)',
      id: 'ACT002',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a OWNER, and ' +
                   'tests that the dataResponse is not null',
      run: function(context, callback, result) {
        ActivitySuite0_8.testNewFetchActivityRequestFor(Config.OWNER,
            callback, result, context, Assert.assertTrue, Config.OWNER);
      }
    },


    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(OWNER_FRIENDS)',
      id: 'ACT003',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a OWNER_FRIENDS,' +
                   ' and tests that the dataResponse is not null',
      run: function(context, callback, result) {
        ActivitySuite0_8.testNewFetchActivityRequestFor(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            callback, result, context, Assert.assertTrue, Config.OWNER);
      }
    },


    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(INVALID_ID) ' +
            'using an invalid ID',
      id: 'ACT004',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a INVALID_ID,' +
                   ' and tests that the dataResponse is not null',
      run: function(context, callback, result) {
        var invalidId = new Date().getTime();
        ActivitySuite0_8.testNewFetchActivityRequestFor(
            opensocial.newIdSpec({userId: invalidId}),
            callback, result, context, Assert.assertFalse);
      }
    },


    { name: 'opensocial.DataRequest.newFetchActivitiesRequest(ID) using '
        + "a known owner's friend",
      id: 'ACT005',
      priority: Test.PRIORITY.P2,
      description: 'Tries to fetch activities for a OWNERS_FRIENDS if' +
                   ' container supports fetching activities.',
      run: function(context, callback, result) {
        var thisTest = this;
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
            'owner_friends');
        req.send(function(dataResponse){
          result.addSubResult('Data response not null',
              Assert.assertNotNull(dataResponse), dataResponse, 'not null');
          if (Assert.assertDataResponseShouldContinue(dataResponse, result,
              context, null, Result.severity.WARNING,
              Config.OWNER)) {
            var ownerFriends = dataResponse.get('owner_friends');
            if (ownerFriends && ownerFriends.getData()) {
              var friends = ownerFriends.getData();
              Helper.logIntoResult(
                  result, 'OWNER_FRIENDS.size()', friends.size(),
                  'should have at least 1 friend for fetching activities');
              var friend = (friends && (friends.size() > 0))
                  ? friends.asArray()[0] : undefined;
              if (friend) {
                ActivitySuite0_8.testNewFetchActivityRequestFor(
                    opensocial.newIdSpec({userId : friend.getId()}),
                    callback, result, context, false);
              }
            } else {
              result.addSubResult('owner_friends has no data', Assert.fail,
                  'no data', 'Some value');
            }
          }
          callback(result);
        });
      }
    },


    /**
     * Activities Creation - Basic
     */

    { name: 'opensocial.requestCreateActivity using title',
      id: 'ACT100',
      priority: Test.PRIORITY.P1,
      description: 'Create activity using title, verify if fetching is' +
                   ' supported.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();

        var title = '[' + this.id + ']: usingTitle ' + today + ' '
            + activityStamp;

        var activity = opensocial.newActivity({'title' : title});
        opensocial.requestCreateActivity(activity,
            opensocial.CreateActivityPriority.HIGH,
          function(responseItem) {
            if (Assert.assertDataResponseShouldContinue(responseItem, result,
                context, null, Result.severity.WARNING,
                Config.OWNER)) {
              // Check activity creation
              Helper.addSubResult(result, 'dataResponse.hadError()'
                  , Assert.assertFalse, responseItem.hadError());
              ActivitySuite0_8.testFetchActivity(result, context,
                  opensocial.Activity.Field.TITLE, activityStamp,
                  function(activityFetched, result) {
                    if (activityFetched) {
                      Helper.addInfoSubResult(result, 'activityFetched',
                          Assert.assertDefined,
                          gadgets.json.stringify(activityFetched), 'Defined');
                      var actualTitle = activityFetched.getField(
                          opensocial.Activity.Field.TITLE);
                      Helper.addSubResult(result, 'Title',
                          Assert.assertEquals, actualTitle, title);
                    }
                    callback(result);
                  });
            } else {
              if (!opensocial.getEnvironment().supportsField(
                  opensocial.Environment.ObjectType.ACTIVITY, 'title')) {
                Helper.addSubResult(result, 'Doesn\'t support title - ' +
                    'responseItem.hadError()', Assert.assertTrue,
                    responseItem.hadError(), 'true');
              }
              callback(result);
            }
          });
      }
    },


    { name: 'opensocial.requestCreateActivity using titleId',
      id: 'ACT101',
      priority: Test.PRIORITY.P1,
      description: 'Tests if you can create activity using title-id without ' +
                   'title.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();

        var titleId = '[' + this.id + ']: usingTitleId '
            + today + ' ' + activityStamp;

        var activity = opensocial.newActivity({'titleId' : titleId});

        opensocial.requestCreateActivity(activity,
            opensocial.CreateActivityPriority.HIGH,
          function(responseItem) {
            if(Assert.assertDataResponseShouldContinue(responseItem, result,
                context, null, Result.severity.WARNING,
                Config.OWNER)) {
              //Check activity creation
              result.addSubResult('Response item is not null',
                  Assert.assertNotNull(responseItem), responseItem,
                  'responseItem not null');
              Helper.addSubResult(result, 'dataResponse.hadError()'
                  , Assert.assertFalse, responseItem.hadError());
              ActivitySuite0_8.testFetchActivity(result, context,
                  opensocial.Activity.Field.TITLE_ID, activityStamp,
                  function(activityFetched, result) {
                    if (activityFetched) {
                      Helper.addInfoSubResult(result, 'activityFetched',
                          Assert.assertDefined,
                          gadgets.json.stringify(activityFetched), 'Defined');
                      var actualTitleId = activityFetched.getField(
                          opensocial.Activity.Field.TITLE_ID);
                      Helper.addSubResult(result, 'Title Id',
                          Assert.assertEquals, actualTitleId, titleId);
                    }
                    callback(result);
                  });
            } else {
              if (!opensocial.getEnvironment().supportsField(
                  opensocial.Environment.ObjectType.ACTIVITY, 'titleId')) {
                Helper.addSubResult(result, 'Doesn\'t support titleId - ' +
                    'responseItem.hadError()', Assert.assertTrue,
                    responseItem.hadError(), 'true');
              }
              callback(result);
            }
          });
      }
    },


    { name: 'opensocial.requestCreateActivity using Template and Template '
        + 'Params',
      id: 'ACT102',
      priority: Test.PRIORITY.P1,
      description: 'Tests if it\'s possible to create activity using template' +
                   'and template params. It tries to verify if fetching ' +
                   'activities is supported.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();

        var owner = context.getOwner();
        var viewer = context.getViewer();
        var templateParams = {};
        templateParams['Song'] = 'With or without you';
        templateParams['Artist'] = 'U2';
        templateParams['Viewer'] = viewer;
        templateParams['Owner'] = owner;

        var title = '[' + this.id + ']: templateParams ' + today + ' ' +
            activityStamp;
        var titleId = '[' + this.id + ']: templateParams titleId ' + today
            + ' ' + activityStamp;

        var params = {};
        params[opensocial.Activity.Field.TITLE] = title;
        params[opensocial.Activity.Field.TITLE_ID] = titleId;
        params[opensocial.Activity.Field.TEMPLATE_PARAMS] = templateParams;
        var activity = opensocial.newActivity(params);

        ActivitySuite0_8.testRequestCreateActivity(result, activity,
            function(result, responseItem) {
              if(Assert.assertDataResponseShouldContinue(responseItem, result,
                  context, null, Result.severity.WARNING,
                  Config.OWNER)) {
                ActivitySuite0_8.testFetchActivity(result, context,
                    opensocial.Activity.Field.TITLE, activityStamp,
                    function(activityFetched, result) {
                      if (activityFetched) {
                        var _templateParams = activityFetched.getField(
                            opensocial.Activity.Field.TEMPLATE_PARAMS);

                        if (opensocial.getEnvironment().supportsField(
                            opensocial.Environment.ObjectType.ACTIVITY
                            , opensocial.Activity.Field.TEMPLATE_PARAMS)) {
                          Helper.addSubResult(result, 'Activity Template Object'
                              , Assert.assertObjectEquals('ActivityTemplate',
                              _templateParams, null, templateParams, result),
                              _templateParams, templateParams);
                          Helper.addSubResult(result,
                              'Activity Template Fields',
                              Assert.assertObjectEquals('ActivityTemplate',
                              _templateParams, ['Song', 'Artist', 'Viewer'
                                  , 'Owner'], templateParams, result),
                              _templateParams, templateParams);

                          Helper.addSubResult(result, 'Template Song',
                              Assert.assertEquals, _templateParams['Song'],
                              templateParams['Song']);
                          Helper.addSubResult(result, 'Template Artist',
                              Assert.assertEquals, _templateParams['Artist'],
                              _templateParams['Artist']);
                          Helper.addSubResult(result, 'Template Viewer',
                              Assert.assertEquals, _templateParams['Viewer'],
                              _templateParams['Viewer']);
                          Helper.addSubResult(result, 'Template Owner',
                              Assert.assertEquals, _templateParams['Owner'],
                              _templateParams['Owner']);

                          Helper.addSubResult(result, 'Activity Title',
                              Assert.assertEquals, actualTitle
                              , params[opensocial.Activity.Field.TITLE]);
                        } else {
                          Helper.logIntoResult(result,
                              'templateParams is NOT supported');
                        }

                        if (opensocial.getEnvironment().supportsField(
                        opensocial.Environment.ObjectType.ACTIVITY, 'title')) {
                          var actualTitle = activityFetched.getField(
                              opensocial.Activity.Field.TITLE);
                          Helper.addSubResult(result, 'Activity Title',
                              Assert.assertEquals, actualTitle
                              , params[opensocial.Activity.Field.TITLE]);
                        }
                      }
                      callback(result);
                    });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
      }
    },

    { name: 'opensocial.requestCreateActivity using Template',
      id: 'ACT103',
      tags: ['notImplementedYet'],
      priority: Test.PRIORITY.P1,
      description: 'Tests if it\'s possible to create activity using template' +
                   '. It tries to verify if fetching activities is supported.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();

        var owner = context.getOwner();
        var viewer = context.getViewer();

        var title = '[' + this.id + ']: Using Template ' + today + ' ' +
            activityStamp;
        var titleId = '[' + this.id + ']: titleId ' + today + ' ' +
            activityStamp;

        var params = {};
        params[opensocial.Activity.Field.TITLE] = title;
        params[opensocial.Activity.Field.TITLE_ID] = titleId;
        var activity = opensocial.newActivity(params);

        activity.setField('LISTEN_TO_THIS_SONG',
            {Song: 'Do That There - (Young Einstein hoo-hoo mix)',
             Artist: 'Lyrics Born', Subject: viewer, Owner: owner});

        ActivitySuite0_8.testRequestCreateActivity(result, activity,
            function(result, responseItem){
              if(Assert.assertDataResponseShouldContinue(responseItem, result,
                  context, null, Result.severity.WARNING,
                  Config.OWNER)) {
                ActivitySuite0_8.testFetchActivity(result, context,
                    opensocial.Activity.Field.TITLE, activityStamp,
                    function(activityFetched, result){
                      if (activityFetched) {
                        if (opensocial.getEnvironment().supportsField(
                            opensocial.Environment.ObjectType.ACTIVITY
                            , opensocial.Activity.Field.TEMPLATE_PARAMS)) {

                          var _templateParams = activityFetched.getField(
                              opensocial.Activity.Field.TEMPLATE_PARAMS);
                          Helper.addSubResult(result, 'Template Song',
                              Assert.assertEquals, _templateParams['Song'],
                              'Do That There - (Young Einstein hoo-hoo mix)');
                          Helper.addSubResult(result, 'Template Artist',
                              Assert.assertEquals, _templateParams['Artist'],
                              'Lyrics Born');
                          Helper.addSubResult(result, 'Template Viewer',
                              Assert.assertEquals, _templateParams['Viewer'],
                              viewer);
                          Helper.addSubResult(result, 'Template Owner',
                              Assert.assertEquals, _templateParams['Owner'],
                              owner);
                        } else {
                          Helper.logIntoResult(result,
                              'templateParams is NOT supported');
                        }

                       if (opensocial.getEnvironment().supportsField(
                        opensocial.Environment.ObjectType.ACTIVITY, 'title')) {
                        var actualTitle = activityFetched.getField(
                            opensocial.Activity.Field.TITLE);
                        Helper.addSubResult(result, 'Activity Title',
                            Assert.assertEquals, actualTitle
                            , params[opensocial.Activity.Field.TITLE]);
                       }

                      }
                      callback(result);
                    });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
      }
    },


    /**
     * Activities Creation - Fields
     */

    { name: 'opensocial.Activity newActivity(params) with list of Activity '
        + 'attributes in the params map. Only checks those that are supported.',
      id : 'ACT200',
      bugs : ['1040595'],
      priority: Test.PRIORITY.P1,
      description: 'Tests if it\'s possible to create activity with list of ' +
                   'Activity attributes in the params map. Only checks those ' +
                   'that are supported.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();
        var owner = context.getOwner();
        var viewer = context.getViewer();
        var templateParams = {};
        templateParams['Song'] = 'With or without you';
        templateParams['Artist'] = 'With or without you';
        templateParams['Viewer'] = viewer;
        templateParams['Owner'] = owner;

        var params = {
          'titleId': '[' + this.id + ']: allFields titleId <i>' + today
              + '</i> ' + activityStamp,
          'title': '[' + this.id + ']: allFields <b>' + today + '</b> '
              + activityStamp,
          'templateParams': templateParams,
          'url': 'url_' + activityStamp,
          'mediaItems': ActivitySuite0_8.sampleMediaItems(),
          'bodyId': 'bodyId_' + activityStamp,
          'body': 'This <i>body</i> is <b>' + activityStamp + '</b>',
          'externalId': 'externalId_' + activityStamp,
          'streamTitle': 'streamTitle_' + activityStamp,
          'streamUrl': 'streamUrl_' + activityStamp,
          'streamSourceUrl': 'streamSourceUrl_' + activityStamp,
          'streamFaviconUrl': 'streamFaviconUrl_' + activityStamp,
          'priority': '1',
          'id': 'id_' + activityStamp,
          'userId': 'userId_' + activityStamp,
          'appId': 'appId_' + activityStamp,
          'postedTime': activityStamp
        };

        var activity = opensocial.newActivity(params);
        result.addSubResult('Activity is not null',
            Assert.assertNotNull, activity, 'Not null.', Result.severity.INFO);

        //Creates the Activity on the server
        ActivitySuite0_8.testRequestCreateActivity(result, activity,
            function(result, responseItem) {
                if(Assert.assertDataResponseShouldContinue(responseItem, result,
                    context, null, Result.severity.WARNING,
                    Config.OWNER)) {
                 ActivitySuite0_8.testFetchActivity(result, context
                     , opensocial.Activity.Field.TITLE, activityStamp,
                     function(activityFetched, result) {
                       if (activityFetched) {
                         for (var fieldName in params) {
                           var expected = params[fieldName];
                           var actual = (typeof(activity.getField(
                              fieldName)) == 'string')
                              ? gadgets.util.unescapeString(
                                  activity.getField(fieldName))
                              : activity.getField(fieldName);
                           if (opensocial.getEnvironment().supportsField(
                               opensocial.Environment.ObjectType.ACTIVITY,
                               fieldName)) {
                             Helper.addSubResult(result, 'activity.getField('
                                 + fieldName + ')', Assert.assertEquals,
                                 actual, expected);
                           } else {
                               Helper.addSubResult(result, 'Unsupported Field['
                                   + fieldName + ']', Assert.assertEquals,
                                   actual, expected);
                           }
                         }
                       }
                       callback(result);
                    });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
      }
    },


    { name: 'Create activity and set fields: STREAM_TITLE, STREAM_URL, '
          + 'STREAM_SOURCE_URL, STREAM_FAVICON_URL',
      id: 'ACT202',
      priority: Test.PRIORITY.P1,
      description: 'Tests if it\'s possible to create activity with stream ' +
                   'params. STREAM_TITLE, STREAM_URL, STREAM_SOURCE_URL, ' +
                   'STREAM_FAVICON_URL',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime();

        var title = '[' + this.id + ']: StreamFields ' + today + ' '
            + activityStamp;
        var titleId = '[' + this.id + ']: StreamFields titleId ' + today + ' '
            + activityStamp;

        var params = {};
        params[opensocial.Activity.Field.TITLE] = title;
        params[opensocial.Activity.Field.TITLE_ID] = titleId;
        var activity = opensocial.newActivity(params);

        var streamTitle = 'Testing Stream Fields: STREAM_TITLE';
        activity.setField(opensocial.Activity.Field.STREAM_TITLE, streamTitle);
        var streamUrl = 'Testing Stream Fields: STREAM_URL';
        activity.setField(opensocial.Activity.Field.STREAM_URL, streamUrl);
        var streamSourceUrl = 'Testing Stream Fields: STREAM_SOURCE_URL';
        activity.setField(opensocial.Activity.Field.STREAM_SOURCE_URL,
            streamSourceUrl);
        var streamFaviconUrl = 'Testing Stream Fields: STREAM_FAVICON_URL';
        activity.setField(opensocial.Activity.Field.STREAM_FAVICON_URL,
            streamFaviconUrl);

        ActivitySuite0_8.testRequestCreateActivity(result, activity,
          function(result, responseItem) {
              if(Assert.assertDataResponseShouldContinue(responseItem, result,
                  context, null, Result.severity.WARNING,
                  Config.OWNER)) {
                ActivitySuite0_8.testFetchActivity(result, context
                , opensocial.Activity.Field.TITLE, activityStamp,
                function(activityFetched, result){
                  if (activityFetched) {
                    var actualTitle = activityFetched.getField(
                       opensocial.Activity.Field.TITLE);
                    Helper.addSubResult(result, 'Activity Title',
                       Assert.assertEquals, actualTitle, title);

                    if (opensocial.getEnvironment().supportsField(
                         opensocial.Environment.ObjectType.ACTIVITY,
                         opensocial.Activity.Field.STREAM_TITLE)) {
                      var _streamTitle = activityFetched.getField(
                         opensocial.Activity.Field.STREAM_TITLE);
                      var _streamUrl = activityFetched.getField(
                         opensocial.Activity.Field.STREAM_URL);
                      var _streamSourceUrl = activityFetched.getField(
                         opensocial.Activity.Field.STREAM_SOURCE_URL);
                      var _streamFaviconUrl = activityFetched.getField(
                         opensocial.Activity.Field.STREAM_FAVICON_URL);

                      Helper.addSubResult(result, 'Stream Title',
                          Assert.assertEquals, _streamTitle, streamTitle,
                          Result.severity.WARNING);
                      Helper.addSubResult(result, 'Stream Url',
                          Assert.assertEquals, _streamUrl, streamUrl,
                          Result.severity.WARNING);
                      Helper.addSubResult(result, 'Stream Source Url',
                          Assert.assertEquals, _streamSourceUrl,
                          streamSourceUrl, Result.severity.WARNING);
                      Helper.addSubResult(result, 'Stream Favicon Url',
                          Assert.assertEquals, _streamFaviconUrl,
                          streamFaviconUrl, Result.severity.WARNING);
                    }
                  }
                  callback(result);
                });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
     }
    },

    { name: 'Create activity and try to set read-only fields: '
        + 'ID, USER_ID, APP_ID, POSTED_TIME',
      id: 'ACT203',
      priority: Test.PRIORITY.P1,
      description: 'Test that creating activity with read-only fields ' +
                   'has no side effects. Container should ignore read only ' +
                   'fields.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime() + '';
        var title = '[' + this.id + ']: ReadOnlyFields ' + today
            + ' ' + activityStamp;
        var titleId = '[' + this.id + ']: ReadOnlyFields titleId '
            + today + ' ' + activityStamp;

        var params = {};
        params[opensocial.Activity.Field.TITLE] = title;
        params[opensocial.Activity.Field.TITLE_ID] = titleId;
        var activity = opensocial.newActivity(params);

        var id = 'some id';
        activity.setField(opensocial.Activity.Field.ID, id);
        var userId = 'some user_id';
        activity.setField(opensocial.Activity.Field.USER_ID, userId);
        var appId = 'some app_id';
        activity.setField(opensocial.Activity.Field.APP_ID, appId);
        var postedTime = activityStamp;
        activity.setField(opensocial.Activity.Field.POSTED_TIME, activityStamp);

        ActivitySuite0_8.testRequestCreateActivity(result, activity,
            function(result, responseItem) {
              if(Assert.assertDataResponseShouldContinue(responseItem, result,
                  context, null, Result.severity.WARNING,
                  Config.OWNER)) {
                 ActivitySuite0_8.testFetchActivity(result, context
                     , opensocial.Activity.Field.TITLE, activityStamp,
                     function(activityFetched, result) {
                       if (activityFetched) {
                         var actualTitle = activityFetched.getField(
                             opensocial.Activity.Field.TITLE);
                         var _id = activityFetched.getField(
                             opensocial.Activity.Field.ID);
                         var _userId = activityFetched.getField(
                             opensocial.Activity.Field.USER_ID);
                         var _appId = activityFetched.getField(
                             opensocial.Activity.Field.APP_ID);
                         var _postedTime = activityFetched.getField(
                             opensocial.Activity.Field.POSTED_TIME);

                         Helper.addSubResult(result, 'Activity Title',
                             Assert.assertEquals, actualTitle, title);
                         Helper.addSubResult(result, 'Activity Id',
                             Assert.assertNotEquals, _id, id);
                         Helper.addSubResult(result, 'Activity User Id',
                             Assert.assertNotEquals, _userId, userId);
                         Helper.addSubResult(result, 'Activity App Id',
                             Assert.assertNotEquals, _appId, appId);
                         Helper.addSubResult(result, 'Activity Posted Time',
                             Assert.assertNotEquals, _postedTime, postedTime);
                       }
                       callback(result);
                     });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
     }
    },


    /**
     * Activities Creation - Media Items
     */

    { name: 'opensocial.requestCreateActivity with media item',
      id: 'ACT300',
      bugs: ['1040595', '1040601'],
      priority: Test.PRIORITY.P1,
      description: 'Test that it\'s possible to creating activity with ' +
                   'media items. It shouldn\'t fail.',
      run: function(context, callback, result) {
        var today = new Date();
        var activityStamp = today.getTime() + '';
        var title = '[' + this.id + ']: ActivityWithMediaItems ' + today
            + ' ' + activityStamp;
        var titleId = '[' + this.id + ']: ActivityWithMediaItems titleId '
            + today + ' ' + activityStamp;

        var params = {};
        params[opensocial.Activity.Field.TITLE] = title;
        params[opensocial.Activity.Field.TITLE_ID] = titleId;
        var activity = opensocial.newActivity(params);

        activity.setField(opensocial.Activity.Field.MEDIA_ITEMS,
            ActivitySuite0_8.sampleMediaItems())

        ActivitySuite0_8.testRequestCreateActivity(result, activity,
            function(result, responseItem) {
              if(Assert.assertDataResponseShouldContinue(responseItem, result,
                  context, null, Result.severity.WARNING,
                  Config.OWNER)) {
                ActivitySuite0_8.testFetchActivity(result, context
                , opensocial.Activity.Field.TITLE, activityStamp,
                function(activityFetched, result) {
                  if (activityFetched) {
                    var mediaItems = activityFetched.getField(
                       opensocial.Activity.Field.MEDIA_ITEMS);

                    Helper.addSubResult(result, 'MediaItems is an Array',
                        Assert.assertDefined, mediaItems,
                        'Array of Media Items Defined');
                    Helper.addSubResult(result, 'MediaItems with 3 elements',
                        Assert.assertEquals, mediaItems.length,
                        ActivitySuite0_8.sampleMediaItems().length);

                    for (var mediaItem in mediaItems) {
                      var type = mediaItems[mediaItem].getField(
                         opensocial.MediaItem.Field.TYPE);
                      var url = mediaItems[mediaItem].getField(
                         opensocial.MediaItem.Field.URL);
                      var mimeType = mediaItems[mediaItem].getField(
                         opensocial.MediaItem.Field.MIME_TYPE);

                      switch (type) {
                        case opensocial.MediaItem.Type.AUDIO:
                          Helper.addSubResult(result, 'Media Item Audio URL'
                              , Assert.assertEquals, url, Config.rootPath
                              + 'suites/0.7/activities/content/beep.wav' );
                          Helper.addSubResult(result, 'Mime Type Audio',
                             Assert.assertEquals, mimeType, 'audio/x-wav');
                          break;
                        case opensocial.MediaItem.Type.IMAGE:
                          Helper.addSubResult(result, 'Media Item Image URL'
                              , Assert.assertEquals, url, Config.rootPath
                              + 'suites/0.7/activities/content/car.jpeg');
                          Helper.addSubResult(result, 'Mime Type Image',
                              Assert.assertEquals, mimeType, 'image/jpeg');
                          break;
                        case opensocial.MediaItem.Type.VIDEO:
                          Helper.addSubResult(result, 'Media Item Video URL'
                              , Assert.assertEquals, url, Config.rootPath
                              + 'suites/0.7/activities/content/eddie.asf');
                          Helper.addSubResult(result, 'Mime Type Video',
                              Assert.assertEquals, mimeType, 'video/x-ms-asf');
                          break;
                        default:
                          Helper.addSubResult(result, 'Media not identified',
                              Assert.assertFalse, type, 'Known Type');
                          break;
                      }
                    }
                  }
                  callback(result);
                });
              } else {
                Assert.assertDataResponseHadError(
                    responseItem, result, null, true, Result.severity.INFO);
                callback(result);
              }
            }, this, context);
      }
    },


    /**
     * Activities Creation - Error Handling
     */
    { name: 'Create activity without setting the title',
      id: 'ACT500',
      priority: Test.PRIORITY.P2,
      description: 'Test if trying to create an activity without title ' +
                   'throws an error.',
      run: function(context, callback, result) {
        var today = new Date();
        var activity = opensocial.newActivity({'id' :  today.getTime()});
        opensocial.requestCreateActivity(activity,
            opensocial.CreateActivityPriority.HIGH,
            function(responseItem) {
          Assert.assertDataResponseItemHadError('responseItem',
                responseItem, result, true
                , opensocial.ResponseItem.Error.BAD_REQUEST
                , 'You must pass in an activity with a title or title id.');
          callback(result);
        });
     }
    },


    { name: 'opensocial.requestCreateActivity using Invalid Param Keys',
      id: 'ACT501',
      priority: Test.PRIORITY.P2,
      description: 'Test if trying to create an activity with invalid param' +
                   'keys throws an error.',
      run: function(context, callback, result) {
        var today = new Date();
        var owner = context.getOwner();
        var viewer = context.getViewer();
        var activity = opensocial.newActivity('LISTEN_TO_THIS_SONG',
            {Song: 'With or without you', Artist: 'U2',
                Subject: viewer, Owner: owner});
         activity.setField(opensocial.Activity.Field.TITLE,
            '[' + this.id + ']: Create Activity Using Template '
            + today + ' ' + today.getTime());
        opensocial.requestCreateActivity(activity,
            opensocial.CreateActivityPriority.HIGH,
          function(responseItem) {
            Assert.assertDataResponseItemHadError('createActivity',
                responseItem, result, true,
                opensocial.ResponseItem.Error.BAD_REQUEST);
            callback(result);
          });
      }
    },


    { name: 'Create activity - empty title',
      id: 'ACT502',
      priority: Test.PRIORITY.P2,
      description: 'Test if trying to create an activity with empty title' +
                   ' throws an error.',
      run: function(context, callback, result) {
        var activity = opensocial.newActivity({'title' : ''});
        opensocial.requestCreateActivity(activity,
            opensocial.CreateActivityPriority.HIGH,
            function(responseItem) {
              Assert.assertDataResponseItemHadError('responseItem',
                responseItem, result, true,
                opensocial.ResponseItem.Error.BAD_REQUEST,
                'You must pass in an activity with a title or title id.');
          callback(result);
        });
     }
    }
  ]
};

/**
 * Tries to create an activity.
 *
 * @param activity The activity to create.
 * @param callback The callback function, usually to write the test results.
 * @param test The test reference to create the result.
 */
ActivitySuite0_8.testRequestCreateActivity = function(result, activity, callback,
    test, context) {
  opensocial.requestCreateActivity(activity,
      opensocial.CreateActivityPriority.HIGH,
      function(responseItem) {
        Helper.addSubResult(result, 'responseItem defined',
            Assert.assertTrue, responseItem != undefined, 'defined');
        if (responseItem) {
          if (context.getOwner().isViewer()) {
            Helper.addUnsevereSubResult(result, 'responseItem.hadError()',
                Assert.assertFalse, responseItem.hadError(), 'false');
            Helper.addUnsevereSubResult(result, 'responseItem.getErrorCode()',
                Assert.assertNull, responseItem.getErrorCode(), 'null');
          } else {
            Helper.addUnsevereSubResult(result, 'responseItem.hadError()',
                Assert.assertTrue, responseItem.hadError(), 'true');
          }
        }
        callback(result, responseItem);
      });
}

ActivitySuite0_8.testFetchActivity = function(result, context,
    criteria, activityStamp, callback) {
  criteria = criteria || opensocial.Activity.Field.TITLE;
  var activity = null;
  var req = opensocial.newDataRequest();
  req.add(req.newFetchActivitiesRequest(Config.OWNER), 'activities');

  req.send(function(dataResponse) {
    if(Assert.assertDataResponseShouldContinue(dataResponse, result,
        context, Result.severity.WARNING)) {
      // bug fix for 0.8
      var activities = dataResponse.get('activities').getData().asArray();
      Helper.addUnsevereSubResult(result, 'At least one activity to '
          + 'iterate', Assert.assertTrue, activities.length > 0,
          activities.length);

      var index = 0;
      var found = false;
      while (!found && index < activities.length) {

        var activityStep = activities[index];
        var fieldValue = activityStep.getField(criteria);
        if (Assert.assertStringContains(fieldValue, activityStamp)) {
          found = true;
          activity = activityStep;
        }
        index ++;
      }
      Helper.addUnsevereSubResult(result, 'Activity Found',
          Assert.assertDefined, activity, 'Activity fetched');
      callback(activity, result);
    } else {
      Helper.addUnsevereSubResult(result, 'Activity Fetching : ' +
          'dataResponse.hadError() :', Assert.assertTrue,
          dataResponse.hadError(), 'false', Result.severity.INFO);
      callback(null, result);
    }
  });
}


/**
 * Tries to fetch activities for a given idSpec, and tests that the dataResponse
 * is not null and gets an error, because for now reading activities is not
 * supported.  
 *
 * @param idSpec The id specification, like 'VIEWER'.
 * @param callback The callback function, usually to write the test results.
 * @param test The test reference to create the result.
 * @param opt_viewerOrOwner whether to succeed if viewerOrOwner is present 
 */
ActivitySuite0_8.testNewFetchActivityRequestFor = function(idSpec, callback,
    result, context, method, opt_viewerOrOwner){
  var req = opensocial.newDataRequest();
  req.add(req.newFetchActivitiesRequest(idSpec), 'activities');
  Helper.logIntoResult(result, 'fetchActivity for idSpec', idSpec);
  req.send(function(dataResponse) {
    if(Assert.assertDataResponseShouldContinue(dataResponse, result,
        context, null, Result.severity.WARNING, opt_viewerOrOwner)) {
      // bug fix for 0.8
      var activities = dataResponse.get('activities').getData().asArray();
      Helper.addSubResult(result, 'Activities Array',
          Assert.assertDefined, activities, 'activities defined');
      Helper.logIntoResult(result, 'activities.length', activities.length);
      Helper.addSubResult(result, 'activities.length > 0', method,
          (activities.length > 0), 'activities.length > 0',
          Result.severity.WARNING);
      if (activities.length > 0) {
        for (var i = 0 ; i < activities.length; i++){
          var id = activities[i].getField(opensocial.Activity.Field.ID);
          Helper.addSubResult(result,
              'Activity Id: ', Assert.assertDefined, id, 'Id is defned');
        }
      }
    } else {
      Helper.addUnsevereSubResult(result, 'Activity Fetching Failed: ' +
          'dataResponse.hadError', Assert.fail, dataResponse.hadError(),
          false);
    }
    ActivitySuite0_8.callbackExecuted = true;
    callback(result);
  });
}

ActivitySuite0_8.sampleMediaItems = function() {
  var imageUrl = Config.rootPath + 'suites/0.7/activities/content/car.jpeg';
  var imageParams = {};
  imageParams[opensocial.MediaItem.Field.MIME_TYPE] = 'image/jpeg';
  imageParams[opensocial.MediaItem.Field.TYPE] =
      opensocial.MediaItem.Type.IMAGE;
  imageParams[opensocial.MediaItem.Field.URL] =  imageUrl;
  var imageItem = opensocial.newMediaItem('image/jpeg', imageUrl, imageParams);

  var audioUrl = Config.rootPath + 'suites/0.7/activities/content/beep.wav';
  var audioParams = {};
  audioParams[opensocial.MediaItem.Field.MIME_TYPE] = 'audio/x-wav';
  audioParams[opensocial.MediaItem.Field.TYPE] =
      opensocial.MediaItem.Type.AUDIO;
  audioParams[opensocial.MediaItem.Field.URL] =  audioUrl;
  var audioItem = opensocial.newMediaItem('audio/x-wav', audioUrl, audioParams);

  var videoUrl = Config.rootPath + 'suites/0.7/activities/content/eddie.asf';
  var videoParams = {};
  videoParams[opensocial.MediaItem.Field.MIME_TYPE] = 'video/x-ms-asf';
  videoParams[opensocial.MediaItem.Field.TYPE] =
      opensocial.MediaItem.Type.VIDEO;
  videoParams[opensocial.MediaItem.Field.URL] =  videoUrl;
  var videoItem = opensocial.newMediaItem(
      'video/x-ms-asf', videoUrl, videoParams);
  return [imageItem, audioItem, videoItem];
}
