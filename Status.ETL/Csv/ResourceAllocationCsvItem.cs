using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using Status.Model;

namespace Status.ETL.Csv
{
    // EmployeeID,Name,JobCode,EmployeeType,EmployeeCategory,Month,ProjectID
    // Project,AllocationPercentage,Column1,InternalAllocationPercentage,WeightedPercentage
    // MonthlyCost,ProjectType,ParentProjectType,BudgetType,ProjectCaptionRollup,
    // ProjectTypeCaptionRollup,BusinessAlignment,TechnologyAlignment,ResourceTeamID,
    // ProjectTeamID,Team,ResourceTeam,ProjectTeam,ResourceTeamLead,BillingCode,TaskCode,
    // QuarterNumber,Quarter,AllocationPercentageNegative,ProjectYear,Year
    [DelimitedRecord(",")]
    public class ResourceAllocationCsvItem
    {
        public string EmployeeID;

        public string Name;

        public string JobCode;

        public string EmployeeType;

        public string EmployeeCategory;

        [FieldConverter(ConverterKind.Date, "M/d/yyyy")]
        public DateTime Month; 
        
        public uint ProjectID;
        
        [FieldQuoted()]
        public string Project;

        public decimal AllocationPercentage;

        public decimal Column1;

        public decimal? InternalAllocationPercentage;

        public decimal WeightedPercentage;

        public decimal MonthlyCost;

        [FieldQuoted()]
        public string ProjectType; 
        
        public string ParentProjectType;

        [FieldConverter(typeof(ProjectTypeConverter))]
        public ProjectType BudgetType;

        [FieldQuoted()]
        public string ProjectCaptionRollup;

        public string ProjectTypeCaptionRollup; 
        
        public string BusinessAlignment; 
        
        public string TechnologyAlignment;

        public uint ResourceTeamID;

        public uint ProjectTeamID; 
        
        public string Team; 
        
        public string ResourceTeam; 
        
        public string ProjectTeam; 
        
        public string ResourceTeamLead; 
        
        public string BillingCode; 
        
        public uint TaskCode;

        public uint QuarterNumber; 
        
        public string Quarter; 
        
        public decimal AllocationPercentageNegative; 
        
        public string ProjectYear; 
        
        public uint Year;
    }
}
