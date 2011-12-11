var jsonDateRE = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;

var parseJsonDateString = function (value) {
    var arr = value && jsonDateRE.exec(value);
    if (arr) {
        return new Date(parseInt(arr[1]));
    }
    return value;
};

var statusReport = {
    Id: ko.observable(0),
    PeriodStart: ko.observable(/Date(1322456400000)/),
    Caption: ko.observable(''),
    NumberOfStatusItems: ko.observable(0),
    Items: ko.observableArray([]),
    StatusItemToAdd: ko.observable(''),
    StatusItemDateToAdd: ko.observable(new Date()),
    StatusItemMilestoneToAdd: ko.observable(0)
};

statusReport.PeriodStartFormatted = ko.dependentObservable(function () {
    return parseJsonDateString(statusReport.PeriodStart());
});

statusReport.Name = ko.dependentObservable(function () {
    return statusReport.Caption() +
    " (" + statusReport.PeriodStart() + ")";
});

statusReport.addReport = function () {
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

var statusReportItem =
{
    Id: ko.observable(0),
    TopicCaption: ko.observable(''),
    TopicExternalId: ko.observable(null),
    TopicId: ko.observable(0),
    MilestoneType: ko.observable(1),
    MilestoneDate: ko.observable(new Date()),
    MilestoneConfidenceLevel: ko.observable(0),
    Caption: ko.observable(''),
    ProjectId: ko.observable(1),
    ProjectName: ko.observable(''),
    ProjectDepartmentName: ko.observable(''),
    ProjectDepartmentManagerFullName: ko.observable(''),
    ProjectType: ko.observable(0),
    ProjectTeamName: ko.observable('')
};

statusReport.addStatusItem = function () {
    if (statusReport.StatusItemToAdd() !== '') {
        var sr = { Caption: ko.observable(statusReport.StatusItemToAdd()),
            MilestoneType: ko.observable(statusReport.StatusItemMilestoneToAdd()),
            MilestoneDate: ko.observable(statusReport.StatusItemDateToAdd())
        };
        statusReport.Items.push(sr);
        statusReport.StatusItemToAdd('');
        statusReport.StatusItemMilestoneToAdd(0);
        statusReport.StatusItemDateToAdd(new Date());
    }
};

statusReport.removeStatusItem = function (itemToRemove) {
    statusReport.Items.remove(itemToRemove);
}