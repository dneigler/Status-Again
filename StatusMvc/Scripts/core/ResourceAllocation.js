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
}

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

function entityObject() {
    // this is required, otherwise the prototype for Id method will always refer
    // to the same instance.
    this.Id = ko.observable(0);
};

entityObject.prototype.Id = ko.observable(0);

entityObject.prototype.Exists = function (collection, objId) {
    var item1 = ko.utils.arrayFilter(ko.utils.unwrapObservable(collection), function (item) {
        return item.Id() == objId;
    });
    return (item1.length > 0);
};

function allocationTree() {
	var self = this;

	this.Months = ko.observableArray([]);
    this.Teams = ko.observableArray([]);

    var initData = function (response) {
        this.Months(response.Months);
        $.each(response.Teams, function (x, item) {
            var sri = new team()
				.LoadFromObject(item);
            self.LoadTeam(sri);
        });

        return self;
    };

    this.LoadTeam = function (obj) {
        var matchingItem = ko.utils.arrayFilter(self.Teams(), function (item) {
            return item.Id() == obj.Id();
        });
        if (!matchingItem)
            self.Teams.push(obj);
    };
};

function team() {
    team.superclass.constructor.call(this);
    this.Name = ko.observable('');
    this.LeadFullName = ko.observable('');
    this.LeadId = ko.observable(0);
    this.Members = ko.observableArray([]);
};
extend(team, entityObject);

// team.prototype.Id = ko.observable(0);
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