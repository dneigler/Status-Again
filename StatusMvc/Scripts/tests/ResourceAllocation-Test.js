/// <reference path="api/jquery-1.7.1.min.js" />
/// <reference path="api/jquery.validate.min.js" />
/// <reference path="api/knockout-2.0.0.js" />
/// <reference path="api/jquery-ui-1.8.16.min.js" />
/// <reference path="api/jquery.autoresize.js" />
/// <reference path="api/date.js" />
/// <reference path="api/jquery.hotkeys.js" />
/// <reference path="api/tag-it.js" />
/// <reference path="../core/JqueryExt.js" />
/// <reference path="../core/ResourceAllocation.js" />

ResourceAllocationTest = TestCase("ResourceAllocationTest");

ResourceAllocationTest.prototype.testGreet = function () {
    var sr = new allocationTree();
    assertEquals(0, sr.Tags().length);
};
