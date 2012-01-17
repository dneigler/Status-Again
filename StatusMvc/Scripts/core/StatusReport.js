/*!
* jQuery.parseJSON() extension (supports ISO & Asp.net date conversion)
*
* Version 1.0 (13 Jan 2011)
*
* Copyright (c) 2011 Robert Koritnik
* Licensed under the terms of the MIT license
* http://www.opensource.org/licenses/mit-license.php
*/
(function ($) {

	// JSON RegExp
	var rvalidchars = /^[\],:{}\s]*$/;
	var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
	var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
	var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
	var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
	var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

	// replacer RegExp
	var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
	var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

	// determine JSON native support
	var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
	var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

	var jsonDateConverter = function (key, value) {
		if (typeof (value) === "string") {
			if (dateISO.test(value)) {
				return new Date(value);
			}
			if (dateNet.test(value)) {
				return new Date(parseInt(dateNet.exec(value)[1], 10));
			}
		}
		return value;
	};

	$.extend({
		parseJSON: function (data, convertDates) {
			/// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
			/// <param name="data" type="String">The JSON string to parse.</param>
			/// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>

			// convertDates = convertDates === false ? false : true;
			
			if (typeof data !== "string" || !data) {
				return null;
			}
			
			// Make sure leading/trailing whitespace is removed (IE can't handle it)
			data = $.trim(data);

			// Make sure the incoming data is actual JSON
			// Logic borrowed from http://json.org/json2.js
			if (rvalidchars.test(data
				.replace(rvalidescape, "@")
				.replace(rvalidtokens, "]")
				.replace(rvalidbraces, ""))) {
				// Try to use the native JSON parser
				if (extendedJSON || (nativeJSON && convertDates !== true)) {
					return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
				}
				else {
					data = convertDates === true ?
						data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
							.replace(replaceNet, "new Date($1)") :
						data;
					return (new Function("return " + data))();
				}
			} else {
				$.error("Invalid JSON: " + data);
			}
		}
	});
})(jQuery);
 
 
var jsonDateRE = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;

var parseJsonDateString = function (value) {
	var arr = value && jsonDateRE.exec(value);
	if (arr) {
		return new Date(parseInt(arr[1]));
	}
	return value;
};


ko.bindingHandlers.datepicker = {
	init: function (element, valueAccessor, allBindingsAccessor) {
		//initialize datepicker with some optional options
		var options = allBindingsAccessor().datepickerOptions || {};
		$(element).datepicker(options);

		//handle the field changing
		ko.utils.registerEventHandler(element, "change", function () {
			var observable = valueAccessor();
			observable($(element).datepicker("getDate"));
		});

		//handle disposal (if KO removes by the template binding)
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$(element).datepicker("destroy");
		});

	},
	update: function (element, valueAccessor) {
		var value = ko.utils.unwrapObservable(valueAccessor());
		//console.log("value", value);
		$(element).datepicker("setDate", value);
	}
};


var statusReportVM = {
	Report: ko.observable(new statusReport()),
	loadReport: function (reportDate) {
		var url = "/StatusReport/GetStatusReport?statusDate=" + reportDate;
		$.ajax({
			url: url,
			dataType: "json",
			converters: {
				"text json": function (data) {
					return $.parseJSON(data, true);
				}
			},
			success: function (response) {
				if (response != null) {
					var sr = new statusReport()
						.Caption(response.PeriodStart)
						.PeriodStart(response.PeriodStart)
						.Id(response.Id)
						.NumberOfStatusItems(response.NumberOfStatusItems)
						.StatusItemToAdd("")
						.StatusItemDateToAdd(new Date())
						.StatusItemMilestoneToAdd(0);
					$.each(response.Items, function (x, item) {
						var sri = new statusReportItem()
							.LoadFromObject(item);
						//.Report(sr)
//								.Id(item.Id)
//								.TopicCaption(item.TopicCaption)
//								.TopicExternalId(item.TopicExternalId)
//								.TopicId(item.TopicId)
//								.MilestoneType(item.MilestoneType)
//								.MilestoneDate(item.MilestoneDate)
//								.MilestoneConfidenceLevel(item.MilestoneConfidenceLevel)
//								.Caption(item.Caption)
//								.ProjectId(item.ProjectId)
//								.ProjectName(item.ProjectName)
//								.ProjectDepartmentName(item.ProjectDepartmentName)
//								.ProjectDepartmentManagerFullName(item.ProjectDepartmentManagerFullName)
//								.ProjectType(item.ProjectType)
//								.ProjectTeamId(item.ProjectTeamId)
//								.ProjectTeamName(item.ProjectTeamName)
//								.ProjectLeadFullName(item.ProjectLeadFullName)
//								.ProjectTeamLeadFullName(item.ProjectTeamLeadFullName);
//						sri.OriginalVersion = getMembers(item);
//						sri.ListenForChanges();

						sr.loadStatusItem(sri);
					});
					statusReportVM.Report(sr);
					//$("#tabs").tabs();
					//console.log("calling to #tabs in jquery");
					$("#tabs").tabs();
					// $(".datefield").datepicker({dateFormat:'yy-mm-dd',changeMonth:true,changeYear:true });
					$('textarea input').autoResize({
						// On resize:
						onResize: function () {
							$(this).css({ opacity: 0.8 });
						},
						// After resize:
						animateCallback: function () {
							$(this).css({ opacity: 1 });
						},
						// Quite slow animation:
						animateDuration: 300,
						// More extra space:
						extraSpace: 40
					});
				} else {
					alert(response.message);
				}
			},
			error: function (response) {
				alert("Failed to get report for date " + reportDate + "..." + response.responseText);
			}
		});
	}
};

