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
 * @fileoverview Appdata, viewer/owner/viewer friends/owner friends related
 * data requests tests. <br>
 * These tests depends on framework
 * http://opensocial-resources.googlecode.com/svn/tests/trunk/common/<br>
 *
 * 1) person requests: PPL000
 * - 1.1 newFetchPersonRequest VIEWER PPL000
 * - 1.2 newFetchPersonRequest OWNER PPL100
 * - 1.3 newFetchPersonRequest otherID PPL200
 * - 1.4 newFetchPersonRequest error handling PPL300
 * - newFetchPersonRequest misc PPL400
 * 2) people requests: PPL500 and up
 * - 2.1 newFetchPeopleRequest VIEWER_FRIENDS PPL500
 * - 2.2 newFetchPeopleRequest OWNER_FRIENDS PPL600
 * - 2.3 newFetchPeopleRequest other IDS PPL700
 * - 2.4 newFetchPeopleRequest error handling PPL800
 * - 2.5 newFetchPeopleRequest MISC PPL900
 *
 */
function PeopleSuite0_8() {
  this.name = 'People/Person Requests Test Suite';
  this.id = 'PPL';
  this.tests = [
    { name: 'newFetchPersonRequest - VIEWER/OWNER (no parameters)',
      id: 'PPLX00',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve profile of VIEWER/OWNER.',
      run: function(context, callback, result) {
        var ids = [ 'VIEWER', 'OWNER' ];
        PeopleSuite0_8.counter['PPL000'] = ids.length;
        for (var i = 0; i < ids.length; i++) {
          var req = opensocial.newDataRequest();
          req.add(req.newFetchPersonRequest(ids[i]), 'viewer');
          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var viewerData = dataResponse.get('viewer').getData();
                for (var field in opensocial.Person.Field) {
                  try {
                    var fieldValue =
                        viewerData.getField(opensocial.Person.Field[field]);
                     if(fieldValue != null) {
                        Helper.addSubResult(result, 'Field: ' + field,
                            Assert.assertNotNull, fieldValue, 'Some Value');
                      } else if (opensocial.getEnvironment().supportsField(
                         opensocial.Environment.ObjectType.PERSON,
                         opensocial.Person.Field[field])) {
                        Helper.addSubResult(result,
                            'Empty field - ' + field,
                            Assert.assertDataEmpty, fieldValue, 'null');
                      } else {
                       Helper.addSubResult(result,
                           'Container non-supported field - ' + field,
                           Assert.assertDataEmpty, fieldValue, 'null');
                     }
                  } catch (ex) {
                    Helper.addSubResult(result, 'Field:' + field,
                        Assert.assertEquals, ex,
                        'No Exception');
                  }
                }
              }
            }
            PeopleSuite0_8.counter['PPL000']--;
            if (PeopleSuite0_8.counter['PPL000'] == 0) {
              callback(result);
            }
          });
        }
      }
    },

    { name: 'newFetchPersonRequest - VIEWER/OWNER (all person fields)',
      id : 'PPLX01',
      priority: Test.PRIORITY.P0,
      description: 'Test if view all parameters of ' +
                   'opensocial.Person.Field for VIEWER/OWNER profile.',
      run: function(context, callback, result) {
        var ids = [ 'VIEWER', 'OWNER' ];
        PeopleSuite0_8.counter['PPL001'] = ids.length;
        for (var i = 0; i < ids.length; i++) {
          var params = {};
          params[ opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS ] =
              context.getData().personFields;
          var req = opensocial.newDataRequest();
          req.add(req.newFetchPersonRequest(ids[i], params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result, null,
                  false);
              if (!dataResponse.hadError()) {
                var viewerData = dataResponse.get('viewer').getData();
                for (var field in opensocial.Person.Field) {
                  try {
                    var fieldValue =
                        viewerData.getField(opensocial.Person.Field[field]);
                     if(fieldValue != null) {
                        Helper.addSubResult(result, 'Field: ' + field,
                            Assert.assertNotNull, fieldValue, 'Some Value');
                      } else if (opensocial.getEnvironment().supportsField(
                         opensocial.Environment.ObjectType.PERSON,
                         opensocial.Person.Field[field])) {
                        Helper.addSubResult(result,
                            'Empty field - ' + field,
                            Assert.assertDataEmpty, fieldValue, 'null');
                      } else {
                       Helper.addSubResult(result,
                           'Container non-supported field - ' + field,
                           Assert.assertDataEmpty, fieldValue, 'null');
                     }
                  } catch (ex) {
                    Helper.addSubResult(result, 'Field:' + field,
                        Assert.assertEquals, ex,
                        'No Exception');
                  }
                }
              }
            }
            PeopleSuite0_8.counter['PPL001']--;
            if (PeopleSuite0_8.counter['PPL001'] == 0) {
              callback(result);
            }
          });
        }
      }
    },

    { name: 'opensocial.hasPermission(VIEWER)',
      id: 'PPLX02',
      bugs: ['1046794'],
      priority: Test.PRIORITY.P0,
      description: 'Test if gadget has permission as VIEWER',
      run: function(context, callback, result) {
        var hasViewerPermission;
        try {
          hasViewerPermission = opensocial.hasPermission(
              opensocial.Permission.VIEWER);
          result.addSubResult('opensocial.hasPermission', Assert.assertNotNull,
              hasViewerPermission, 'Some value');
        } catch (ex) {
          var stacktrace = ex.stack || '';
          var exception =  'Exception: ' + ex.name +
              ' - File: ' + ex.fileName +
              ' - Line:(' + ex.lineNumber + ') - Msg:' +
              ex.message + '\nStacktrace: ' + stacktrace;
          result.addSubResult('opensocial.hasPermission',
              Assert.fail, exception, 'No Exception');
        }
        callback(result);
      }
    },

    { name: 'opensocial.requestPermission(opensocial.Permission.VIEWER, reason,'
      + 'callback)',
      id : 'PPLX03',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can call requestPermission for VIEWER for ' +
                   'test reason',
      run: function(context, callback, result) {
        opensocial.requestPermission(opensocial.Permission.VIEWER, 'test',
            function(dataResponse) {
              Helper.addSubResult(result,
                  'dataResponse instanceof opensocial.ResponseItem',
                  Assert.assertTrue,
                  dataResponse instanceof opensocial.ResponseItem, true);
              callback(result);
        });
      }
    },

    { name: 'opensocial.requestShareApp(' +
            'VIEWER/VIEWER_FRIENDS/OWNER/OWNER_FRIENDS, reason, callback)',
      id : 'PPLX04',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can call requestPermission for VIEWER for ' +
                   'test reason',
      run: function(context, callback, result) {
        var ids = [ 'VIEWER', 'OWNER', 'VIEWER_FRIENDS', 'OWNER_FRIENDS' ];
        for (var i = 0; i < ids.length; i++) {
          opensocial.requestShareApp(ids[i], 'test', function(dataResponse) {
            Helper.addSubResult(result,
                  'dataResponse instanceof opensocial.ResponseItem',
                  Assert.assertTrue,
                  dataResponse instanceof opensocial.ResponseItem, true);
          });
        }
        callback(result);
      }
    },


    /************************************
    * 1.1 newFetchPersonRequest - VIEWER
    ************************************/

    // PeopleRequestFields - profileDetail
    { name: 'newFetchPersonRequest - VIEWER (profile_details: addresses)',
      id: 'PPL003',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view address details of VIEWER profile.',
      bugs: ['1027335'],
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.ADDRESSES)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.ADDRESSES];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewer').getData().
                    getField(opensocial.Person.Field.ADDRESSES);
                var expectedViewer = context.getExpectedViewer();
                var expected = expectedViewer[opensocial.Person.Field.ADDRESSES]
                    ? expectedViewer[opensocial.Person.Field.ADDRESSES]
                    : undefined;

                Assert.assertSupportedObjectFields(result, actual, expected,
                    context.getData().addressFields,
                    opensocial.Environment.ObjectType.ADDRESS);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - VIEWER (profile_details: urls)',
      id: 'PPL004',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view URLs information for VIEWER' +
                   ' profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.URLS)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.URLS];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewer').getData().
                    getField(opensocial.Person.Field.URLS);
                var expectedViewer = context.getExpectedViewer();
                var expected = expectedViewer[opensocial.Person.Field.URLS] ?
                    expectedViewer[opensocial.Person.Field.URLS] : [];

                var outcome = actual && expected ?
                   (Assert.assertEquals(actual.length, expected.length))
                   : undefined;
                if (!expectedViewer[opensocial.Person.Field.URLS]) {
                  outcome = actual.length > 0;
                }

                Helper.addSubResult(result, 'urls.length',
                    Assert.assertNotEmpty, 'length should match', outcome);

                Assert.assertSupportedObjectFields(result, actual, expected,
                    context.getData().urlFields,
                    opensocial.Environment.ObjectType.URL);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - VIEWER (profile_details: name)',
      id: 'PPL005',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view NAME of VIEWER profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.NAME)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.NAME];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewer').getData().
                    getField(opensocial.Person.Field.NAME);
                var expected = context.getExpectedViewer()
                    [opensocial.Person.Field.NAME];
                Assert.assertSupportedObjectFields(result, [actual], [expected],
                    context.getData().nameFields,
                    opensocial.Environment.ObjectType.NAME);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - VIEWER (profile_details: currentLocation)',
      id: 'PPL006',
      bugs: ['1027335'],
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view CURRENT_LOCATION of VIEWER profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.CURRENT_LOCATION)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.CURRENT_LOCATION];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewer').getData().
                    getField(opensocial.Person.Field.CURRENT_LOCATION);
                var expected = context.getExpectedViewer()
                    [opensocial.Person.Field.CURRENT_LOCATION];
                Assert.assertSupportedObjectFields(result, [actual], [expected],
                    context.getData().addressFields,
                    opensocial.Environment.ObjectType.ADDRESS);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },

    { name: 'newFetchPersonRequest - VIEWER (profile_details: gender)',
      id: 'PPL007',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view GENDER details of VIEWER profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.GENDER)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.GENDER];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual =
                    dataResponse.get('viewer').getData().getField('gender');

                Helper.addSubResult(result, 'getKey', Assert.assertNotNull,
                    actual.getKey(), 'Some value');

                Helper.addSubResult(result, 'getDisplayValue',
                    Assert.assertNotNull, actual.getDisplayValue(),
                    'Some value');
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - VIEWER (profile_details: bodyType)',
      id: 'PPL008',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view BODY_TYPE of VIEWER profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.BODY_TYPE)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.BODY_TYPE];
          req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewer'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewer').getData().
                    getField(opensocial.Person.Field.BODY_TYPE);
                var expected = context.getExpectedViewer()
                    [opensocial.Person.Field.BODY_TYPE];
                Assert.assertSupportedObjectFields(result, actual, expected,
                    context.getData().bodyTypeFields,
                    opensocial.Environment.ObjectType.BODY_TYPE);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - VIEWER (profile_details: schools)',
      id: 'PPL009',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view SCHOOLS of VIEWER profile.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.SCHOOLS)) {

        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
            [opensocial.Person.Field.SCHOOLS];
        req.add(req.newFetchPersonRequest('VIEWER', params), 'viewer');

        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewer').getData().
                  getField(opensocial.Person.Field.SCHOOLS);
              var expected = context.getExpectedViewer()['schools'] ?
                  context.getExpectedOwner()['schools'] : {};
              for (var i = 0; i < actual.length; i++) {
                Assert.assertObjectEquals('schools', actual,
                    context.getData().organizationFields, expected[i], result);
              }
            }
          }
          callback(result);
        });
        } else {
          callback(result);
        }
      }
    },


    /************************************
    * 1.2 newFetchPersonRequest - OWNER
    ************************************/

    { name: 'newFetchPersonRequest - OWNER (profile_details: addresses)',
      id: 'PPL104',
      bugs: ['1027335'],
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view address information of OWNER profile',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.ADDRESSES)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.ADDRESSES];
          req.add(req.newFetchPersonRequest('OWNER', params), 'owner');

          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('owner').getData()
                  .getField(opensocial.Person.Field.ADDRESSES);
              var expectedOwner = context.getExpectedOwner();
              var expected = expectedOwner[opensocial.Person.Field.ADDRESSES] ?
                  expectedOwner[opensocial.Person.Field.ADDRESSES] : undefined;

              Assert.assertSupportedObjectFields(result, actual, expected,
                  context.getData().addressFields,
                  opensocial.Environment.ObjectType.ADDRESS);
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - OWNER (profile_details: bodyType)',
      id: 'PPL105',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view BODY_TYPE of OWNER profile',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.BODY_TYPE)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.BODY_TYPE];
          req.add(req.newFetchPersonRequest('OWNER', params), 'owner');

          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], null);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('owner').getData().
                  getField(opensocial.Person.Field.BODY_TYPE);
              var expected = context.getExpectedOwner()
                  [opensocial.Person.Field.BODY_TYPE];
              Assert.assertSupportedObjectFields(result, actual, expected,
                  context.getData().bodyTypeFields,
                  opensocial.Environment.ObjectType.BODY_TYPE);
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - OWNER (profile_details: schools)',
      id: 'PPL106',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view SCHOOLS of OWNER profile',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.SCHOOLS)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.SCHOOLS];
          req.add(req.newFetchPersonRequest('OWNER', params), 'owner');

          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('owner').getData().
                  getField('schools');
              var expected = context.getExpectedOwner()['schools'] ?
                  context.getExpectedOwner()['schools'] : {};
              for (var i = 0; i < actual.length; i++) {
                Assert.assertObjectEquals('schools', actual,
                    context.getData().organizationFields, expected[i], result);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPersonRequest - OWNER (profile_details: urls)',
      id: 'PPL107',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view URLS of OWNER profile',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.URLS)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.URLS];
          req.add(req.newFetchPersonRequest('OWNER', params), 'owner');

          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('owner').getData().
                  getField(opensocial.Person.Field.URLS);
              var expected = context.getExpectedOwner()
                  [opensocial.Person.Field.URLS] ?
                  context.getExpectedOwner()[opensocial.Person.Field.URLS]
                  : undefined;

              if (actual && expected) {
                Helper.addSubResult(result, 'urls.length', Assert.assertEquals,
                    actual.length, expected.length);
              } else {
                Helper.addUnverifiedResult(result, 'urls.length',
                    actual.length);
              }

              Assert.assertSupportedObjectFields(result, actual, expected,
                  context.getData().urlFields,
                  opensocial.Environment.ObjectType.URL);
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },

    { name: 'newFetchPersonRequest - OWNER (profile_details: GENDER) test ENUM',
      id: 'PPL108',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can view GENDER information of OWNER profile',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        if (opensocial.getEnvironment().supportsField(
            opensocial.Environment.ObjectType.PERSON,
            opensocial.Person.Field.GENDER)) {

          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              [opensocial.Person.Field.GENDER];
          req.add(req.newFetchPersonRequest('OWNER', params), 'owner');

          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], null);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('owner').getData();

              Helper.addSubResult(result, 'Gender is in the gender Enum',
                  Assert.assertDefined,
                  opensocial.Enum.Gender[
                      actual.getField(
                          opensocial.Person.Field.GENDER).getKey()],
                  'Gender Display Name Equals');
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },



    /***************************************
    * 1.3 newFetchPersonRequest - OTHER IDS
    ***************************************/
    { name: 'newFetchPersonRequest - String ID',
      id: 'PPL200',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can view profile for given ID - numeric value',
      run: function(context, callback, result) {
        if (!context.isViewerWithoutApp()) {
          var req = opensocial.newDataRequest();
          var expected = context.getExpectedViewer();
          var viewerId = context.isViewerWithoutApp() ?
              context.getData().testUserData['id']
              : context.getViewer().getId();
          req.add(req.newFetchPersonRequest(viewerId), 'viewer');
          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], null);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewer').getData();
              // attach individual verification items to the result object
              Assert.assertObjectEquals('viewer', actual, null, expected, result);
              Assert.assertPersonFields(result, actual, expected,
                  context.getData().basicPersonFields,
                  opensocial.getEnvironment().supportedFields
                  [opensocial.Environment.ObjectType.PERSON]);
            }
            callback(result);
          });
        } else {
          Helper.addInfoSubResult(result, 'newFetchPersonRequest(\'id\')',
              Assert.assertDefined, 'Won\'t work without user id');
          callback(result);
        }
      }
    },


    /*********************************************
    * 1.4 newFetchPersonRequest - Error Handling
    *********************************************/
    { name: 'newFetchPersonRequest - Empty id. Error is expected.',
      id: 'PPL300',
      priority: Test.PRIORITY.P2,
      description: 'Test if you get error while trying to fetch profile ' +
                   'without passing any parameter.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonRequest(), 'someone');

        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['someone'], true);
          Assert.assertDataResponseItemHadError('someone',
              dataResponse.get('someone'), result, true);
          callback(result);
        });
      }
    },


    /*********************************************
    * 1.5 newFetchPersonRequest - Misc
    *********************************************/
    // PPL400

    /********************************************
    * 2.1 newFetchPeopleRequest - viewer's friends
    ********************************************/
    { name: 'newFetchPeopleRequest - viewer\'s friends  (default - no params) ',
      id: 'PPL501',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve viewer\'s friends information',
      run: function(context, callback, result) {
        var supportedPersonFields = opensocial.getEnvironment()
            .supportedFields[opensocial.Environment.ObjectType.PERSON] || [];
        var req = opensocial.newDataRequest();

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1})),
            'viewerFriends');
        // defaults to ALL, max is 20, details: id, name, isViewer, isOwner
        // and basic fields
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], null);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewerFriends').getData()
                  .asArray();
              var expectedViewer = context.getExpectedViewer();
              // default to max=20, sortByTopFriends, filterAll
              Assert.assertFriendProfileFields(result, actual, expectedViewer,
                  context.getData().basicPersonFields);
              Helper.addSubResult(result, 'actual.length <= 20',
                  Assert.assertTrue, actual.length <= 20, true);
            }
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - viewer friends  (profile_details basic)',
      id: 'PPL502',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve viewer\'s friends basic profile ' +
                   'detail.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
            context.getData().basicPersonFields;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'viewerFriends');

        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], null);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewerFriends').getData()
                  .asArray();
              var expectedFriends = context.getExpectedViewer() &&
                  context.getExpectedViewer()['__friends__'];
              // Attach individual verification items to the result object.
              Assert.assertFriendsEquals('viewerFriends', actual,
                  expectedFriends, context.getData().basicPersonFields,
                  null, result);
            }
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - viewer\'s friends  (all & default sort)' +
            ' ACL',
      id: 'PPL503',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can retrieve viewer\'s friends all details.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
            context.getData().personFields;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'viewerFriends');
        // defaults to ALL, max is 20, details: id, name, isViewer, isOwner
        // and only accessible fields should be returned
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewerFriends').getData()
                  .asArray();
              var expectedViewer = context.getExpectedViewer();
              // can only access fields user is allowed to access
              Assert.assertFriendProfileFields(result, actual, expectedViewer,
                  context.getData().basicPersonFields,
                  opensocial.getEnvironment().supportedFields
                  [opensocial.Environment.ObjectType.PERSON]);
              // default to max=20, sortByTopFriends, filterAll
              var expectedViewerCloned = Helper.cloneObject(expectedViewer);
              var expectedFriends = expectedViewerCloned &&
                  expectedViewerCloned['__friends__'];

              Helper.addSubResult(result, 'actual.length<=20',
                  Assert.assertTrue, actual.length <= 20, 'length<=20');

              // verify the default sorting is TOP_FRIENDS
              Assert.assertFriendsEquals('viewerFriends', actual,
                  expectedFriends, context.getData().basicPersonFields,
                  null, result);
            }
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - viewer\'s friends  (topFriends)',
      id: 'PPL504',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve viewer\'s top friends',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.SORT_ORDER] =
           opensocial.DataRequest.SortOrder.TOP_FRIENDS;

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'viewerFriends');
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewerFriends').getData()
                  .asArray();

              // Clone the expected VIEWER and sort their friends by name.
              var expectedViewerCloned = Helper.cloneObject(
                  context.getExpectedViewer());
              var expectedFriends = expectedViewerCloned &&
                  expectedViewerCloned['__friends__'];
              // Attach individual verification items to the result object.
              Assert.assertFriendsEquals('viewerFriends', actual,
                  expectedFriends, context.getData().basicPersonFields,
                  null, result);
            }
          }
          callback(result);
        });
      }
    },


    { name:  'newFetchPeopleRequest - viewer\'s friends  (hasApp)',
      id: 'PPL505',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve viewer\'s friends who HAS_APPS',
      run: function(context, callback, result) {
        if (PeopleSuite0_8.filteringHasAppSupported()) {
          var req = opensocial.newDataRequest();
          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.FILTER] =
             opensocial.DataRequest.FilterType.HAS_APP;
          req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
              {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
              params), 'viewerFriends');
          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewerFriends'], true);
            } else {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewerFriends'], false);
              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewerFriends').getData()
                    .asArray();
                var expectedViewer = context.getExpectedViewer();
                Assert.assertHasAppFriendFields(result, actual, expectedViewer,
                    context.getData().basicPersonFields);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },

    { name: 'newFetchPeopleRequest - viewer\'s friends  (paginated 1 per page)',
      id: 'PPL506',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can retrieve viewer\'s friends information ' +
                   'and access it as pages, 1 per page.',
      run: function(context, callback, result) {
        // restricting max to 20 just in case if someone has not implemented
        // spec properly.
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.MAX] = 20;

        var req = opensocial.newDataRequest();
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'viewerFriends');
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
            callback(result);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], false);
            if (!dataResponse.hadError()) {
              var dataCollection = dataResponse.get('viewerFriends').getData();

              Helper.addSubResult(result, 'viewerFriends.totalSize >= 0',
                  Assert.assertTrue, dataCollection.getTotalSize() >= 0,
                  'viewerFriends.totalSize >= 0');


              PeopleSuite0_8.assertAndCheckPersonData(0, 1, 0,
                  dataCollection.getTotalSize(), context, null, result,
                  callback);
            } else {
              callback(result);
            }
          }
        });
      }
    },

    /*******************************************
    * 2.2 newFetchPeopleRequest - owner's friends
    *******************************************/
    { name: 'newFetchPeopleRequest - owner\'s friends',
      id: 'PPL600',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve OWNER\'s friends information',
      run: function(context, callback, result) {
        var supportedPersonFields = opensocial.getEnvironment()
            .supportedFields[opensocial.Environment.ObjectType.PERSON] || [];
        var req = opensocial.newDataRequest();

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
            'ownerFriends');
        // defaults to ALL, max is 20, details: id, name, isOwner, isOwner
        // and basic fields
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('ownerFriends').getData().asArray();
            var expectedOwner = context.getExpectedOwner();
            // return basic fields by default
            Assert.assertHasAppFriendFields(result, actual, expectedOwner,
                context.getData().basicPersonFields);

            Helper.addSubResult(result, 'actual.length<=20', Assert.assertTrue,
                actual.length <= 20, 'length<=20');

          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - owner\' friends (profileDetails all) ACL',
      id: 'PPL601',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve OWNER\'s friends\'' +
                   ' profile details',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
            context.getData().supportedPersonFields;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        // defaults to ALL, max is 20, details: id, name, isOwner, isOwner
        // and only accessible fields should be returned
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('ownerFriends').getData().asArray();
            var expectedOwner = context.getExpectedOwner();
            // can only access fields user is allowed to access
            Assert.assertFriendProfileFields(result, actual, expectedOwner,
                context.getData().basicPersonFields,
                opensocial.getEnvironment().supportedFields
                [opensocial.Environment.ObjectType.PERSON]);
            // default to max=20, sortByTopFriends, filterAll

            Helper.addSubResult(result, 'actual.length<=20', Assert.assertTrue,
                actual.length <= 20, 'length<=20');

            var expectedOwnerCloned = Helper.cloneObject(expectedOwner);
            var expectedFriends = expectedOwnerCloned &&
                expectedOwnerCloned['__friends__'];
            Assert.assertFriendsEquals('ownerFriends', actual,
                expectedFriends, context.getData().basicPersonFields,
                null, result);
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - owner\' friends (SortOrder TOP_FRIENDS)',
      id: 'PPL602',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve OWNER\'s top friends',
      run: function(context, callback, result) {
        var supportedPersonFields = opensocial.getEnvironment()
          .supportedFields[opensocial.Environment.ObjectType.PERSON] || [];
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.SORT_ORDER] =
          opensocial.DataRequest.SortOrder.TOP_FRIENDS;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        // default to returning only basic fields
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('ownerFriends').getData().asArray();
            var expectedOwner = context.getExpectedOwner();
            var expectedOwnerCloned = Helper.cloneObject(expectedOwner);
            var expectedFriends = expectedOwnerCloned &&
              expectedOwnerCloned['__friends__'];
            Assert.assertFriendsEquals('ownerFriends', actual,
              expectedFriends, context.getData().basicPersonFields,
              null, result);
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - owner\' friends (first, max)',
      id: 'PPL603',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve subset of owner\'s friends i.e.' +
                   ' friends from entry 3 to 5 from returned list',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
            'ownerFriendsDefault');

        // with Params
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FIRST] = 1;
        params[opensocial.DataRequest.PeopleRequestFields.MAX] = 1;
        params[opensocial.DataRequest.PeopleRequestFields.SORT_ORDER] =
            opensocial.DataRequest.SortOrder.TOP_FRIENDS;

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');

        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actualDefaultCollection = dataResponse.get(
                'ownerFriendsDefault').getData();
            var actualCollection = dataResponse.get('ownerFriends').getData();
            var actual = actualCollection.asArray();

            Helper.logIntoResult(result, 'TotalSize'
                , actualDefaultCollection.getTotalSize());

            Helper.addSubResult(result,
                'getTotalSize(withMaxFilter vs. withoutMaxFilter)',
                Assert.assertTrue,
                actualDefaultCollection.getTotalSize() ==
                actualCollection.getTotalSize(), '');

            if (actualDefaultCollection.getTotalSize() >= 2) {
              Helper.addSubResult(result,
                  'actualCollection.size() != actualDefaultCollection.size()',
                  Assert.assertNotEquals, actualCollection.size(),
                  actualDefaultCollection.size());
            }
            // Clone and sort by default sorting order, used as expected value.
            var expectedOwnerCloned =
                Helper.cloneObject(context.getExpectedOwner());
            if (expectedOwnerCloned['__friends__'] != undefined) {

              Assert.assertObjectEquals('ownerFriends', actual[0], null,
                  expectedOwnerCloned['__friends__'][1], result);
              Assert.assertObjectEquals('ownerFriends',
                  actual[0], context.getData().basicPersonFields,
                  expectedOwnerCloned['__friends__'][1], result);
            }
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - owner\' friends (max = 21)',
      id: 'PPL604',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve more than 20 friends of OWNER',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.MAX] = 21;
        var ownerFriends = context.getExpectedOwner()['__friends__'];
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'friends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['friends'], false);
          if (!dataResponse.hadError() && ownerFriends != undefined) {
            var actual = dataResponse.get('friends').getData().asArray();

            if (ownerFriends.length > 21) {
              Helper.addSubResult(result, 'friends.length',
                  Assert.assertEquals, actual.length, 21);
            } else {
              Helper.addSubResult(result, 'friends.length',
                  Assert.assertEquals, actual.length, ownerFriends.length);
            }
          }
          callback(result);
        });
      }
    },


     { name:  'newFetchPeopleRequest - owner\' friends (hasApp)',
      id: 'PPL605',
       priority: Test.PRIORITY.P0,
       description: 'Test if you can retrieve friends of OWNER who HAS_APP',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FILTER] =
            opensocial.DataRequest.FilterType.HAS_APP;

        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('ownerFriends').getData().asArray();
            var expectedOwner = context.getExpectedOwner();
            Assert.assertHasAppFriendFields(result, actual, expectedOwner,
                context.getData().basicPersonFields);
          }
          callback(result);
        });
      }
    },

    { name:  'newFetchPeopleRequest - owner\'s friends (hasApp, Max=100)',
     id: 'PPL606',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve upto 100 friends of ' +
                   'OWNER who HAS_APP',
     run: function(context, callback, result) {
       var req = opensocial.newDataRequest();
       var params = {};
       params[opensocial.DataRequest.PeopleRequestFields.MAX] = 100;
       params[opensocial.DataRequest.PeopleRequestFields.FILTER] =
           opensocial.DataRequest.FilterType.HAS_APP;
       req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
           {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
           params), 'ownerFriends');
       req.send(function(dataResponse) {
         Assert.assertDataResponseHadError(dataResponse, result,
             ['ownerFriends'], false);
         if (!dataResponse.hadError()) {
           var actual = dataResponse.get('ownerFriends').getData().asArray();
           var expectedOwner = context.getExpectedOwner();
           Assert.assertHasAppFriendFields(result, actual, expectedOwner,
               context.getData().basicPersonFields);
         }
         callback(result);
       });
     }
   },

    /***************************************
    * 2.3 newFetchPeopleRequest - Other IDS
    ***************************************/
    { name: 'newFetchPeopleRequest - String ID',
      id: 'PPL700',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve friends of profile with given id',
      run: function(context, callback, result) {
        if (context.isViewerWithoutApp()) {
          var req = opensocial.newDataRequest();
          var expected = context.getExpectedViewer();
          var viewerId = context.isViewerWithoutApp() ?
              context.getData().testUserData['id']
              : context.getViewer().getId();
          req.add(req.newFetchPeopleRequest(viewerId), 'viewer');
          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewer').getData().asArray();
              // attach individual verification items to the result object
              Assert.assertObjectEquals('viewer',
                  actual[0], null, expected, result);
              Assert.assertPersonFields(result, actual[0], expected,
                  context.getData().basicPersonFields,
                  opensocial.getEnvironment().supportedFields
                  [opensocial.Environment.ObjectType.PERSON]);
            }
            callback(result);
          });
        } else {
          Helper.addInfoSubResult(result, 'newFetchPersonRequest(\'id\')',
              Assert.assertDefined, 'Won\'t work without user id');
          callback(result);
        }
      }
    },


    { name: 'newFetchPeopleRequest - Array<String> IDs (all hasApp)',
      id: 'PPL701',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can retrieve friends profiles for list of ids' +
                   ' with HAS_APP=true',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var expected = context.getExpectedViewer();
        var expectedFriends = expected && expected['__friends__'] ?
            expected['__friends__'] : [];
        var expectedFriendsWithApp = [];
        var expectedFriendsWithAppIds = [];
        for (var i = 0; i < expectedFriends.length; i++) {
          if (expectedFriends[i]['hasPeopleApp']) {
            expectedFriendsWithApp.push(expectedFriends[i]);
            expectedFriendsWithAppIds.push(expectedFriends[i]['id']);
          }
        }

        if (expectedFriendsWithApp.length > 0) {
          var params = {};
          params[opensocial.DataRequest.PeopleRequestFields.PROFILE_DETAILS] =
              context.getData().personFields;
          params[opensocial.DataRequest.PeopleRequestFields.FILTER] =
              opensocial.DataRequest.FilterType.HAS_APP;
          req.add(req.newFetchPeopleRequest(expectedFriendsWithAppIds, params),
              'viewerFriendsWithApp');
          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriendsWithApp'], false);
            if (!dataResponse.hadError()) {
              var responseItem = dataResponse.get('viewerFriendsWithApp');
              Assert.assertDataResponseItemHadError('viewerFriendsWithApp',
                  responseItem, result, false);
              var actual = responseItem.getData().asArray();
              Assert.assertHasAppFriendFields(result, actual,
                  context.getExpectedViewer(),
                  context.getData().personFields);

              for (var i = 0; i < context.getData().personFields.length; i++) {
                var f = context.getData().personFields[i];
                var subtest;
                var fieldValue = actual[0].getField(f);
                if (Helper.indexOf(context.getData().supportedPersonFields,
                    f) < 0) {
                  if (!Assert.assertNull(fieldValue)) {

                    Helper.addSubResult(result,
                        'non-container-supported field - ' + f,
                        Assert.assertNull, fieldValue,
                        'non-supported field should be NULL');
                  }
                }
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPeopleRequest - Array<String> IDs (hasApp false)',
      id: 'PPL702',
      priority: Test.PRIORITY.P2,
      description: 'Test if you can retrieve friends profiles for list of ids' +
                   ' with HAS_APP=false',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var expected = context.getExpectedViewer();
        var expectedFriends = expected && expected['__friends__'] ?
            expected['__friends__'] : [];
        var expectedFriendsWithoutApp = [];
        var expectedFriendsWithoutAppIds = [];
        for (var i = 0; i < expectedFriends.length; i++) {
          if (!expectedFriends[i]['hasPeopleApp']) {
            expectedFriendsWithoutApp.push(expectedFriends[i]);
            expectedFriendsWithoutAppIds.push(expectedFriends[i]['id']);
          }
        }
        if (expectedFriendsWithoutApp.length > 0) {
          req.add(req.newFetchPeopleRequest(expectedFriendsWithoutAppIds),
              'viewerFriendsWithoutApp');
          req.send(function(dataResponse) {
            Assert.assertDataResponseHadError(dataResponse, result, null, true);
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPeopleRequest - Array<String> IDs',
      id: 'PPL703',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve friends profiles for list of ids',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var expectedViewer = context.getExpectedViewer();
        if (expectedViewer && expectedViewer['__friends__'] &&
            expectedViewer['__friends__'][0] &&
            expectedViewer['__friends__'][1]) {
          req.add(req.newFetchPeopleRequest(
              [expectedViewer['__friends__'][0]['getId'],
              expectedViewer['__friends__'][1]['getId']]), 'viewerFriend');

          req.send(function(dataResponse) {
            if (context.isViewerWithoutApp()) {
              Assert.assertDataResponseHadError(dataResponse, result,
                  ['viewerFriend'], true);
            } else {
              if (expectedViewer['__friends__'][0]['hasPeopleApp'] &&
                  expectedViewer['__friends__'][1]['hasPeopleApp']) {
                Assert.assertDataResponseHadError(dataResponse, result,
                    ['viewerFriend'], false);
              }

              if (!dataResponse.hadError()) {
                var actual = dataResponse.get('viewerFriend').getData()
                    .asArray();
                // Check both length.
                Helper.addSubResult(result, 'people.length', Assert.assertTrue,
                    actual.length == 2, 2);
              }
            }
            callback(result);
          });
        } else {
          callback(result);
        }
      }
    },


    { name: 'newFetchPeopleRequest - VIEWER',
      id: 'PPL704',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve VIEWER information using ' +
                   'newFetchPeopleRequest()',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var expectedViewer = context.getExpectedViewer();
        req.add(req.newFetchPeopleRequest(Config.VIEWER), 'viewer');
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewer'], false);
            if (!dataResponse.hadError()) {
              var actual = dataResponse.get('viewer').getData().asArray();

              // Attach individual verification items to the result object.
              Assert.assertObjectEquals('viewer', actual[0], null,
                  expectedViewer, result);
              Assert.assertObjectEquals('viewer', actual[0],
                  context.getData().basicPersonFields, expectedViewer, result);
            }
          }
          callback(result);
        });
      }
    },


    { name: 'newFetchPeopleRequest - OWNER',
      id: 'PPL705',
      priority: Test.PRIORITY.P0,
      description: 'Test if you can retrieve OWNER information using ' +
                   'newFetchPeopleRequest()',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var expectedOwner = context.getExpectedOwner();
        req.add(req.newFetchPeopleRequest(Config.OWNER), 'owner');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['owner'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('owner').getData().asArray();

            // Attach individual verification items to the result object.
            Assert.assertObjectEquals('owner', actual[0],  null,
                expectedOwner, result);
            Assert.assertObjectEquals('owner', actual[0],
                context.getData().basicPersonFields, expectedOwner, result);
          }
          callback(result);
        });
      }
    },


    /********************************************
    * 2.4 newFetchPeopleRequest - Error Handling
    ********************************************/
     { name: 'newFetchPeopleRequest - with invalid opt_params',
      id: 'PPL801',
      bugs: ['1047119'],
      priority: Test.PRIORITY.P2,
      description: 'Test if you get error for passing wrong params to ' +
                   'newFetchPeopleRequest()',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();

        // Wrong kind of parameter. Must generate an error BAD_REQUEST
        var params = { 'AA' : 'AA', 'max' : 'a' , 'first' : 'b'};
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'friends');

        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['friends'], true);
          Assert.assertDataResponseItemHadError('friends',
              dataResponse.get('friends'),
              result, true, opensocial.ResponseItem.Error.BAD_REQUEST);
          callback(result);
        });
      }
    },

    { name: 'newFetchPeopleRequest - viewer\'s friends  (paging trying record' +
            ' out of bounds)',
      id: 'PPL802',
      priority: Test.PRIORITY.P2,
      description: 'Test if you get error for trying to fetch VIEWER\'s ' +
                   'friends page that\'s out of bounds. i.e. accessing 1000th' +
                   ' page where only 10 pages exists',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FIRST] = 99999;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'viewerFriends');
        req.send(function(dataResponse) {
          if (context.isViewerWithoutApp()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], true);
          } else {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['viewerFriends'], false);
            if (!dataResponse.hadError()) {
              var dataCollection = dataResponse.get('viewerFriends').getData();
              Helper.addSubResult(result, 'viewerFriends.totalSize >= 0',
                  Assert.assertTrue, dataCollection.getTotalSize() >= 0,
                  'viewerFriends.totalSize >= 0');
              Helper.addSubResult(result, 'viewerFriends.size == 0',
                  Assert.assertTrue, dataCollection.size() == 0,
                  'viewerFriends.size == 0');
            }
          }
          callback(result);
        });
      }
    },

    { name: 'newFetchPeopleRequest - owner\'s friends (paging trying record out '
          + 'of bounds)',
      id: 'PPL803',
      priority: Test.PRIORITY.P2,
      description: 'Test if you get error for trying to fetch OWNER\'s ' +
                   'friends page that\'s out of bounds. i.e. accessing 1000th' +
                   ' page where only 10 pages exists',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FIRST] = 99999;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var dataCollection = dataResponse.get('ownerFriends').getData();
            Helper.addSubResult(result, 'ownerFriends.totalSize >= 0',
                Assert.assertTrue, dataCollection.getTotalSize() >= 0,
                'ownerFriends.totalSize >= 0');
            Helper.addSubResult(result, 'ownerFriends.size == 0',
                Assert.assertTrue, dataCollection.size() == 0,
                'ownerFriends.size == 0');
          }
          callback(result);
        });
      }
    },

    /********************************************
    * 2.5 newFetchPeopleRequest - Misc
    ********************************************/
    { name: 'Collection - Collection.each(Person)',
      id: 'PPL900',
      priority: Test.PRIORITY.P2,
      description: 'Test if fetching owner\'s friends gives collection object ' +
                   ' and collection has no empty record',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
            'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var perCollection = dataResponse.get('ownerFriends').getData();
            var atLeastOneDoesntExist = false;
            perCollection.each(function(person) {
              if (perCollection.getById(person.getId()) == null) {
                atLeastOneDoesntExist = true;
              }
            });
            Helper.addSubResult(result, 'Collection.each()',
                Assert.assertFalse, atLeastOneDoesntExist,
                'False. All persons must exist')
          }
          callback(result);
        });
      }
    },


    { name: 'Collection - size() and totalSize()',
      id: 'PPL901',
      priority: Test.PRIORITY.P2,
      description: 'Test if fetching owner\'s friends  gives collection ' +
                   'object and collection size is right',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
            'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var actual = dataResponse.get('ownerFriends').getData();
            var friends = context.getExpectedViewer()['__friends__'];
            var smallLength = friends ? (friends.length <= 20) : undefined;
            var largeLength = friends ? (friends.length > 20) : undefined;

            if (smallLength || largeLength) {
              if (smallLength) {
                Helper.addSubResult(result, 'size() & totalSize()',
                    Assert.assertEquals, actual.getTotalSize(), actual.size());
              } else {
                Helper.addSubResult(result, 'size() & totalSize()',
                    Assert.assertNotEquals, actual.getTotalSize(),
                    actual.size());
              }

            } else {
              Helper.addUnverifiedResult(result, 'size() & totalSize()',
                  'Total:' + actual.getTotalSize() + ' & ' + 'Size:' +
                  actual.size());
            }
            if (friends) {
              Helper.addSubResult(result, 'Collection.size()',
                  Assert.assertEquals, actual.size(), friends.length);
            } else {
              Helper.addSubResult(result, 'Collection.size()',
                  Assert.assertTrue, actual.size() >= 0, actual.size());
          }
        }
        callback(result);
        });
      }
    },

    { name: 'Collection -  getOffset()',
      id: 'PPL902',
      priority: Test.PRIORITY.P2,
      description: 'Test if fetching owner\'s friends  gives collection' +
                   ' object getOffset gives the offset we assigned.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FIRST] = 1;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var dataCollection = dataResponse.get('ownerFriends').getData();
            Helper.addSubResult(result, 'Collection - getOffset>0',
                Assert.assertTrue, dataCollection.getOffset() > 0,
                'getOffset > 0');
          }
          callback(result);
        });
      }
    },

    { name: 'Collection -  getOffset() huge offset',
      id: 'PPL903',
      priority: Test.PRIORITY.P2,
      description: 'Test if fetching owner\'s friends  gives collection ' +
                   'object and preserves huge offset we have set.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var params = {};
        params[opensocial.DataRequest.PeopleRequestFields.FIRST] = 9999;
        req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            params), 'ownerFriends');
        req.send(function(dataResponse) {
          Assert.assertDataResponseHadError(dataResponse, result,
              ['ownerFriends'], false);
          if (!dataResponse.hadError()) {
            var dataCollection = dataResponse.get('ownerFriends').getData();
            Helper.addSubResult(result, 'Collection - getOffset > 0',
                Assert.assertTrue, dataCollection.getOffset() > 0,
                'getOffset > 0');
          }
          callback(result);
        });
      }
    }

  ]
};

