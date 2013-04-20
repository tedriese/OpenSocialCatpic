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
 * @fileoverview appdata related data requests tests.
 *
 * 1) Person requests: APP000
 * - newFetchPersonAppRequest fetch APP000
 * - newFetchPersonAppRequest error handling APP100
 * - newFetchPersonAppRequest acl APP200
 * - newFetchPersonAppRequest batch requests APP300
 * - newFetchPersonAppRequest response item APP400
 *
 * 2) Instance requests: APP500 and up
 * - newFetchInstanceAppDataRequest fetch APP500
 * - newFetchInstanceAppDataRequest error handling APP600
 * - newFetchInstanceAppDataRequest acl APP700
 */
function AppDataSuite0_8() {
  this.name = 'AppData Test Suite';
  this.id = 'APP';
  this.tests = [

    /**
     * Person App Data - fetch
     */
    { name: 'opensocial.DataRequest.newFetchPersonAppDataRequest('
        + 'opensocial.newIdSpec({userId: \'VIEWER\'}), \'key\')',
      id: 'APP000',
      description: '',
      run: function(context, callback, result) {
        var viewer = context.getViewer();
        this.data.personalValue = 'personalValue ' + new Date().getTime();
        var req = opensocial.newDataRequest();
        req.add(req.newUpdatePersonAppDataRequest(
            'VIEWER', 'viewerKey_app000', this.data.personalValue));
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'VIEWER'}), 'viewerKey_app000'),
            'per_app000');
        /*
         * Update as VIEWER and fetch using OWNER key since update using OWNER
         * is currently not supported.  Use this as workaround to test fetch
         * using OWNER ID.
         */
        if (viewer.isOwner()) {
          req.add(req.newUpdatePersonAppDataRequest(
              opensocial.IdSpec.PersonId.VIEWER, this.id + '_dataKey2',
              this.data.personalValue), 'updateOwner_app000');
          req.add(req.newFetchPersonAppDataRequest(
              opensocial.newIdSpec({userId: 'OWNER'}), [this.id + '_dataKey2']),
              'ownerFetch_app000');
        }
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys: ['per_app000', 'ownerFetch_app000'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
            , context, ['per_app000'])) {
          var personData = dataResponse.get('per_app000').getData();
          var viewer = context.getViewer();
          var actual = (personData && personData[viewer.getId()]) ?
              personData[viewer.getId()]['viewerKey_app000'] : undefined;
          Helper.addSubResult(result, 'personalData.getData()',
              Assert.assertEquals, actual, this.data.personalValue);

          /*
           * Update as VIEWER and fetch using OWNER key since update using
           * OWNER is currently not supported.
           */
          if (viewer.isOwner()) {
            // Get update result and assert there are no errors.
            var updateResult = dataResponse.get('updateOwner_app000');
            Helper.addUnsevereSubResult(result, 'updateOwner_app000 hadError()',
                Assert.assertFalse, updateResult.hadError(), 'false');

            // Get fetch result and assert there are no errors.
            var fetchResult = dataResponse.get('ownerFetch_app000');
            Helper.addUnsevereSubResult(result, 'ownerFetch_app000 hadError()',
                Assert.assertFalse, fetchResult.hadError(), 'false');

            if (!fetchResult.hadError()) {
              personData = dataResponse.get('ownerFetch_app000').getData();
              actual = (personData && personData[viewer.getId()]) ?
                  personData[viewer.getId()][this.id + '_dataKey2'] : undefined;
              Helper.addUnsevereSubResult(result, 'ownerPersonalData.getData()',
                  Assert.assertEquals, actual, this.data.personalValue);
            }
          }
        }
      }
    },

    { name: 'opensocial.DataRequest.newFetchPersonAppDataRequest(' +
            'idSpec({userId :ownerId}), \'*\')',
      id: 'APP001',
      description: 'Creates an item to request app data for some id and with' +
                   '* as key which fetches all the values. Validates ' +
                   'the returned data, if it contains the value for the ' +
                   'specified key',
      queue: 6,
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var owner = context.getOwner();
        this.data.selfId = owner.getId();
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId : this.data.selfId }), '*'),
            'fetchWithIds');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys: ['fetchWithIds'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
            , context, ['fetchWithIds'])) {
          var personData = dataResponse.get('fetchWithIds').getData();
          var actual = (personData && personData[this.data.selfId]) ?
              personData[this.data.selfId] : undefined;
          if (actual != undefined) {
            for (var key in actual) {
              Helper.addSubResult(result,
                  'starData[' + this.data.selfId + '][' + key + ']',
                  Assert.assertDefined, actual[key], 'defined');
            }
          } else {
            Helper.addUnsevereSubResult(result,
                'newFetchPersonAppDataRequest(oenweId, \'*\')', Assert.fail,
                actual, 'defined');
          }
        }
      }
    },


    { name: 'opensocial.DataRequest.newFetchPersonAppDataRequest('
        + 'opensocial.newIdSpec({userId: \'VIEWER\'}), \'*\')',
      id: 'APP002',
      description: 'Creates an item to request app data for VIEWER and with' +
                   '* as key which fetches all the values. Validates ' +
                   'the returned data, if it contains the value for the ' +
                   'specified key',
      run: function(context, callback, result) {
        this.data.personalValue = 'personalValue ' + new Date().getTime();
        var req = opensocial.newDataRequest();

        // update info for 2 keys for same user
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            this.id + '_dataKey1', this.data.personalValue), 'k1Update');
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            this.id + '_dataKey2', this.data.personalValue), 'k2Update');

        // fetch info using "*"
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'VIEWER'}), '*' ), 'per_app002');

        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },

      responseKeys: ['per_app002'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
            , context, ['k1Update', 'k2Update'])) {
          var viewer = context.getViewer();
          var personData =
              dataResponse.get('per_app002').getData()[viewer.getId()];
          Helper.addSubResult(result, '(using *)personalData.getData(key1)',
              Assert.assertEquals, personData[this.id + '_dataKey1'],
              this.data.personalValue);
          Helper.addSubResult(result, '(using *)personalData.getData(key2)',
              Assert.assertEquals, personData[this.id + '_dataKey1'],
              this.data.personalValue);
        }
      }
    },


    { name: 'newUpdate/FetchPersonAppDataRequest - 1 user, fetch [key1, key2]',
      id: 'APP003',
      description: 'Tests if information is updated for the VIEWER' +
                   'with the keys. Fetch for the information by passing the' +
                   'same keys which were used for updating.',
      run: function(context, callback, result) {
        this.data.personalValue = 'personalValue ' + new Date().getTime();
        var req = opensocial.newDataRequest();

        //update info for 2 keys for same user
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            this.id + '_dataKey1', this.data.personalValue), 'key_update1');
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            this.id + '_dataKey2', this.data.personalValue), 'key_update2');

        // fetch info using Array[]. Use the same set of data uploaded before.
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'VIEWER'}), [this.id + '_dataKey1',
            this.id + '_dataKey2']), 'per_app003');

        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys: ['per_app003'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
            , context, ['per_app003', 'key_update1', 'key_update2'])) {
          var viewer = context.getViewer();
          var personData =
              dataResponse.get('per_app003').getData()[viewer.getId()];
          Helper.addSubResult(result,
              '(using Array)personalData.getData(dataKey1)',
              Assert.assertEquals, personData[this.id + '_dataKey1'],
              this.data.personalValue);
          Helper.addSubResult(result,
              '(using Array)personalData.getData(dataKey2)',
              Assert.assertEquals, personData[this.id + '_dataKey2'],
              this.data.personalValue);
        }
      }
    },

    { name: 'newUpdate/FetchPersonAppDataRequest - I18N',
      id: 'APP004',
      description: 'Tests if information is updated for the specified VIEWER' +
                   'with the i18n keys. Fetch for the information by passing ' +
                   'the same keys which were used for updating.',
      run: function(context, callback, result) {
        this.data.unicode =
            '\u00E1\u00E9\u00ED\u00F3\u0278' + new Date().getTime();
        var req = opensocial.newDataRequest();
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            'i18nKey_app004', this.data.unicode), 'key_app004_update1');
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'VIEWER'}), 'i18nKey_app004'),
            'per_app004');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys: ['per_app004'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result,
            context, ['per_app004', 'key_app004_update1'])) {
          var actual = dataResponse.get('per_app004')
              .getData()[context.getViewer().getId()]['i18nKey_app004'];
          Helper.addSubResult(result, 'I18N PersonalData',
              Assert.assertEquals, actual, this.data.unicode);
        }
      }
    },


    { name: 'newFetchPersonAppDataRequest - Owner Friends' +
            '{userId : \'OWNER\', groupId : \'FRIENDS\', networkDistance : 1}',
      id: 'APP005',
      description: 'Tests if information can be retreived from OWNER_FRIENDS ' +
                   'with the given keys.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec(
            {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
            '*'), 'ownerFriendPer');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },


      validation: function(dataResponse, result, context) {
        var ownerFriendData = (dataResponse.get('ownerFriendPer')) ?
            dataResponse.get('ownerFriendPer').getData() : undefined;
        var countFriendsWithData = 0;
        var outcome = false;
        if (ownerFriendData) {
          for (var prop in ownerFriendData) {
            if (ownerFriendData.hasOwnProperty(prop)) {
              countFriendsWithData++;
              var friendPerData = ownerFriendData && ownerFriendData[prop];
              for (var key in friendPerData) {
                var friendPerDataValue = friendPerData && friendPerData[key];
                Helper.addSubResult(result,
                    'OwnerFriendData["' + prop + '"]["' + key + '"]',
                    Assert.assertDefined, friendPerDataValue, 'not undefined');
              }
            }
          }
          Helper.addInfoSubResult(result, 'OwnerFriendData.keys().size()',
              Assert.assertDefined, countFriendsWithData, 'defined');
        }
      }
    },


    { name: 'newFetchPersonAppDataRequest - Viewer Friends' +
            '{userId : \'VIEWER\', groupId : \'FRIENDS\', networkDistance : 1}',
      id: 'APP006',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            '*'), 'viewerFriendPer');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys: ['viewerFriendPer'],

      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result,
            context, ['viewerFriendPer'])) {
          var viewerFriendData = (dataResponse.get('viewerFriendPer')) ?
            dataResponse.get('viewerFriendPer').getData() : undefined;
          var countFriendsWithData = 0;
          var outcome = false;
          if (viewerFriendData) {
            for (var prop in viewerFriendData) {
              if (viewerFriendData.hasOwnProperty(prop)) {
                countFriendsWithData++;
                var friendPerData = viewerFriendData &&
                    viewerFriendData[prop];
                for (var key in friendPerData) {
                  var friendPerDataValue = friendPerData && friendPerData[key];
                  Helper.addSubResult(result,
                    'ViewerFriendData["' + prop + '"]["' + key + '"]',
                    Assert.assertDefined, friendPerDataValue, 'not undefined');
                }
              }
            }
          }
          Helper.addInfoSubResult(result, 'ViewerFriendData.keys().size()',
              Assert.assertDefined, countFriendsWithData);
        }
      }
    },

     { name: 'newUpdatePersonAppDataRequest - emptyString - *',
       id: 'APP009',
       description: 'Updating data using * (all keys), with an empty string ' +
                    'value, should remove all keys from the user app data.',
       queue: 8,
       run: function(context, callback, result) {
         var runner = new Helper.FunctionRunner();
         runner.add(this.doAdd, [result, runner.getSequencer()], this);
         runner.add(this.doRequest
             , [result, context ,runner.getSequencer()]
             , this);
         runner.add(this.doDelete, [result, runner.getSequencer()]);
         runner.add(this.doRequestAll, [result, runner.getSequencer()]);
         runner.add(callback, [result]);
         runner.run();
       },

       doAdd: function(result, callback) {
         var req = opensocial.newDataRequest();
         var id = this.id;
         req.add(req.newUpdatePersonAppDataRequest('VIEWER',
             id + '_dataKey1', 'some value1'), 'updateKey1_app009');
         req.add(req.newUpdatePersonAppDataRequest('VIEWER',
             id + '_dataKey2', 'some value2'), 'updateKey2_app009');
         req.send(function (dataResponse) {
           Assert.assertDataResponseHadError(dataResponse, result
               , ['updateKey1_app009', 'updateKey2_app009'], false);
           callback();
         });
         return true;
       },

       doRequest: function(result, context, callback) {
         var req = opensocial.newDataRequest();
         var id = this.id;
         req.add(req.newFetchPersonAppDataRequest(
             opensocial.newIdSpec({userId: 'VIEWER'}), '*'),
             'requestData_app009');
         req.send(function (dataResponse) {
           if (Assert.assertDataResponseShouldContinue(
               dataResponse, result, context, ['requestData_app009'])) {
             Assert.assertDataResponseHadError(dataResponse, result
                 , ['requestData_app009'], false);
             var read1ResponseData = dataResponse.get('requestData_app009')
                 .getData()[context.getViewer().getId()];
             Helper.addSubResult(result, 'Verify key is set',
                 Assert.assertEquals,
                 read1ResponseData[id + '_dataKey1'], 'some value1');
             Helper.addSubResult(result, 'Verify key is set',
                 Assert.assertEquals,
                 read1ResponseData[id + '_dataKey2'], 'some value2');
           }
           callback();
         });
         return true;
       },

       doDelete: function(result, callback) {
         var req = opensocial.newDataRequest();
         req.add(req.newRemovePersonAppDataRequest('VIEWER', '*'),
             'deleteKeys');
         req.send(function (dataResponse) {
           if (dataResponse.hadError()) {
             result.addSubResult("dataResponse.hadError() :", Assert.fail,
                 true, false);
           } else {
             Assert.assertDataResponseHadError(dataResponse, result,
                 ['deleteKeys'], false);
           }
           callback();
         });
         return true;
       },

       doRequestAll: function(result, callback) {
         var req = opensocial.newDataRequest();
         req.add(req.newFetchPersonAppDataRequest(
             opensocial.newIdSpec({userId: 'VIEWER'}), '*'), 'requestData_009');
         req.send(function (dataResponse) {
           Assert.assertDataResponseHadError(dataResponse, result
               , ['requestData_009'], false);
           var read2Response = dataResponse.get('requestData_009');
           Helper.addSubResult(result, 'Verify keys were removed',
               Assert.assertEmptyMap, read2Response.getData(), 'empty map');
           callback();
         });
         return true;
       }
     },

    /**
     * Try to fetch data as some user requested.
     */
    { name: 'newFetchPersonAppDataRequest - fetching multiple keys '
          + 'existing/not existing for viewer and ownerFriends',
      id: 'APP011',
      description: 'Tests if information is fetched from VIEWER and ' +
                   'OWNER_FRIENDS for the specified fields. Validates the ' +
                   'data response for every key',
      run: function(context, callback, result) {
        // As provided from a user bug report
       var req = opensocial.newDataRequest();
       var fields = [ 'AppField1', 'AppField2', 'AppField3', 'viewerKey_011' ];
       req.add(req.newUpdatePersonAppDataRequest(
           'VIEWER', 'viewerKey_011','some value1'), 'updateKey1_app011');
       req.add(req.newFetchPersonAppDataRequest(
           opensocial.newIdSpec({userId: 'VIEWER'}), fields), 'viewer_data');
       req.add(req.newFetchPersonAppDataRequest(
           opensocial.newIdSpec(
           {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
           fields), 'ownerFriendsData');
       req.send(Helper.createValidationCallback(this, callback, context,
           result));
      },

      responseKeys: ['viewer_data', 'ownerFriendsData'],

      validation: function(dataResponse, result, context) {
        Assert.assertDataResponseHadError(dataResponse, result,
            this.responseKeys, false);
        for (var i = 0; i < this.responseKeys.length; i++) {
          var responseItem = dataResponse.get(this.responseKeys[i]);
          Assert.assertDataResponseItemHadError(this.responseKeys[i],
              responseItem, result, false);
          Helper.addSubResult(result,
              dataResponse, true, '', '');
        }
      }
    },

    /**
     * Person App Data - Error Handling
     */
    { name: 'opensocial.DataRequest.newFetchPersonAppDataRequest('
          + 'opensocial.newIdSpec({userId: \'OWNER\'}, \'key\')',
      id: 'APP100',
      description: 'Creates an item to request app data for OWNER and with' +
                   'some key which fetches values. Validates the returned ' +
                   'data, if it contains the value for the specified key',
      run: function(context, callback, result) {
        var data = 'personalValue ' + new Date().getTime();
        var req = opensocial.newDataRequest();
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            'ownerKey_app100', data), 'dataUpdate_app100');
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'OWNER'}), ['ownerKey_app100']),
            'per_app100');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },

      responseKeys: [ 'dataUpdate', 'per' ],

      validation: function(dataResponse, result) {
        if (Assert.assertDataResponseHadError(dataResponse, result, null,
            true, Result.severity.INFO)) {
          var updateResult = dataResponse.get('dataUpdate_app100');
          Helper.addSubResult(result, 'dataUpdate OWNER hadError()',
              Assert.assertTrue, updateResult.hadError(), 'true');
          Helper.addSubResult(result, 'dataUpdate OWNER getErrorMessage()',
              Assert.assertStringContains, updateResult.getErrorMessage(),
              'unsupported id: OWNER');
        } else {
          var responseItem = dataResponse.get('per_app100');
          Assert.assertDataResponseItemHadError('per_app100',
              responseItem, result, false);
          Helper.addSubResult(result, 'Response', true,
              gadgets.json.stringify(responseItem.getData()), 'Not empty');
        }
      }
    },


    { name: 'newFetchPersonAppDataRequest - fetch with non-existing key',
      id: 'APP102',
      description: 'Tests if data can be fetched for an non-existing key. ' +
                   'Checks that there is no data for the non-existing key' +
                   'in the response.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var randomKey = 'randomKey ' + new Date().getTime();
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({userId: 'VIEWER'}), randomKey),
            'randomFetch');
        req.send(Helper.createValidationCallback(this, callback, context,
            result, randomKey));
      },
      responseKeys: ['randomFetch'],

      validation: function(dataResponse, result, context, callBack, randomKey) {
        Assert.assertDataResponseHadError(dataResponse, result, null,
            false, Result.severity.INFO);
        var data = dataResponse.get('randomFetch').getData();
        for (var key in data) {
          result.addSubResult('Empty map', Assert.assertEmptyMap,
              data[key], 'Empty Map');
        }
      }
    },

    { name: 'newUpdatePersonAppDataRequest - update/fetch with invalid key',
      id: 'APP103',
      description: 'Tests if the data updated with an invalid key can be ' +
                   'fetched back with the same invalid key.Checks that ' +
                   'the date response throws an error.',
      run: function(context, callback, result) {
        var data = 'personalValue ' + new Date().getTime();
        var req = opensocial.newDataRequest();
        var viewer = context.getViewer();
        var key = '(invalidKey)';
        req.add(req.newUpdatePersonAppDataRequest(
            'VIEWER', key, data), 'dataUpdate');
        req.add(req.newFetchPersonAppDataRequest(opensocial.newIdSpec(
            {userId: viewer['id']}), key), 'fetchWithSingleId');
        req.send(Helper.createValidationCallback(
            this, callback, context, result));
      },
      responseKeys: ['dataUpdate'],

      validation: function(dataResponse, result) {
        if(Assert.assertDataResponseHadError(dataResponse, result, null,
            true)) {
          var keys = this.responseKeys;
          for (var i = 0; i < keys.length; i++) {
            var respItem = dataResponse.get(keys[i]);
            Assert.assertDataResponseItemHadError(keys[i], respItem, result,
                true);
          }
        }
      }
    },


    { name: 'Error when invalid key is attempted to be set, accepts only' +
            ' (A-Za-z0-9)(_)(.)(-)',
      id: 'APP105',
      description: 'Tests if the data updated with an invalid key (if the key ' +
                   'contains special symbols other than these - ' +
                   '\'(A-Za-z0-9)(_)(.)(-)\')',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var invalidKey = 'Key' + '[{}]';
        req.add(req.newUpdatePersonAppDataRequest('VIEWER',
            invalidKey, 'ValueOf=' + invalidKey), 'valueSet');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },

      validation: function(dataResponse, result) {
        if(Assert.assertDataResponseHadError(dataResponse, result, null,
            true)) {
          var item = dataResponse.get('valueSet');
          Helper.addSubResult(result, 'Verify Response Item is defined',
              Assert.assertDefined, item, 'is defined');
        }
      }
    },


    /**
     * Person App Data - Batch Requests
     */
    { name: 'Batch Requests - update and fetch',
      id: 'APP300',
      description: 'Tests if a batch request with success for update for' +
                   ' VIEWER and fetch for VIEWER Friends ' +
                   ' {userId : \'VIEWER\', groupId : \'FRIENDS\', ' +
                   'networkDistance : 1}',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var data = 'personalValue ' + new Date().getTime();
        req.add(req.newUpdatePersonAppDataRequest(
            'VIEWER', this.id + '_dataKey1',
            data), 'validUpdate');
        req.add(req.newFetchPersonAppDataRequest(opensocial.newIdSpec(
            {userId : 'VIEWER', groupId : 'FRIENDS', networkDistance : 1}),
            [this.id + '_dataKey1']), 'validFetch');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },

      responseKeys: ['validUpdate', 'validFetch'],

      validation: function(dataResponse, result, context) {
        if(Assert.assertDataResponseHadError(dataResponse, result, null,
            false)) {
          if (context.isViewerWithoutApp()) {
            // valid update should result in error as well
            Assert.assertDataResponseItemHadError('validUpdate',
                dataResponse.get('validUpdate'), result, true,
                opensocial.ResponseItem.Error.UNAUTHORIZED);
            Assert.assertDataResponseItemHadError('validFetch',
                dataResponse.get('validFetch'), result, true,
                opensocial.ResponseItem.Error.UNAUTHORIZED);
          } else {
            Helper.addSubResult(result, 'validUpdate.hadError()',
              Assert.assertFalse, dataResponse.get('validUpdate').hadError(),
              'hadError() should be false');
            Assert.assertDataResponseItemHadError('validFetch',
                dataResponse.get('validFetch'), result, false);
          }
        }
      }
    },


    { name: 'Batch Requests - without errors',
      id: 'APP301',
      description: 'Tests if a batch request can be sent with valid ' +
                   'scenarios and verifies the fetched data.',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        var data = 'personalValue ' + new Date().getTime();
        //update info for 2 keys for same user
        req.add(req.newUpdatePersonAppDataRequest(
            'VIEWER', 'KeyBatch1_app301', data),
            'ignored');
        req.add(req.newUpdatePersonAppDataRequest(
            'VIEWER', 'KeyBatch2_app301', data),
            'ignored');
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({ userId : 'VIEWER' }),
            ['KeyBatch1_app301']), 'viewer_app301');
        req.add(req.newFetchPersonAppDataRequest(
            opensocial.newIdSpec({ userId : 'VIEWER' }),
            ['KeyBatch2_app301']), 'viewer2_app301');

        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },


      validation: function(dataResponse, result, context) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
            , context, ['viewer_app301', 'viewer2_app301'])) {
          var responseForViewer = (dataResponse.get('viewer_app301')) ?
              dataResponse.get('viewer_app301').getData() : undefined;
          Helper.addSubResult(result, 'dataResponse.get(KeyBatch1_app301)',
              Assert.assertDefined, responseForViewer, 'defined');
          var responseForViewer2 = (dataResponse.get('viewer2_app301')) ?
              dataResponse.get('viewer2_app301').getData() : undefined;
          Helper.addSubResult(result, 'dataResponse.get(KeyBatch2_app302)',
              Assert.assertDefined, responseForViewer2, 'defined');
        }
      }
    },

    { name: 'Batch Requests - mixed person and appdata requests '
          + '(viewer == owner)',
      id: 'APP303',
      description: 'Tests if a batch request can be updated by the owner to' +
                   'the viewer\'s data. Checks if OWNER can check for the ' +
                   'updated information.',
      run: function(context, callback, result) {
        if (context.getViewer().isOwner()) {
          var req = opensocial.newDataRequest();
          req.add(req.newUpdatePersonAppDataRequest(
              'VIEWER', 'AppData303',
              'AppDataValue'), 'ignored');
          req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
              {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1})),
              'ownerFriends');
          req.add(req.newFetchPersonAppDataRequest(
              opensocial.newIdSpec({ userId : 'OWNER' }), 'AppData303'),
              'owner_data');
          req.send(Helper.createValidationCallback(this, callback, context,
              result));
        } else {
          result.addSubResult('Skipped', true,
              'Not executed because owner == viewer.', true);
          callback(result);
        }
      },
      responseKeys: ['owner_data', 'ownerFriends'],

      validation: function(dataResponse, result, context) {
        var keys = ['owner_data', 'ownerFriends'];

        for (var i = 0; i < keys.length; i++) {
          var respItem = dataResponse.get(keys[i]);
          Assert.assertDataResponseItemHadError(keys[i], respItem, result,
              false);
        }
      }
    },


    { name: 'Batch Requests - mixed person and appdata requests'
        + '(viewer != owner)',
      id: 'APP304',
      run: function(context, callback, result) {
        if (!context.getViewer().isOwner()) {
          var req = opensocial.newDataRequest();
          req.add(req.newUpdatePersonAppDataRequest(
              'VIEWER', 'AppData304', 'AppDataValue'), 'someData');
          req.add(req.newFetchPersonAppDataRequest(opensocial.newIdSpec(
              { userId : 'OWNER' }), 'AppData304'), 'owner_data');
          req.add(req.newFetchPeopleRequest(opensocial.newIdSpec(
              {userId : 'OWNER', groupId : 'FRIENDS', networkDistance : 1}),
              'ownerFriends'));
          req.send(Helper.createValidationCallback(this, callback, context,
              result));
        } else {
          Helper.logIntoResult(result,
              'Not executed because owner != viewer.', true, true);
          callback(result);
        }
      },

      validation: function(dataResponse, result, context) {
        if(Assert.assertDataResponseHadError(dataResponse, result, null,
            true)) {
          var keys = ['ownerFriends'];

          for (var i = 0; i < keys.length; i++) {
            var respItem = dataResponse.get(keys[i]);
            Assert.assertDataResponseItemHadError(keys[i], respItem, result,
                false);
          }
        }
      }
    },

    /**
     * Person App Data - ResponseItem
     */
    { name: 'ResponseItem - getOriginalDataRequest()',
      id: 'APP400',
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonAppDataRequest(opensocial.newIdSpec(
            { userId : 'OWNER' }), ['ownerKey']), 'owner');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },

      validation: function(dataResponse, result, context, callback){
        var prevReq = dataResponse.get('owner').getOriginalDataRequest();
        Helper.addSubResult(result, 'getOriginalDataRequest() is defined',
            Assert.assertDefined, prevReq, 'defined');
        var firstOwnerDataJson =
            JSON.stringify(dataResponse.get('owner').getData());
        Helper.logIntoResult(result, 'first Data:', firstOwnerDataJson);
        var req = new opensocial.newDataRequest();
        req.add(prevReq, 'owner');
        req.send(function(dataResponse) {
          if (dataResponse.hadError()) {
            Assert.assertDataResponseHadError(dataResponse, result,
                ['owner'], false);
            Assert.assertDataResponseItemHadError('second Data',
                dataResponse.get('owner'), result, false);
          } else {
            var secondOwnerDataJson =
                JSON.stringify(dataResponse.get('owner').getData());
            Helper.logIntoResult(result, 'second Data:', secondOwnerDataJson);
            Helper.addSubResult(result, 'Data equal', Assert.assertTrue,
                firstOwnerDataJson == secondOwnerDataJson, 'data is equal');
          }
          callback(result);
        });
      }
    },

    /**
     * Person App Data - update
     */
    { name: 'Write Test: newUpdatePersonAppDataRequest('
        + 'opensocial.newIdSpec({userId: \'VIEWER\'}, \'viewerKey_app800\')',
      id: 'APP800',
      description: 'Creates an item to request app data for VIEWER. ' +
                    'and checks if the value is updated.',
      queue: 6,
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newUpdatePersonAppDataRequest('VIEWER', 'viewerKey_app800',
            'viewerValue ' + new Date().getTime()), 'viewerKeySet');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys:['viewerKeySet'],
      validation: function(dataResponse, result, context, callback) { }
    },

   /**
     * Person App Data - read
     */
    { name: 'Read Test: newFetchPersonAppDataRequest('
         + 'opensocial.newIdSpec({userId: \'VIEWER\'}, \'viewerKey\')',
      id: 'APP900',
      description: 'Creates an item to request app data for VIEWER. ' +
                   'Validates the returned data, if it contains the value ',
      queue: 7,
      run: function(context, callback, result) {
        var req = opensocial.newDataRequest();
        req.add(req.newFetchPersonAppDataRequest(opensocial.newIdSpec(
            { userId : 'VIEWER' }), 'viewerKey_app800'), 'viewerKeyGet');
        req.send(Helper.createValidationCallback(this, callback, context,
            result));
      },
      responseKeys:['viewerKeyGet'],
      validation: function(dataResponse, result, context, callback) {
        if (Assert.assertDataResponseShouldContinue(dataResponse, result
                    , context, ['viewerKeyGet'])) {
          var responseItem = dataResponse.get('viewerKeyGet');
          var value = responseItem.getData() ?
              responseItem.getData()[
                  context.getViewer().getId()]['viewerKey_app800'] : '';
           Helper.addSubResult(result,'Test value',
              Assert.assertStringContains, value, 'viewerValue');
        }
      }
    }
  ]
};
