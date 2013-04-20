// Copyright 2008 Google Inc. All Rights Reserved.
/**
 * @fileoverview Container specific environment capability tests.<br>
 * These tests depends on framework
 * http://opensocial-resources.googlecode.com/svn/tests/trunk/common/
 *
 * Note on test cases numbers:
 * 000 to 499 are positive cases
 * 500 to 999 are negative cases that present themselves due to a mis-use
 * of the API.
 * And thus they are ordered that way.
 */
function EnvironmentSuite() {
  this.name = 'Environment Test Suite';
  this.id = 'ENV';
  this.tests = [

    { name: 'Environment and Views',
      id : 'ENV001',
      priority: Test.PRIORITY.P0,
      description: 'Test environment access and equality.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentEquals(result, 'Environment'
            , opensocial.getEnvironment(), context.getEnvironment());
        callback(result);
      }
    },

    { name: 'Environment.supportsField(PERSON)',
      id: 'ENV002',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.PERSON and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.PERSON,
            context.getData().personFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(ACTIVITY)',
      id: 'ENV004',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.ACTIVITY and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.ACTIVITY,
            context.getData().activityFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(ACTIVITY_MEDIA_ITEM)',
      id: 'ENV005',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.ACTIVITY_MEDIA_ITEM ' +
                   'and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.ACTIVITY_MEDIA_ITEM,
            context.getData().mediaItemFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(BODY_TYPE)',
      id: 'ENV006',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.BODY_TYPE ' +
                   'and its fields.',
      tags: ['notImplementedYet'],
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.BODY_TYPE,
            ['build', 'eyeColor', 'hairColor', 'height', 'weight'], result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(EMAIL)',
      id: 'ENV007',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.EMAIL and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.EMAIL,
            context.getData().emailFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(ADDRESS)',
      id: 'ENV009',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.ADDRESS and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.ADDRESS,
            context.getData().addressFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(FILTER_TYPE)',
      id: 'ENV010',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.FILTER_TYPE ' +
                   'and its fields.',
      tags: ['notImplementedYet'],
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.FILTER_TYPE, [''], result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(MESSAGE_TYPE)',
      id: 'ENV011',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.MESSAGE_TYPE ' +
                   'and its fields.',
      tags: ['notImplementedYet'],
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.MESSAGE_TYPE,
            ['email', 'notification', 'privateMessage', 'publicMessage'],
            result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(NAME)',
      id: 'ENV012',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.NAME and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.NAME,
            context.getData().nameFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(ORGANIZATION)',
      id: 'ENV013',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.ORGANIZATION ' +
                   'and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.ORGANIZATION,
            context.getData().organizationFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(PHONE)',
      id: 'ENV014',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.PHONE and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.PHONE,
            context.getData().phoneFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(SORT_ORDER)',
      id: 'ENV015',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   'opensocial.Environment.ObjectType.SORT_ORDER ' +
                   'and its fields.',
      tags: ['notImplementedYet'],
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.SORT_ORDER, [''], result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(URL)',
      id: 'ENV016',
      priority: Test.PRIORITY.P0,
      description: 'Test if environment supports ' +
                   ' opensocial.Environment.ObjectType.URL and its fields.',
      run: function(context, callback, result) {
        Assert.assertEnvironmentSupportedFieldsEquals(
            opensocial.getEnvironment(),
            opensocial.Environment.ObjectType.URL,
            context.getData().urlFields, result);
        callback(result);
      }
    },

    { name: 'Environment.supportsField(PERSON) - Empty fields',
      id: 'ENV502 (ENV007)',
      description: 'ERROR HANDLING CASE: Enviroment.hasCapability() without ' +
                   'parameters. Checks how the framework handles a malformed ' +
                   'invoke on a method ideally it should return false or ' +
                   'avoid exceptions by returning null or undefined.',
      run: function(context, callback, result) {
        var env = opensocial.getEnvironment();
        Helper.addSubResult(result, 'supportsField(PERSON) - No field',
            Assert.assertFalse,
            env.supportsField(opensocial.Environment.ObjectType.PERSON), false);
        Helper.addSubResult(result, 'supportsField(PERSON) - null field',
            Assert.assertFalse,
            env.supportsField(opensocial.Environment.ObjectType.PERSON, null),
            false);
        Helper.addSubResult(result, 'supportsField(PERSON) - undefined field',
            Assert.assertFalse,
            env.supportsField(
                opensocial.Environment.ObjectType.PERSON, undefined),  
            false);
        callback(result);
      }
    }
  ]
};

