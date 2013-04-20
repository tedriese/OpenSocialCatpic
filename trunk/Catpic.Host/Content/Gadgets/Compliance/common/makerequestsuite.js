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
 * @fileoverview MakeRequest tests.
 */
function MakeRequestSuite() {
  this.name = 'MakeRequest Test Suite';
  this.id = 'MKR';
  this.tests = [

    { name: 'makeRequest() - html, default',
      id : 'MKRT001',
      description: 'Tests if the makeRequest() can fetch an html file from ' +
                   'remote content .',
      run: function(context, callback, result) {
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/htmlsample.html',
            function(dataObject) {
          var expected = '<html><body>sample</body></html>';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.TEXT,
              result, expected);
          callback(result);
        });
      }
    },


    { name: 'makeRequest() - xml, default',
      id : 'MKRT002',
      description: 'Tests if the makeRequest() can fetch an xml file from ' +
                   'remote content and check the data content.',
      run: function(context, callback, result) {
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/xmlsample.xml',
            function(dataObject) {
          var expected = '<xml><tag>xmlsample</tag></xml>';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.TEXT,
              result, expected);
          callback(result);
        }, {});
      }
    },


    //  makeRequest for html file with ContentType DOM
    //  doesn't return a DOM object
    //  bugid : 1017755 

    { name: 'makeRequest() - xml - DOM contentType',
      id : 'MKRT014',
      description: 'Tests if the makeRequest() can fetch an html file with ' +
                   'DOM as content type and check if the returned data' +
                   'content is DOM.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.DOM;
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/xmlsample.xml',
            function(dataObject) {
          var textExpected = '<xml><tag>xmlsample</tag></xml>';
          var textContentExpected = 'xmlsample';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.DOM,
              result, textExpected, textContentExpected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - xml - DOM contentType with ?xml tag',
      id : 'MKRT018',
      description: 'Tests if makeRequest() can fetch an xml file with ' +
                   'content type as DOM. Also checks for the returned' +
                   'content type to be DOM',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.DOM;
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/xmlsample2.xml',
            function(dataObject) {
          var textExpected = '<?xml version="1.0" encoding="UTF-8"?>\n' +
              '<person>\n  <name>John</name>\n</person>';
          var textContentExpected = 'John';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.DOM,
              result, textExpected, textContentExpected);
          callback(result);
        }, params);
      }
    },

    { name: 'makeRequest() - txt - JSON contentType',
      id : 'MKRT015',
      description: 'Tests if makeRequest() can fetch an txt file with ' +
                   'content type as JSON. Also checks for the returned data' +
                   'content type to be JSON',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
             gadgets.io.ContentType.JSON;
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/jsonexample.txt',
            function(dataObject) {
          var jsonObject = {'widget':{'debug':'on',
              'window':{'title':'Sample Konfabulator Widget','height':500},
              'image':{'src':'Images/Sun.png','alignment':'center'}}};
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.JSON,
              result, gadgets.json.stringify(jsonObject));
          callback(result);
        }, params);
      }
    },

      
    { name: 'makeRequest() - feed, default Atom',
      id : 'MKRT003',
      description: 'Tests if makeRequest() can fetch a feed with format ' +
                   'as atom. Also checks for the returned data content ' +
                   'type to be FEED',
      queue: 7,
      bugs: ['1015442'],
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.FEED;
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/atomfeedsample.xml',
            function(dataObject) {
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.FEED,
              result);
//          var subtest = new ResultValidation('makeRequest math with title');
//          var expected = 'Example Feed';
//          subtest.setResult(
//              Assert.assertEquals(data.text.match(expected), expected),
//                  data.text.match(expected), expected);
//          result.add(subtest);
//
//          var subtest2 = new ResultValidation('makeRequest match with row3');
//          var expected2 = 'Row3';
//          subtest2.setResult(
//              Assert.assertEquals(data.text.match(expected2), expected2),
//                  data.text.match(expected2), expected2);
//          result.add(subtest2);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - feed, default RSS',
      id : 'MKRT012',
      description: 'Tests if makeRequest() can fetch a feed with rss format. ' +
                   'Also checks for the returned data content type to be FEED',
      bugs: ['1015442'],
      queue: 8,
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.FEED;
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/rssfeedsample.xml',
            function(dataObject) {
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.FEED,
              result);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - html, POST with postData',
      id : 'MKRT004',
      description: 'Tests if makeRequest() can fetch an html file by using ' +
                   'POST method with some data to be posted. Checks for the ' +
                   'returned data content type to be text file.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.TEXT;
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;
        params[gadgets.io.RequestParameters.POST_DATA] = 'value1';
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/htmlsample.html',
            function(data) {
          var expected = '<html><body>sample</body></html>';
          Assert.assertDataContent(data, gadgets.io.ContentType.TEXT, result,
              expected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - feed, POST with postData, entries, summaries',
      id : 'MKRT005',
      description: 'Tests if makeRequest() can fetch a feed using POST method' +
                   'with some data to be posted and to fetch data with ' +
                   'number of entries and summary. Checks for the returned ' +
                   'data content type to be a FEED.',
      bugs: ['1015442'],
      queue: 9,
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.FEED;
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;
        params[gadgets.io.RequestParameters.NUM_ENTRIES] = 5;
        params[gadgets.io.RequestParameters.GET_SUMMARIES] = true;
        params[gadgets.io.RequestParameters.POST_DATA] = 'value1';
 
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/atomfeedsample.xml',
            function(data) {
          Assert.assertDataContent(data, gadgets.io.ContentType.FEED, result);

//          var subtest = new ResultValidation('matching title');
//          var expected = 'Example Feed';
//          subtest.setResult(
//              Assert.assertEquals(data.text.match(expected), expected),
//                  data.text.match(expected), expected);
//          result.add(subtest);
//
//          var subtest2 = new ResultValidation('match with entry 3');
//          var expected2 = 'Row4';
//          subtest2.setResult(
//              Assert.assertEquals(data.text.match(expected2), expected2),
//                  data.text.match(expected2), expected2);
//          result.add(subtest2);
//
//          var subtest3 = new ResultValidation('match with summary');
//          var expected3 = 'Summary4';
//          subtest3.setResult(
//              Assert.assertEquals(data.text.match(expected3), expected3),
//                  data.text.match(expected3), expected3);
//          result.add(subtest3);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - xml, POST with postData',
      id : 'MKRT006',
      description: 'Tests if makeRequest() can fetch an xml file using POST ' +
                   'method with some data to be posted.Checks for the ' +
                   'returned data content type to be a DOM.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.DOM;
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;
        params[gadgets.io.RequestParameters.POST_DATA] = 'value1';
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/xmlsample.xml',
            function(dataObject) {
          var textExpected = '<xml><tag>xmlsample</tag></xml>';
          var textContentExpected = 'xmlsample';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.DOM,
              result, textExpected, textContentExpected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - xml, POST with no postData)',
      id : 'MKRT007',
      description: 'Tests if makeRequest() can fetch an xml file using POST ' +
                   'method without any data to be posted.Checks for the ' +
                   'returned data content type to be a DOM.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.DOM;
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;

        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/xmlsample.xml',
            function(dataObject) {
          var textExpected = '<xml><tag>xmlsample</tag></xml>';
          var textContentExpected = 'xmlsample';
          Assert.assertDataContent(dataObject, gadgets.io.ContentType.DOM,
              result, textExpected, textContentExpected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - html, POST with no postData',
      id : 'MKRT008',
      description: 'Tests if makeRequest() can fetch an html file using POST ' +
                   'method with some data to be posted. Checks for the ' +
                   'returned data content type to be a TEXT.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/htmlsample.html',
            function(data) {
          var expected = '<html><body>sample</body></html>';
          Assert.assertDataContent(data, gadgets.io.ContentType.TEXT,
              result, expected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - txt file with html extension, GET, default',
      id : 'MKRT009',
      description: 'Tests if makeRequest() can fetch a txt file using GET ' +
                   'method. Checks for the returned data content type ' +
                   'to be a TEXT.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.GET;
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/txtsample.html',
            function(data) {
          var expected = 'text';
          Assert.assertDataContent(data, gadgets.io.ContentType.TEXT,
              result, expected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - txt file with csv extension, GET, default',
      id : 'MKRT010',
      description: 'Tests if makeRequest() can fetch a txt file with csv extn' +
                   'and using GET method. Checks for the returned data ' +
                   'content type to be a TEXT.',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.GET;
        gadgets.io.makeRequest(
            Config.contentPath + 'suites/0.7/makerequest/content/txtsample.csv',
            function(data) {
          var expected = 'text';
          Assert.assertDataContent(data, gadgets.io.ContentType.TEXT,
              result, expected);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - feed, no Postdata, 4 entries & no summaries',
      id : 'MKRT011',
      description: 'Tests if makeRequest() can fetch a feed using POST ' +
                   'without any post data and with 4 entries and without ' +
                   'the summary. Checks for the returned data ' +
                   'content type to be a FEED.',
      bugs: ['1015442'],
      queue: 10,
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.FEED;
        params[gadgets.io.RequestParameters.METHOD] =
            gadgets.io.MethodType.POST;
        params[gadgets.io.RequestParameters.GET_SUMMARIES] = false;
        params[gadgets.io.RequestParameters.NUM_ENTRIES] = 4;

        gadgets.io.makeRequest(Config.contentPath + 'suites/0.7/makerequest/' +
            'content/atomfeedsample.xml',
            function(data) {
           Assert.assertDataContent(data, gadgets.io.ContentType.FEED,
              result);

//          var subtest1 = new ResultValidation('Summary must not exist');
//          var expected1 = 'Summary1';
//          subtest1.setResult(
//              Assert.assertNotEquals(data.text.match(expected1), expected1),
//                  data.text.match(expected1), 'Summary1 should not exist');
//          result.add(subtest1);
//
//          var subtest2 = new ResultValidation('entry 5 must not exist');
//          var expected2 = 'Row5';
//          subtest2.setResult(
//              Assert.assertNotEquals(data.text.match(expected2), expected2),
//                  data.text.match(expected2), 'Row5 should not exist');
//          result.add(subtest2);
          callback(result);
        }, params);
      }
    },


    { name: 'makeRequest() - html, non-existent URL should return 404 error',
      id : 'MKRT013',
      description: 'Tests if makeRequest() can fetch an html file with non ' +
                   'existing URL. Checks if the method returns 404 error.',
      bugs:['1008132'],
      run: function(context, callback, result) {
        try {
            gadgets.io.makeRequest(
                Config.contentPath + 'suites/0.7/makerequest/content/nonexistent.html',
                function(dataOutput) {
                  Helper.addSubResult(result, 'Data Response: ', Assert.assertTrue,
                      dataOutput, 'Not null');
                callback(result);
              }, {});
        } catch (ex) {
          Helper.addSubResult(
                  result, 'Exception occured: ', Assert.fail, ex.toString(), 'No exception');
          callback(result);
        }
      }
    },

    //  When Passing invalid content types such as html the xml will
    //  simply be ignored
    //  bugid : 1015480

    { name: 'makeRequest() - php, SIGNED',
      id : 'SMKRT001',
      description: 'Tests if makeRequest() can fetch a php file with a ' +
                   'signed request. ',
      run: function(context, callback, result) {
        var params = {};
        params[gadgets.io.RequestParameters.AUTHORIZATION] =
            gadgets.io.AuthorizationType.SIGNED;
        params[gadgets.io.RequestParameters.CONTENT_TYPE] =
            gadgets.io.ContentType.JSON;
        gadgets.io.makeRequest(Config.contentPath +
            'suites/0.7/makerequest/content/signed.php',
            function(dataObject) {
          var data = dataObject.data;

          Helper.addSubResult(result, 'field: validated',
              Assert.assertEquals, data.validated, 'success');

          Helper.addSubResult(result, 'field: query.opensocial_owner_id',
              Assert.assertNotEmpty, data.query.opensocial_owner_id,
              'not empty');

          Helper.addSubResult(result, 'field: query.opensocial_app_url',
              Assert.assertNotEmpty, data.query.opensocial_app_url,
              'not empty');

          Helper.addUnsevereSubResult(result, 'field: query.opensocial_viewer_id',
              Assert.assertNotEmpty, data.query.opensocial_viewer_id,
              'not empty');

          Helper.addUnsevereSubResult(result, 'field: query.opensocial_app_id',
              Assert.assertNotEmpty, data.query.opensocial_app_id,
              'not empty');

          Helper.addUnsevereSubResult(result, 'field: query.oauth_consumer_key',
              Assert.assertNotEmpty, data.query.oauth_consumer_key,
              'not empty');

          Helper.addUnsevereSubResult(result, 'field: query.xoauth_signature_publickey',
              Assert.assertStringContains,
              data.query.xoauth_signature_publickey, '.cer');

          callback(result);
        }, params);
      }
    }
  ]
};
