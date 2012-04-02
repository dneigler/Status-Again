﻿/// <reference path="../api/jquery-1.7.1.min.js" />
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
    var self = this;
    this.Parent = ko.observable(null);

    this.Id = ko.observable(0);
    this.OriginalVersion = {};
    this.HasInsertion = ko.computed(function () {
        return this.Id() == 0;
    }.bind(this));

    this.ChangeLog = ko.observableArray([]);
    this.HasDeletion = ko.observable(false);
    this._subscribers = new Array();

    this._dependentCollections = ko.observableArray([]); // new Array();

    // dependent collections to be included
    this.HasChanges = ko.computed(function () {
        var hasC = this.ChangeLog().length > 0 || this._dependentChangesFlag() === true; // || this.HasChildChanges();
        if (hasC === false) {
            console.log("entityObject.HasChanges calling HasChildChanges");
            var hasC1 = this.HasChildChanges();
            console.log(typeof this + ": entityObject.HasChanges hasC1 returned " + hasC1);
        }
        // check dependent collections
        //if (hasC === false && this._dependentCollections.length > 0) {
        //    console.log("Checking _dependentCollections");
        //    $.each(this._dependentCollections(), function (x, item) {
        //        console.log("HasChanges.each " + item);

        //        $.each(item(), function (y, dc) {
        //            if (dc.HasChanges()) {
        //                hasC = true;
        //                console.log("i.HasChanges() == true on index " + y);

        //            }
        //        });    
        //    });
        //}
        return hasC;
    }.bind(this));

    this.HasChildChanges1 = ko.computed(function () {
        console.log("base.HasChildChanges called");
        return false;
    }.bind(this));

    this.HasChildChanges = ko.computed(function () {
        var self = this;
        var retVal = false;
        // find all arrays
        $.each(self, function (x, prop) {
            if (!self.isInternal(x, prop)) {

                if (ko.isObservable(prop) && $.isArray(prop())) {
                    console.log("HasChildChanges examining " + x + " : " );//+ JSON.stringify(prop()));
                    var val = null;
                    if (prop()["HasChanges"] != null) {
                        val = ko.utils.arrayFirst(prop(), function (item1) {
                            if (item1["HasChanges"] != null)
                                return (item1.HasChanges());
                        });
                    }

                    retVal = (val != null);
                    console.log("project.prototype.HasChildChanges called with val " + val);
                }
            }
        });

        return retVal;
    }.bind(this));

    this._dependentChangesFlag = ko.observable(false);

    this.Editable = ko.computed(function () {
        return (this.HasDeletion() == false);
    }.bind(this));
};
entityObject.prototype.Id = ko.observable(0);
entityObject.prototype.HasChildChanges = function () { console.log("bogus HasChildChanges"); return false; }

entityObject.prototype.Exists = function (collection, objId) {
    var item1 = ko.utils.arrayFilter(ko.utils.unwrapObservable(collection), function (item) {
        return item.Id() == objId;
    });
    return (item1.length > 0);
};

entityObject.prototype.isInternal = function (x, item) {
    return !(x != "ChangeLog" && x != "HasChanges" && x != "HasDeletion" && x != "_dependentCollections" && x != "_dependentChangesFlag" && x != "HasInsertion" && x != "Editable" && x != "Parent" && ko.isObservable(item));
};

// entityObject.prototype.HasInsertion = ko.observable(false);
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
    // self.HasInsertion(false);
};

entityObject.prototype.ListenForChanges = function () {
    var self = this;
    this.OriginalVersion = this.getMembers(this);
    self.ClearSubscribers();
    
    $.each(self, function (x, item) {
        if (!self.isInternal(x, item)) {
            var sub = item.subscribe(function (newValue) {
                self._updateChangeTracking(x, newValue);

            });
            // console.log("ListenForChanges subscriber added for " + x + " with value " + item());
            self._subscribers.push(sub);
        }
    });
};
entityObject.prototype._subscribers = new Array();
entityObject.prototype._dependentCollections = ko.observableArray([]);// new Array();
entityObject.prototype._dependentChangesFlag = ko.observable(false);

entityObject.prototype.ClearSubscribers = function () {
    var self = this;
    $.each(self._subscribers, function (x, item) {
        item.dispose();
    });
    // clear change logs
    self.ChangeLog.removeAll();
};

entityObject.prototype.getMembers = function (original) {
    var sri = new Array();
    $.each(original, function (index, item) {
        sri[index] = ko.utils.unwrapObservable(item);
    });
    return sri;
}

