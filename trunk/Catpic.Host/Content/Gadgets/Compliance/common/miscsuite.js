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
 * @fileoverview Miscellaneous features
 * 1) opensocial.requestSendMessage:
 *
 * 2) opensocial.Collection
 * - 2.1 newFetchPeopleRequest
 */
function MiscSuite0_8() {
  this.name = 'Miscellaneous';
  this.id = 'MISC';
  this.tests = [

    /************************************
    * 1) opensocial.requestSendMessage:
    ************************************/
    { name: 'Message Object Creation',
      id: 'MISC000',
      priority: Test.PRIORITY.P0,
      run: function(context, callback, result) {
        var today = new Date();

        var body = 'This is a message sent [' + today + '] to Owner';
        var title = 'Test requestSendMessage (Owner)';
        var params = {}
        params[opensocial.Message.Field.BODY] = body;
        params[opensocial.Message.Field.TITLE] = title ;
        params[opensocial.Message.Field.TYPE] =
            opensocial.Message.Type.PUBLIC_MESSAGE;
        var message = opensocial.newMessage(body, params);

        Helper.addSubResult(result, 'Message Creation', Assert.assertDefined,
            message, 'Message Defined');

        var messageValue = message.getField(opensocial.Message.Field.TITLE);
        Helper.addSubResult(result, 'Message.Title', Assert.assertEquals,
            messageValue, title);

        messageValue = message.getField(opensocial.Message.Field.BODY);
        Helper.addSubResult(result, 'Message.Body', Assert.assertEquals,
            messageValue, body);

        messageValue = message.getField(opensocial.Message.Field.TYPE);
        Helper.addSubResult(result, 'Message.Type', Assert.assertEquals,
            messageValue, opensocial.Message.Type.PUBLIC_MESSAGE)

        callback(result);
      }
    },


    /******************************************************
    * 2 newFetchPeopleRequest - Misc - Collections
    *******************************************************/
    { name: 'Collection - Collection.each(Person)',
      id: 'MSC300',

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
      id: 'MSC301',
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
            var outcome;
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
              outcome = undefined;
              Helper.addSubResult(result, 'size() & totalSize()',
                  Assert.assertTrue, true, 'Total:' + actual.getTotalSize()
                  + ' & ' + 'Size:'+ actual.size());
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
      id: 'MSC302',
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
      id: 'MSC303',
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


MiscSuite0_8.getFirstFriendId = function(){
  var req = opensocial.newDataRequest();

  req.add(req.newFetchPeopleRequest(opensocial.DataRequest.Group.OWNER_FRIENDS),
      'ownerFriends');
  req.send(function(dataResponse) {
      if (!dataResponse.hadError()) {
        var ownerFriends = dataResponse.get('ownerFriends').getData().asArray();
        return ownerFriends[0].getField(opensocial.Person.Field.ID);
      }
      else {
        return null;
      }
  });
}
