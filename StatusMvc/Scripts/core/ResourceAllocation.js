/// <reference path="api/jquery-1.7.1.min.js" />
/// <reference path="api/jquery.validate.min.js" />
/// <reference path="api/knockout-2.0.0.js" />
/// <reference path="api/jquery-ui-1.8.16.min.js" />
/// <reference path="api/jquery.autoresize.js" />
/// <reference path="api/date.js" />
/// <reference path="api/jquery.hotkeys.js" />
/// <reference path="api/tag-it.js" />
/// <reference path="../core/JqueryExt.js" />

var resourceAllocationVM = {
    AllocationTree: ko.observable(new allocationTree()),
    initJQuery: function() {
        
    },
    loadAllocationTree: function(startDate, endDate) {
        var url = "/ResourceAllocation/GetResourceAllocations?startDate=" + startDate + "&endDate=" + endDate;
        $.ajax({
            url: url,
            dataType: "json",
            converters: {
                "text json": function (data) {  return $.parseJSON(data, true); }
            },
            success: function(response) {
                if (response != null) {
                    var at = new allocationTree();
                    at.initData(response);
                    resourceAllocationVM.AllocationTree(at);
                    resourceAllocationVM.initJQuery();
                } else {
                    alert(response.message);
                }
            },
            error: function (response) {
                alert("Failed to get allocations for date range " + startDate + " to " + endDate);
            }
        });
    }
};

function allocationTree() {
    var self = this;

    var initData = function (response) {
    };
}