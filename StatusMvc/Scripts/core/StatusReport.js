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
        // alert(url);
        $.ajax({
            url: url,
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
                    $.each(response.Items, function(x, item) {
                        sr.Items.push(new statusReportItem()
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
                                .ProjectTeamName(item.ProjectTeamName)
                                .ProjectLeadFullName(item.ProjectLeadFullName)
                                .ProjectTeamLeadFullName(item.ProjectTeamLeadFullName)
                        );
                    });
                    statusReportVM.Report(sr);

                    //                    $.each(response.results, function (x, game) {
                    //                        theViewModel.games.push(new gameModel()
                    //            .id(game.Id)
                    //            .name(game.Name)
                    //            .releaseDate(game.ReleaseDate)
                    //            .price(game.Price)
                    //            .imageUrl(game.ImageUrl)
                    //            .genre(game.Genre));
                    //});
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
    this.ProjectTeamName = ko.observable('');
    this.ProjectLeadFullName = ko.observable('');
    this.ProjectTeamLeadFullName = ko.observable('');
//    this.removeStatusItem = function () {
//        this.Report.removeStatusItem(this);
//    };
};
