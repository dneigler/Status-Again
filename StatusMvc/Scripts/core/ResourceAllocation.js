/// <reference path="../api/jquery-1.7.1.min.js" />
/// <reference path="../api/jquery.validate.min.js" />
/// <reference path="../api/knockout-2.0.0.js" />
/// <reference path="../api/jquery-ui-1.8.16.min.js" />
/// <reference path="../api/jquery.autoresize.js" />
/// <reference path="../api/date.js" />
/// <reference path="../api/jquery.hotkeys.js" />
/// <reference path="../api/tag-it.js" />
function extend(subClass, superClass) {
    var F = function () { };
    F.prototype = superClass.prototype;
    subClass.prototype = new F();
    subClass.prototype.constructor = subClass;

    subClass.superclass = superClass.prototype;
    if (superClass.prototype.constructor == Object.prototype.constructor) {
        superClass.prototype.constructor = superClass;
    }
};

function entityObject() {
    // this is required, otherwise the prototype for Id method will always refer
    // to the same instance.
    this.Id = ko.observable(0);
    this.HasInsertion = ko.observable(false);
    this.ChangeLog = ko.observableArray([]);
    this.HasDeletion = ko.observable(false);
    this.HasChanges =  ko.computed(function () {
        return this.ChangeLog().length > 0;
    } .bind(this));
    this.Editable = ko.computed(function () {
        return (this.HasDeletion() == false);
    }.bind(this));
};
entityObject.prototype.Id = ko.observable(0);

entityObject.prototype.Exists = function (collection, objId) {
    var item1 = ko.utils.arrayFilter(ko.utils.unwrapObservable(collection), function (item) {
        return item.Id() == objId;
    });
    return (item1.length > 0);
};

entityObject.prototype.isInternal = function (x, item) {
    return !(x != "ChangeLog" && x != "HasChanges" && x != "HasDeletion" && x != "HasInsertion" && x != "Editable" && ko.isObservable(item));
};

entityObject.prototype.HasInsertion = ko.observable(false);
entityObject.prototype.ChangeLog = ko.observableArray([]);
entityObject.prototype.HasDeletion = ko.observable(false);

entityObject.prototype.reset = function () {
    // statusReportItem version
    // need to undo the value damage though
    var self = this;
    $.each(self, function (x, item) {
        if (!self.isInternal(x, item)) {
            var currentlyChangeExists = ($.inArray(x, self.ChangeLog()) >= 0);
            if (currentlyChangeExists)
                item(self.OriginalVersion[x]);
            // self.Subscribers.push(sub);
        }

    });
    self.ChangeLog.removeAll();
    self.HasDeletion(false);
    self.HasInsertion(false);
};

entityObject.prototype.ListenForChanges = function () {
    var self = this;
    self.ClearSubscribers();
    $.each(self, function (x, item) {
        if (!self.isInternal(x, item)) {
            var sub = item.subscribe(function (newValue) {
                self._updateChangeTracking(x, newValue);

            });
            self.Subscribers.push(sub);
        }
    });
};

entityObject.prototype._updateChangeTracking = function (propertyName, newValue) {
    var self = this;
    var currentlyChangeExists = ($.inArray(propertyName, self.ChangeLog()) >= 0);
    var origValue = self.OriginalVersion[propertyName];
    if (newValue != origValue && !currentlyChangeExists) {
        console.log("The new value for " + propertyName + " is " + newValue + " from " + origValue);
        self.ChangeLog.push(propertyName);
    } else if (newValue == origValue && currentlyChangeExists) {
        self.ChangeLog.pop(propertyName);
    }
};

//entityObject.prototype.HasChanges = ko.computed(function () {
//    var self = this;
//    return self.ChangeLog().length > 0;
//});// .bind(this));


//entityObject.prototype.Editable = ko.computed(function () {
//    var self = this;
//    return (self.HasDeletion() == false);
//});// .bind(this));

function allocationTree(response) {
    allocationTree.superclass.constructor.call(this);
    this.Months = ko.observableArray([]);
    this.Teams = ko.observableArray([]);
    this.LoadFromObject(response);
};
extend(allocationTree, entityObject);

allocationTree.prototype.Months = ko.observableArray([]);
allocationTree.prototype.Teams = ko.observableArray([]);

allocationTree.prototype.LoadFromObject = function (response) {
    var self = this;
    // need to iterate through teams
    this.Months(response.Months);
    $.each(response.Teams, function (x, item) {
        var t = new team()
			.LoadFromObject(item);
        self.LoadTeam(t);
    });

    return this;
};

allocationTree.prototype.LoadTeam = function (obj) {
    var matchingItem = this.Exists(self.Teams, obj.Id());
    if (!matchingItem)
        this.Teams.push(obj);
};

function team() {
    team.superclass.constructor.call(this);
    this.Name = ko.observable('');
    this.LeadFullName = ko.observable('');
    this.LeadId = ko.observable(0);
    this.Members = ko.observableArray([]);
};
extend(team, entityObject);

team.prototype.Name = ko.observable('');
team.prototype.LeadFullName = ko.observable('');
team.prototype.LeadId = ko.observable(0);
team.prototype.Members = ko.observableArray([]);

