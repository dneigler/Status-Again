using System;
using FileHelpers;
using Status.ETL.Csv;
using Status.Model;

namespace Status.Etl.Csv
{
    [DelimitedRecord(",")]
    public class StatusCsvItem
    {
        // StatusDate,ProjectID,Project,StatusType,MilestoneDate,Note,JIRA ID,TeamID,BusinessAlignmentOverride,
        // ProjectTeamID,TeamName,TeamLead,JIRA Status,MilestoneConfidence,MilestoneDateType,ProjectSummary,
        // CurrentMonthAllocation,BudgetType,ParentProjectType,BudgetCaption,BudgetCaptionNumbered,StatusTypeOrdered,
        // Caption,OneLineCaption,BusinessAlignment,ProjectCaptionBudgeted
        [FieldConverter(ConverterKind.Date, "M/d/yyyy")] 
        public DateTime StatusDate;

        public int ProjectID;

        public string Project;

        /// <summary>
        /// Should be an enumeration
        /// </summary>
        [FieldConverter(typeof(MilestoneTypeConverter))]
        public MilestoneTypes StatusType;

        [FieldConverter(ConverterKind.Date, "M/d/yyyy")]
        public DateTime? MilestoneDate;

        [FieldQuoted()]
        public string Note;

        public string JiraID;

        public string TeamID;

        public string BusinessAlignmentOverride;

        public int ProjectTeamID;

        public string TeamName;

        public string TeamLead;

        public string JiraStatus;

        [FieldConverter(typeof(MilestoneConfidenceConverter))]
        [FieldNullValue(MilestoneConfidenceLevels.High)]
        public MilestoneConfidenceLevels? MilestoneConfidence;

        public string MilestoneDateType;

        [FieldQuoted()]
        public string ProjectSummary;

        public double CurrentMonthAllocation;

        public string BudgetType;

        public string ParentProjectType;

        public string BudgetCaption;

        public string BudgetCaptionNumbered;

        public string StatusTypeOrdered;

        [FieldQuoted()]
        public string Caption;

        [FieldQuoted()]
        public string OneLineCaption;
        
        public string BusinessAlignment;
        
        [FieldQuoted()]
        public string ProjectCaptionBudgeted;
    }
}