/**
 * Utility method for test case: PPL506
 * recursively fetch all opensocial.newIdSpec(
 * {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1})
 * simulating paging over the friend collection and checking for every entry
 * the data returned.
 * @param {number} current Currently fetched record.
 * @param {number} pageSize the page size, or the maximum size of the collection
 * @param {number} pageNumber the current page number
 * @param {Context} context The opensocial testing context
 * @param {opensocial.Collection} dataCollection The data collection of
 *     opensocial.newIdSpec(
 *     {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1})
 *     that contain the people data
 * @param {ResultGroup} the result from the test
 * @param {Function} callback the function that is invoked after the test has
 *     been completed
 */
PeopleSuite0_8.assertAndCheckPersonData = function(current, pageSize, pageNumber,
    totalRecords, context, dataCollection, result, callback) {
  var total = totalRecords;
  if (dataCollection) {
    Helper.addSubResult(result, 'Last getTotalSize == getTotalSize',
        Assert.assertEquals, dataCollection.getTotalSize(), totalRecords);
    Helper.addSubResult(result, 'page[' + pageNumber + '] '
        + 'viewerFriends.size <= ' + pageSize,
        Assert.assertTrue, dataCollection.size() <= pageSize,
        'dataCollection.size() <= pageSize');
    dataCollection.each(function(person) {
      var expectedViewer = context.getExpectedViewer();
      Assert.assertFriendProfileFields(result, dataCollection.asArray(),
          expectedViewer, context.getData().basicPersonFields);
      Helper.addSubResult(result, 'person[' + current + '].name',
          Assert.assertDefined, person.getDisplayName(), 'some value');
      current++;
    });
    pageNumber++;
    total = dataCollection.getTotalSize();
  }
  if (current < total) {
    var req = opensocial.newDataRequest();
    var params = {};
    params[opensocial.DataRequest.PeopleRequestFields.MAX] = pageSize;
    params[opensocial.DataRequest.PeopleRequestFields.FIRST] = current;
    req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
        {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}), params),
        'viewerFriends');
    req.send(function(dataResponse) {
      Assert.assertDataResponseHadError(dataResponse, result,
          ['viewerFriends'], false);
      if (!dataResponse.hadError() && dataResponse.get('viewerFriends')) {
        PeopleSuite0_8.assertAndCheckPersonData(current, pageSize, pageNumber,
            totalRecords, context, dataResponse.get('viewerFriends').getData(),
            result, callback);
      } else {
        Assert.assertDataResponseHadError(dataResponse, result,
            ['viewerFriends'], false);
        callback(result);
      }
    });
  } else {
    Helper.addSubResult(result, 'Total Pages:', Assert.assertEquals, pageNumber,
        totalRecords / pageSize);
    callback(result);
  }
};


PeopleSuite0_8.filteringHasAppSupported = function() {
  return opensocial.getEnvironment().supportsField(
      opensocial.Environment.ObjectType.FILTER_TYPE,
      opensocial.DataRequest.FilterType.HAS_APP);
}


PeopleSuite0_8.counter = {};
