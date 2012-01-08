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
                                .Report(sr)
                                .Id(item.Id)
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
                        sr.loadStatusItem(sri);
                    });
                    statusReportVM.Report(sr);
                    //$("#tabs").tabs();
                    console.log("calling to #tabs in jquery");
                    $("#tabs").tabs();
                    $(".datefield").datepicker({dateFormat:'yy-mm-dd',changeMonth:true,changeYear:true });
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
        console.log(team.Name());
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
            this.ItemsByTeam.push(team);
            //statusItem.ProjectTeamId()
        }
        team.addProject(this.getOrCreateProjectFromStatusItem(statusItem));
        return team;
    };

    this.getOrCreateProjectFromStatusItem = function (statusItem) {
        //ItemsByProject
        var projects = ($.grep(this.ItemsByProject(), function (i) {
            return (i.ProjectName() == statusItem.ProjectName());
        }));
        var proj = null;
        if (projects.length > 0) {
            proj = projects[0];
            proj.addItem(statusItem);
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
            proj.addItem(statusItem);
            this.ItemsByProject.push(proj);
        }
        return proj;
    };

    this.PeriodStartFormatted = ko.dependentObservable(function () {
        return parseJsonDateString(this.PeriodStart());
    } .bind(this));
    
    this.Name = ko.dependentObservable(function() {
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

    this.addStatusItem = function() {
        if (this.StatusItemToAdd() != '') {
            var sri = new statusReportItem();
            sri.Caption(this.StatusItemToAdd());
            sri.MilestoneType(this.StatusItemMilestoneToAdd());
            sri.MilestoneDate(this.StatusItemDateToAdd());
            this.Items.push(sri);
        };
        this.StatusItemToAdd('');
        this.StatusItemMilestoneToAdd(0);
        this.StatusItemDateToAdd(new Date());
    };

    this.removeStatusItem = function(itemToRemove) {
        this.Items.remove(itemToRemove);
    };
};

function statusReportItem() {
    this.Report = ko.observable(null);
    this.Id = ko.observable(0);
    this.TopicCaption = ko.observable('');
    this.TopicExternalId = ko.observable(null);
    this.TopicId = ko.observable(0);
    this.MilestoneType = ko.observable(1);
    this.MilestoneDate = ko.observable(new Date());
    this.MilestoneConfidenceLevel = ko.observable(0);
    this.Caption = ko.observable('');
    this.ProjectId = ko.observable(1);
    this.ProjectName = ko.observable('');
    this.ProjectDepartmentName = ko.observable('');
    this.ProjectDepartmentManagerFullName = ko.observable('');
    this.ProjectType = ko.observable(0);
    this.ProjectTeamId = ko.observable(0);
    this.ProjectTeamName = ko.observable('');
    this.ProjectLeadFullName = ko.observable('');
    this.ProjectTeamLeadFullName = ko.observable('');
    this.MilestoneDateFormatted = ko.dependentObservable(function () {
        return parseJsonDateString(this.MilestoneDate());
    } .bind(this));
    this.MilestoneDateString = ko.dependentObservable(function () {
        return $.datepicker.formatDate('mm/dd/yy', this.MilestoneDate());
    } .bind(this));
};

function projectStatus() {
    this.Report = ko.observable(null);
    this.ProjectId = ko.observable(1);
    this.ProjectName = ko.observable('');
    this.ProjectDepartmentName = ko.observable('');
    this.ProjectDepartmentManagerFullName = ko.observable('');
    this.ProjectType = ko.observable(0);
    this.ProjectTeamId = ko.observable(0);
    this.ProjectTeamName = ko.observable('');
    this.ProjectLeadFullName = ko.observable('');
    this.ProjectTeamLeadFullName = ko.observable('');
    this.Items = ko.observableArray([]);

    this.addItem = function (statusItem) {
        this.Items.push(statusItem);
    };
};

function teamStatus() {
    this.Report = ko.observable(null);
    this.TeamId = ko.observable(0);
    this.Name = ko.observable('');
    this.ProjectItems = ko.observableArray([]);

    this.addProject = function(project) {
        this.ProjectItems.push(project);
    };
}