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
 * @fileoverview gadgets.io.* API tests.
 */
function IoSuite() {
  this.name = 'gadgets.io.* TestSuite';
  this.id = 'IO';
  this.tests = [
   { name: 'gadgets.io.encodeValues({...}) - With a simple map.',
      id: 'GIO000',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can encode map to an URL-encoded' +
                  ' data string',
      run: function(context, callback, result) {
        var encoded = gadgets.io.encodeValues({key1: 'value1', key2: 'value2'});
        var expected = 'key1=value1&key2=value2';
        result.setResult(Assert.assertEquals(encoded, expected), encoded
            , expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues({...}) - With reserved characters.',
      id: 'GIO001',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can encode \'$&+,/:;=?@\' - reserved ' +
                   'characters to URL-encoded data string',
      run: function(context, callback, result) {
        var data = '$&+,/:;=?@';
        var encoded = gadgets.io.encodeValues(data);
        var expected =
            '0=%24&1=%26&2=%2B&3=%2C&4=%2F&5=%3A&6=%3B&7=%3D&8=%3F&9=%40';
        result.setResult(Assert.assertEquals(encoded, expected), encoded
            , expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues({...}) - With unsafe characters.',
      id: 'GIO002',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can encode \'" <>#%{}|\\^[]`\' - unsafe ' +
                   'characters to URL-encoded data string',
      bugs: ['1044115'],
      run: function(context, callback, result) {
        var data = '" <>#%{}|\\^[]`';
        var encoded = gadgets.io.encodeValues(data);
        var expected =
            '0=%22&1=%20&2=%3C&3=%3E&4=%23&5=%25&6=%7B&7=%7D&8=%7C&9=%5C&'
            + '10=%5E&11=%5B&12=%5D&13=%60';
        result.setResult(Assert.assertEquals(encoded, expected), encoded
            , expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues(String) - With a String object.',
      id: 'GIO011',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can encode a string object to ' +
                   'URL-encoded data string',
      run: function(context, callback, result) {
        var theString = 'Value';
        var encoded = gadgets.io.encodeValues(theString);
        var expected = '0=V&1=a&2=l&3=u&4=e';
        result.setResult(Assert.assertEquals(encoded, expected), encoded
            , expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues() - With no parameters.',
      id: 'GIO050',
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can call encodeValues() without passing any ' +
                   'parameter and it returns empty string.',
      run: function(context, callback, result) {
        var encoded = gadgets.io.encodeValues();
        result.setResult(Assert.assertEquals(encoded, ''), encoded, '');
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues(null) - With null parameter.',
      id: 'GIO051',
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can pass \'null\' as parameter and it returns ' +
                   'empty string.',
      run: function(context, callback, result) {
        var encoded = gadgets.io.encodeValues(null);
        result.setResult(Assert.assertEquals(encoded, ''), encoded, '');
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues(undefined) - With undefined parameter.',
      id: 'GIO052',
      priority: Test.PRIORITY.P2,
      description: 'Tests if we can pass \'undefined\' as parameter and ' +
                   'api returns empty string.',
      run: function(context, callback, result) {
        var encoded = gadgets.io.encodeValues(undefined);
        result.setResult(Assert.assertEquals(encoded, ''), encoded, '');
        callback(result);
      }
    },

    { name: 'gadgets.io.encodeValues({...}) - Map with I18N values.',
      id: 'GIO053',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can encode an \'\u00E1\u00E9\u00ED\u00F3\'' +
                   'i18n value to URL-encoded data string',
      run: function(context, callback, result) {
        var value = '\u00E1\u00E9\u00ED\u00F3';
        var encoded = gadgets.io.encodeValues({key: value});
        var expected = 'key=%C3%A1%C3%A9%C3%AD%C3%B3';
        result.setResult(Assert.assertEquals(encoded, expected), encoded
            , expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.getProxyUrl(String) - With a String object.',
      id: 'GIO100',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can get the proxy URL with given string ' +
                   'as proxy',
      run: function(context, callback, result) {
        var theString = 'Value';
        var url = gadgets.io.getProxyUrl(theString);
        var expected = 'url=Value';
        var proxy = 'proxy?';
        result.addSubResult(this.name,
            Assert.assertStringContains(url, proxy) ||
            Assert.assertStringEndsWith(url, theString), url, expected);
        callback(result);
      }
    },

    { name: 'gadgets.io.getProxyUrl(String) - With valid URL.',
      id: 'GIO101',
      priority: Test.PRIORITY.P0,
      description: 'Tests if we can get the proxy URL with given URL ' +
                   'as proxy',
      bugs: ['1044115'],
      run: function(context, callback, result) {
        var theString = 'http://www/~user';
        var url = gadgets.io.getProxyUrl(theString);
        var expected = 'url=http%3A%2F%2Fwww%2F~user';
        var proxy = 'proxy?';
        result.addSubResult(this.name,
            Assert.assertStringContains(url, proxy) ||
            Assert.assertStringEndsWith(url, theString), url, expected);
        callback(result);
      }
    }
  ]
};