function statusReport() {
	var self = this;
	this.Id = ko.observable(0);
	this.PeriodStart = ko.observable(/Date(1322456400000)/);
	this.Caption = ko.observable('');
	this.NumberOfStatusItems = ko.observable(0);
	this.Items = ko.observableArray([]);
	this.StatusItemToAdd = ko.observable('');
	this.StatusItemDateToAdd = ko.observable(new Date());
	this.StatusItemMilestoneToAdd = ko.observable(0);

	this.ItemsByProject = ko.observableArray([]);
	this.ItemsByTeam = ko.observableArray([]);

	this.loadStatusItem = function (statusItem) {
		// autocreates team and project if not found
		this.Items.push(statusItem);
		var team = this.getOrCreateTeamFromStatusItem(statusItem);
		//console.log(team.Name());
	};

	this.save = function () {
		var url = "/StatusReport/Save";
		var miniReport = new statusReport();
		miniReport.PeriodStart = self.PeriodStart();
		miniReport.Id = self.Id();
		miniReport.Caption = self.Caption();
		
		miniReport.Items = ko.utils.arrayFilter(self.Items(), function (item) {
			return item.HasChanges(); // ko.utils.stringStartsWith(item.name().toLowerCase(), filter);
		});
		console.log("About to save self.Items = " + miniReport.Items.length);
		
		$.ajax({
			url: url,
			dataType: "json",
			data: ko.toJSON({ report: miniReport }),
			type: "post",
			contentType: "application/json",
			success: function (result) { alert(result); },
			error: function (xhr, status) {
				switch (status) {
					case 404:
						alert('File not found');
						break;
					case 500:
						alert('Server error');
						break;
					case 0:
						alert('Request aborted');
						break;
					default:
						alert('Unknown error ' + status);
				}
			}
		});
	};
	
	

	this.teamCounter = 1;

	this.getOrCreateTeamFromStatusItem = function (statusItem) {
		// ItemsByTeam
		var teams = ($.grep(this.ItemsByTeam(), function (i) {
			return (i.Name() == statusItem.ProjectLeadFullName());
		}));
		var team = null;
		if (teams.length > 0) {
			team = teams[0];
		} else {
			team = new teamStatus()
				.Report(this)
				.TeamId(this.teamCounter++)
				.Name(statusItem.ProjectLeadFullName());
			console.log("Created team " + team.Name());
			this.ItemsByTeam.push(team);
			//statusItem.ProjectTeamId()
		}

		team.addProject(this.getOrCreateProjectFromStatusItem(statusItem));
		return team;
	};

	this.getOrCreateProjectFromStatusItem = function (statusItem) {
		//ItemsByProject
		var projects = ($.grep(this.ItemsByProject(), function (i) {
			return (i.ProjectId() == statusItem.ProjectId());
		}));
		var proj = null;
		if (projects.length > 0) {
			proj = projects[0];
		} else {
			proj = new projectStatus()
				.Report(this)
				.ProjectId(statusItem.ProjectId())
				.ProjectName(statusItem.ProjectName())
				.ProjectDepartmentName(statusItem.ProjectDepartmentName())
				.ProjectDepartmentManagerFullName(statusItem.ProjectDepartmentManagerFullName())
				.ProjectType(statusItem.ProjectType())
				.ProjectTeamId(statusItem.ProjectTeamId())
				.ProjectTeamName(statusItem.ProjectTeamName())
				.ProjectLeadFullName(statusItem.ProjectLeadFullName())
				.ProjectTeamLeadFullName(statusItem.ProjectTeamLeadFullName());
			console.log("Created project " + proj.ProjectName() + " - " + proj.ProjectId());
			this.ItemsByProject.push(proj);
		}
		proj.addItem(statusItem);
		return proj;
	};

	this.PeriodStartFormatted = ko.computed(function () {
		return parseJsonDateString(this.PeriodStart());
	} .bind(this));
	
	this.Name = ko.computed(function() {
		return this.Caption() + " (" + this.PeriodStart() + ")";
	}.bind(this));
	
	this.addReport = function () {
		$.ajax({
			url: "/Home/Create/",
			type: 'post',
			data: ko.toJSON(this),
			contentType: 'application/json',
			success: function (result) {
				alert(result);
			}
		});
	};

	this.addStatusItem = function (sri) {
	    //if (this.StatusItemToAdd() != '') {
//	        var sri = new statusReportItem();
//	        sri.Caption(this.NewStatusItemText());
//	        sri.MilestoneType(this.StatusItemMilestoneToAdd());
//	        sri.MilestoneDate(this.StatusItemDateToAdd());
//	        sri.ListenForChanges();
	    this.Items.push(sri);
	    //};
	    this.StatusItemToAdd('');
	    this.StatusItemMilestoneToAdd(0);
	    this.StatusItemDateToAdd(new Date());
	};

	self.removeStatusItem = function (itemToRemove) {
		console.log("about to remove status item " + itemToRemove);
		self.Items.remove(itemToRemove);
	};
};