team.prototype.LoadFromObject = function (obj) {
    var self = this;
    this.Id(obj.Id)
        .Name(obj.Name)
        .LeadFullName(obj.LeadFullName)
        .LeadId(obj.LeadId);
    $.each(obj.Members, function (x, item) {
        var m = new teamMember()
			.LoadFromObject(item);
        self.LoadMember(m);
    });

    return this;
};

team.prototype.LoadMember = function (obj) {
    var matchingItem = this.teamMemberExists(obj.Id());
    if (!matchingItem)
        this.Members.push(obj);
};

team.prototype.teamMemberExists = function (objId) {
    return this.Exists(this.Members(), objId);
}

function teamMember() {
    teamMember.superclass.constructor.call(this);
    this.FullName = ko.observable('');
    this.Projects = ko.observableArray([]);
};
extend(teamMember, entityObject);

teamMember.prototype.FullName = ko.observable('');
teamMember.prototype.Projects = ko.observableArray([]);

teamMember.prototype.LoadFromObject = function (obj) {
    var self = this;

    this.Id(obj.Id)
        .FullName(obj.FullName);

    $.each(obj.Projects, function (x, item) {
        var p = new project()
            .LoadFromObject(item);
        self.LoadProject(p);
    });

    return this;
};

teamMember.prototype.LoadProject = function (obj) {
    var matchingItem = this.ProjectExists(obj.Id());
    if (!matchingItem)
        this.Projects.push(obj);
};

teamMember.prototype.ProjectExists = function (objId) {
    return this.Exists(this.Projects(), objId);
};


function project() {
    project.superclass.constructor.call(this);
    this.Name = ko.observable('');
    this.Allocations = ko.observableArray([]);
};
extend(project, entityObject);

project.prototype.Name = ko.observable('');
project.prototype.Allocations = ko.observableArray([]);

project.prototype.LoadFromObject = function (obj) {
    var self = this;
    this.Id(obj.Id)
        .Name(obj.Name);

    $.each(obj.MonthlyAllocations, function (x, item) {
        var m = new monthlyAllocation()
				.LoadFromObject(item);

        self.LoadAllocation(m);

    });

    return this;
};

project.prototype.LoadAllocation = function (obj) {
    var matchingItem = this.AllocationExists(obj.Id());

    if (!matchingItem)
        this.Allocations.push(obj);
};

project.prototype.AllocationExists = function (objId) {
    return this.Exists(this.Allocations(), objId);
};

function monthlyAllocation() {
    monthlyAllocation.superclass.constructor.call(this);
    this.Month = ko.observable('');
    this.Allocation = ko.observable(0);
    
}
extend(monthlyAllocation, entityObject);

monthlyAllocation.prototype.Month = ko.observable('');
monthlyAllocation.prototype.Allocation = ko.observable(0);
monthlyAllocation.prototype.LoadFromObject = function (obj) {
    this.Id(obj.Id)
        .Month(obj.Month)
        .Allocation(obj.Allocation);
    return this;
};

var _initAllocationTree = { "Teams":
            [{ "Id": 1,
                "Name": "Management",
                "Members": [
                    { "Id": 2, "FullName": "David Neigler",
                        "Projects": [
                            { "Id": 39, "Name": "Management",
                                "MonthlyAllocations": [
                                    { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 }
                                ]
                            },
                            { "Id": 40, "Name": "Project 2",
                                "MonthlyAllocations": [
                                    { "Month": "\/Date(1293858000000)\/", "Id": 999, "Allocation": 1.00000 }
                                ]
                            }
                        ]
                    }
                ],
                "LeadFullName": "David Neigler",
                "LeadId": "2"
            }],
    "Months": ["\/Date(1293858000000)\/", "\/Date(1296536400000)\/", "\/Date(1298955600000)\/", "\/Date(1301630400000)\/", "\/Date(1304222400000)\/", "\/Date(1306900800000)\/", "\/Date(1309492800000)\/", "\/Date(1312171200000)\/", "\/Date(1314849600000)\/", "\/Date(1317441600000)\/", "\/Date(1320120000000)\/", "\/Date(1322715600000)\/", "\/Date(1325394000000)\/", "\/Date(1328072400000)\/"]
};
    
var resourceAllocationVM = {
    AllocationTree: ko.observable(new allocationTree(_initAllocationTree)
    //.LoadFromObject(_initAllocationTree)
    ),
    initJQuery: function () {
        $("#tabs").tabs({
            spinner: 'Retrieving data...',
            select: function (event, ui) {
                // $('.statusCaptionText').attr('cols',60);
                //$('.statusCaptionText').autoGrow();
            }
        });
    },
    loadAllocationTree: function (startDate, endDate) {
        
        var sd = ($.isEmptyObject(startDate) ? '1/1/2011' : startDate);
        var todayString = Date.today();
        todayString = todayString.toString('MM/dd/yyyy');
        var ed = ($.isEmptyObject(endDate) ? todayString : endDate);
        var url = "/ResourceAllocation/GetResourceAllocations?startDate=" + sd + "&endDate=" + ed;
        // alert(url);
        console.log(url);
        $.ajax({
            url: url,
            dataType: "json",
            converters: {
                "text json": function (data) { return $.parseJSON(data, true); }
            },
            success: function (response) {
                if (response != null) {
                    var at = new allocationTree(response);
                    // at.LoadFromObject(response);
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
