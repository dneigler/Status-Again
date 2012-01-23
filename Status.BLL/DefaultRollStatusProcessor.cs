using System;
using AutoMapper;
using Status.Model;

namespace Status.BLL
{
    public class DefaultRollStatusProcessor : IRollStatusProcessor
    {
        public DefaultRollStatusProcessor()
        {
            Mapper.CreateMap<StatusItem, StatusItem>()
                .ForMember(x => x.Id, y => { y.Ignore(); });
        }
        #region IRollStatusProcessor Members

        public StatusItem MapStatusItem(StatusItem sourceStatusItem, DateTime statusReportDate)
        {
            Mapper.CreateMap<StatusItem, StatusItem>();
            
            var si = new StatusItem();
            Mapper.Map(sourceStatusItem, si);
            // this is a good place for windows workflow to simplify management of transfering milestone types
            if (sourceStatusItem.Milestone.Date < statusReportDate.AddDays(-7))
                return null;
            if (sourceStatusItem.Milestone.Date < statusReportDate)
                si.Milestone.Type = MilestoneTypes.LastWeek;
            else if (sourceStatusItem.Milestone.Date >= statusReportDate &&
                     sourceStatusItem.Milestone.Date < statusReportDate.AddDays(7))
                si.Milestone.Type = MilestoneTypes.ThisWeek;
            //else if (sourceStatusItem.Milestone.Type != MilestoneTypes.Milestone &&
            //         sourceStatusItem.Milestone.Type != MilestoneTypes.OpenItem)
            //    si.Milestone.Type = MilestoneTypes.Milestone;
            return si;
        }
        #endregion
    }
}