/**
Returns the names of all the obj's
variables and functions in a sorted
array
*/

//do some basic mapping (without mapping plugin)
//var mappedData = ko.utils.arrayMap(dataFromServer, function (item) {
//    return new Item(item.name, item.category, item.price);
//});

function getMembers(original) {
    var sri = new Array();
    $.each(original, function (index, item) {
        sri[index] = ko.utils.unwrapObservable(item);
    });
    return sri;
//    // Shallow copy
//	var clone = jQuery.extend({}, original);
//	// var b = obj.slice(0);//  jQuery.extend({}, obj);
//	return clone;
}

function statusReportItem() {
	var self = this;
	//self.Report = ko.observable(null);
	self.OriginalVersion = {};
	self.Id = ko.observable(0);
	self.TopicCaption = ko.observable('');
	self.TopicExternalId = ko.observable(null);
	self.TopicId = ko.observable(0);
	self.MilestoneType = ko.observable(1);
	self.MilestoneDate = ko.observable(new Date());
	self.MilestoneConfidenceLevel = ko.observable(0);
	self.Caption = ko.observable('');
	self.ProjectId = ko.observable(1);
	self.ProjectName = ko.observable('');
	self.ProjectDepartmentName = ko.observable('');
	self.ProjectDepartmentManagerFullName = ko.observable('');
	self.ProjectType = ko.observable(0);
	self.ProjectTeamId = ko.observable(0);
	self.ProjectTeamName = ko.observable('');
	self.ProjectLeadFullName = ko.observable('');
	self.ProjectTeamLeadFullName = ko.observable('');

	this.LoadFromObject = function (item) {
	    self.Id(item.Id)
		.TopicCaption(item.TopicCaption)
		.TopicExternalId(item.TopicExternalId)
		.TopicId(item.TopicId)
		.MilestoneType(item.MilestoneType)
		.MilestoneDate(item.MilestoneDate)
		.MilestoneConfidenceLevel(item.MilestoneConfidenceLevel)
		.Caption(item.Caption)
		.ProjectId(item.ProjectId)
		.ProjectName(item.ProjectName)
		.ProjectDepartmentName(item.ProjectDepartmentName)
		.ProjectDepartmentManagerFullName(item.ProjectDepartmentManagerFullName)
		.ProjectType(item.ProjectType)
		.ProjectTeamId(item.ProjectTeamId)
		.ProjectTeamName(item.ProjectTeamName)
		.ProjectLeadFullName(item.ProjectLeadFullName)
		.ProjectTeamLeadFullName(item.ProjectTeamLeadFullName);
	    self.OriginalVersion = getMembers(item);
	    self.ListenForChanges();
	    return self;
	};
	
	self.Subscribers = new Array();

	this.ClearSubscribers = function () {
		$.each(self.Subscribers, function (x, item) {
			item.dispose();
		});
		// clear change logs
		self.ChangeLog.removeAll();
	};

	self.MilestoneDateFormatted = ko.computed(function () {
		return parseJsonDateString(self.MilestoneDate());
	} .bind(this));

	self.ChangeLog = ko.observableArray();
	this.ListenForChanges = function () {
		self.ClearSubscribers();
		$.each(self, function (x, item) {
			if (x != "ChangeLog" && x != "HasChanges" && ko.isObservable(item)) {
				console.log("Subscribing to changes in " + x);

				var sub = item.subscribe(function (newValue) {
					var currentlyChangeExists = ($.inArray(x, self.ChangeLog()) >= 0);
					if (newValue != self.OriginalVersion[x] && !currentlyChangeExists) {
						console.log("The new value for " + x + " is " + newValue + " from " + self.OriginalVersion[x]);
						self.ChangeLog.push(x);
					} else if (currentlyChangeExists) {
						self.ChangeLog.pop(x);
					}
				});
				self.Subscribers.push(sub);
			}
		});
	};
	this.HasChanges = ko.computed(function () {
		console.log("HasChanges called returning " + self.ChangeLog().length);
		return self.ChangeLog().length > 0;
		//console.log(self.OriginalVersion);
		//	    $.each(self.OriginalVersion, function (x, item) {
		//	        if (self[item]() != self.OriginalVersion[item])
		//	            return true;
		//	    });
		//return false;
	} .bind(this));
	this.MilestoneDateString = ko.computed(function () {
		return $.datepicker.formatDate('mm/dd/yy', self.MilestoneDate());
	} .bind(this));
};

