﻿using System;
using AutoMapper;
using Status.Model;

namespace Status.BLL
{
    public class DefaultRollStatusProcessor : IRollStatusProcessor
    {
        #region IRollStatusProcessor Members

        public StatusItem MapStatusItem(StatusItem sourceStatusItem, DateTime statusReportDate)
        {
            Mapper.CreateMap<StatusItem, StatusItem>();
            
            var si = new StatusItem();
            Mapper.Map<StatusItem, StatusItem>(sourceStatusItem, si);
            // this is a good place for windows workflow to simplify management of transfering milestone types
            if (sourceStatusItem.Milestone.Date < statusReportDate.AddDays(-7))
                return null;
            if (sourceStatusItem.Milestone.Date < statusReportDate)
                //si.Milestone.Type = MilestoneTypes.LastWeek;
                si.Milestone = new Milestone
                                   {
                                       ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel,
                                       Type = MilestoneTypes.LastWeek,
                                       Date = sourceStatusItem.Milestone.Date
                                   };
            else if (sourceStatusItem.Milestone.Date >= statusReportDate &&
                     sourceStatusItem.Milestone.Date < statusReportDate.AddDays(7))
                si.Milestone = new Milestone
                                   {
                                       ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel,
                                       Type = MilestoneTypes.ThisWeek,
                                       Date = sourceStatusItem.Milestone.Date
                                   };
            else if (sourceStatusItem.Milestone.Type == MilestoneTypes.Milestone ||
                     sourceStatusItem.Milestone.Type == MilestoneTypes.OpenItem)
                si.Milestone = new Milestone
                                   {
                                       ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel,
                                       Type = sourceStatusItem.Milestone.Type,
                                       Date = sourceStatusItem.Milestone.Date
                                   };
            else
                si.Milestone = new Milestone
                                   {
                                       ConfidenceLevel = sourceStatusItem.Milestone.ConfidenceLevel,
                                       Type = MilestoneTypes.Milestone,
                                       Date = sourceStatusItem.Milestone.Date
                                   };
            return si;
        }
        #endregion
    }
}