entityObject.prototype._updateChangeTracking = function (propertyName, newValue) {
    var self = this;
    var currentlyChangeExists = ($.inArray(propertyName, self.ChangeLog()) >= 0);
    var origValue = self.OriginalVersion[propertyName];
    if (newValue !== origValue && !currentlyChangeExists) {
        console.log("The new value for " + propertyName + " is " + newValue + " from " + origValue);
        self.ChangeLog.push(propertyName);

        // dependencies?
        if (self.Parent() !== null) {
            // self.Parent()._dependentCollections.valueHasMutated();
            self.Parent()._dependentChangesFlag(true);
        } else {
            console.log("newValue !== origValue && !currentlyChangeExists/self.Parent() == null");
        }
    } else if (newValue === origValue && currentlyChangeExists) {
        self.ChangeLog.pop(propertyName);
        if (self.Parent() != null) {
            // self.Parent()._dependentCollections.valueHasMutated();
            self.Parent()._dependentChangesFlag(true);
        } else {
            console.log("newValue === origValue && currentlyChangeExists/self.Parent() == null");
        }
    }
};

function allocationTree(response) {
    allocationTree.superclass.constructor.call(this);
    this.Months = ko.observableArray([]);
    this.Teams = ko.observableArray([]);
    this.LoadFromObject(response);

    this.HasChildChanges = ko.computed(function () {
        var retVal = false;
        $.each(this.Teams(), function (x, item) {
            if (item.HasChanges())
                retVal = true;
        });
        return retVal;
    }.bind(this));
};
extend(allocationTree, entityObject);

allocationTree.prototype.Months = ko.observableArray([]);
allocationTree.prototype.Teams = ko.observableArray([]);

allocationTree.prototype.LoadFromObject = function (response) {
    var self = this;
    // need to iterate through teams
    // clean up months first

    this.Months = ko.observableArray([]);

    $.each(response.Months, function (x, item) {
        self.Months().push(getShortMonthName(item));
    });

    //this.Months(response.Months);
    $.each(response.Teams, function (x, item) {
        var t = new team()
			.LoadFromObject(item);
        self.LoadTeam(t);
    });
    self._dependentCollections.push(this.Teams);
    return self;
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

    this.HasChildChanges = ko.computed(function () {
        var retVal = false;
        $.each(this.Members(), function (x, item) {
            if (item.HasChanges())
                retVal = true;
        });
        return retVal;
    }.bind(this));
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
        m.Parent(self);
        self.LoadMember(m);
    });
    this._dependentCollections.push(this.Members);
    return this;
};

team.prototype._getDependentArrays = function () {
    return this.Members;
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
    
    this.HasChildChanges = ko.computed(function () {
        var retVal = false;
        $.each(this.Projects(), function (x, item) {
            if (item.HasChanges())
                retVal = true;
        });
        return retVal;
    }.bind(this));
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
        p.Parent(self);
        self.LoadProject(p);
    });
    this._dependentCollections.push(this.Projects);
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

    //this.HasChildChanges1 = ko.computed(function () {

    //    var retVal = false;
    //    var val = ko.utils.arrayFirst(this.Allocations(), function (item) {
    //        return (item.HasChanges());
    //        //if (item.HasChanges())
    //        //    retVal = true;
    //    });
    //    retVal = (val != null);
    //    console.log("project.prototype.HasChildChanges called with val " + val);
    //    return retVal;
    //}.bind(this));
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
        m.Parent(self);
        self.LoadAllocation(m);

    });
    this._dependentCollections.push(this.Allocations);
    return this;
};

project.prototype.LoadAllocation = function (obj) {
    // following is an issue if we're looking at new entries
    var matchingItem = (!obj.HasInsertion() && this.AllocationExists(obj.Id()));

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

//monthlyAllocation.prototype.Has0Allocation = ko.computed(function () {

//}.bind(this));

//monthlyAllocation.prototype.Has0Allocation = ko.computed(function () {

//}.bind(this));

monthlyAllocation.prototype.LoadFromObject = function (obj) {
    this.Id(obj.Id)
        .Month(obj.Month)
        .Allocation(obj.Allocation);
    //  need to encapsulate OriginalVersion and ListenForChanges logic
    // this.OriginalVersion = getMembers(item);
    this.ListenForChanges();
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
                            }
                        ]
                    }
                ],
                "LeadFullName": "David Neigler",
                "LeadId": "2"
            }],
    "Months": ["\/Date(1293858000000)\/"]
};
    
var resourceAllocationVM = {
    AllocationTree: ko.observable(new allocationTree(_initAllocationTree)
    //.LoadFromObject(_initAllocationTree)
    ),
    //ChangedAllocations: ko.computed(function () {
    //    return ko.utils.arrayFilter(this.allocationTree().Items(), function (item) {
    //        return item.HasChanges() || item.HasInsertion(); // ko.utils.stringStartsWith(item.name().toLowerCase(), filter);
    //    });
    //}.bind(this)),
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
        
        var sd = ($.isEmptyObject(startDate) ? '1/1/2012' : startDate);
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