function projectStatus() {
	var self = this;
	self.Report = ko.observable(null);
	self.ProjectId = ko.observable(1);
	self.ProjectName = ko.observable('');
	self.ProjectDepartmentName = ko.observable('');
	self.ProjectDepartmentManagerFullName = ko.observable('');
	self.ProjectType = ko.observable(0);
	self.ProjectTeamId = ko.observable(0);
	self.ProjectTeamName = ko.observable('');
	self.ProjectLeadFullName = ko.observable('');
	self.ProjectTeamLeadFullName = ko.observable('');
	self.Items = ko.observableArray([]);
	self.NewStatusItemText = ko.observable();
	self.NewStatusItemMilestoneDate = ko.observable(new Date());

	this.HasNewItem = ko.computed(function () {
	    return (self.NewStatusItemText() != '' && self.NewStatusItemText() != null);
	} .bind(this));
    
	self.removeStatusItem = function (itemToRemove) {
		console.log("about to remove item via projectStatus " + itemToRemove.TopicCaption());
		self.Items.remove(itemToRemove);
		self.Report().removeStatusItem(itemToRemove);
	};

	self.addItem = function (statusItem) {
		self.Items.push(statusItem);
		self.Report().addStatusItem(statusItem);
		$(".datefield").datepicker({ dateFormat: 'yy-mm-dd', changeMonth: true, changeYear: true });
	};
	self.addItemFromTemplate = function () {
		var statusItem = new statusReportItem()
		//.Report(self.Report)
			.ProjectId(self.ProjectId())
			.ProjectName(self.ProjectName())
			.ProjectDepartmentName(self.ProjectDepartmentName())
			.ProjectDepartmentManagerFullName(self.ProjectDepartmentManagerFullName())
			.ProjectType(self.ProjectType())
			.ProjectTeamId(self.ProjectTeamId())
			.ProjectTeamName(self.ProjectTeamName())
			.ProjectLeadFullName(self.ProjectLeadFullName())
			.ProjectTeamLeadFullName(self.ProjectTeamLeadFullName())
			;
		// statusItem.OriginalVersion = getMembers(statusItem);
		// statusItem.ListenForChanges();

		// statusItem.ListenForChanges();
		// needs to be treated as new anyway
		self.addItem(statusItem);

	};
	self.addItemFromStubProperties = function () {
	    var statusItem = new statusReportItem()
	    // .Report(self.Report)
			.ProjectId(self.ProjectId())
			.ProjectName(self.ProjectName())
			.ProjectDepartmentName(self.ProjectDepartmentName())
			.ProjectDepartmentManagerFullName(self.ProjectDepartmentManagerFullName())
			.ProjectType(self.ProjectType())
			.ProjectTeamId(self.ProjectTeamId())
			.ProjectTeamName(self.ProjectTeamName())
			.ProjectLeadFullName(self.ProjectLeadFullName())
			.ProjectTeamLeadFullName(self.ProjectTeamLeadFullName())
			.TopicCaption(self.NewStatusItemText())
			.Caption(self.NewStatusItemText())
			.MilestoneDate(self.NewStatusItemMilestoneDate());
	    statusItem.ListenForChanges();
	    self.addItem(statusItem);
	    self.NewStatusItemText('');
	    self.NewStatusItemMilestoneDate(new Date());
	};
};

function teamStatus() {
	this.Report = ko.observable(null);
	this.TeamId = ko.observable(0);
	this.Name = ko.observable('');
	this.ProjectItems = ko.observableArray([]);

	this.addProject = function (project) {
		var projects = ($.grep(this.ProjectItems(), function (i) {
			return (i.ProjectId() == project.ProjectId());
		}));
		if (projects.length == 0)
			this.ProjectItems.push(project);
	};
}