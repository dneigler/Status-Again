using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;
using Status.Model;

namespace StatusMvc.Models
{
    public class StatusReportViewModel
    {
        //private IList<StatusItem> _items = null;
        public IList<DateTime> StatusReportDates { get; set; } 
        public int Id { get; set; }
        public DateTime PeriodStart { get; set; }
        [Required, CanSort, Display(Name = "Caption")]
        public string Caption { get; set; }
        public int NumberOfStatusItems { get; set; }
        public IList<StatusReportItemViewModel> Items { get; set; }
        public IList<StatusReportItemViewModel> ItemsToRemove { get; set; } 
    }

    public class StatusReportItemViewModel
    {
        public int Id { get; set; }
        
        public int StatusReportId { get; set; }

        [Required, CanSort, Display(Name = "Caption")]
        public string TopicCaption { get; set; }
        public string TopicExternalId { get; set; }
        public int TopicId { get; set; }

        [Required, CanSort, Display(Name = "Type")]
        public MilestoneTypes MilestoneType { get; set; }

        [DateRange(SMinimum = "Today-3M", SMaximum = "Today+6M")]
        public DateTime? MilestoneDate { get; set; }

        public MilestoneConfidenceLevels MilestoneConfidenceLevel { get; set; }

        public string Caption { get; set; }

        public IList<Note> Notes { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDepartmentName { get; set; }
        public string ProjectDepartmentManagerFullName { get; set; }
        public ProjectType ProjectType { get; set; }
        public int ProjectTeamId { get; set; }
        public string ProjectTeamName { get; set; }
        public string ProjectTeamLeadFullName { get; set; }
        public string ProjectLeadFullName { get; set; }
        public string ProjectCaption { get; set; }
        public string ProjectDescription { get; set; }
        public int ProjectYear { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public decimal ProjectBudget { get; set; }
        public Uri ProjectWikiLocation { get; set; }
        public Uri ProjectJiraLocation { get; set; }
        public string ProjectJiraProject { get; set; }
    }
}