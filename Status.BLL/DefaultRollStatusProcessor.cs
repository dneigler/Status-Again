using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.BLL
{
    public class DefaultRollStatusProcessor : IRollStatusProcessor
    {
        public Model.StatusItem MapStatusItem(Model.StatusItem sourceStatusItem, DateTime statusReportDate)
        {
            StatusItem si = new StatusItem();
            si.Caption = sourceStatusItem.Caption;
            // this is a good place for windows workflow to simplify management of transfering milestone types
            if (sourceStatusItem.Milestone.Date < statusReportDate.AddDays(-7))
                return null;
            if (sourceStatusItem.Milestone.Date < statusReportDate)
                si.Milestone = new Milestone { ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel, Type = MilestoneTypes.LastWeek, Date = sourceStatusItem.Milestone.Date };
            else if (sourceStatusItem.Milestone.Date >= statusReportDate && sourceStatusItem.Milestone.Date < statusReportDate.AddDays(7))
                si.Milestone = new Milestone { ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel, Type = MilestoneTypes.ThisWeek, Date = sourceStatusItem.Milestone.Date };
            else if (sourceStatusItem.Milestone.Type == MilestoneTypes.Milestone || sourceStatusItem.Milestone.Type == MilestoneTypes.OpenItem)
                si.Milestone = new Milestone { ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel, Type = sourceStatusItem.Milestone.Type, Date = sourceStatusItem.Milestone.Date };
            else
                si.Milestone = new Milestone { ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel, Type = MilestoneTypes.Milestone, Date = sourceStatusItem.Milestone.Date };
            return si;
        }

        public DateTime GetPeriodStart(Model.StatusReport sourceReport)
        {
            throw new NotImplementedException();
        }

        public DateTime GetPeriodEnd(Model.StatusReport sourceReport)
        {
            throw new NotImplementedException();
        }
    }
}
