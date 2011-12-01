using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;
using Status.Model;

namespace Status.ETL.Csv
{
    public class MilestoneTypeConverter : ConverterBase
    {
        public override object StringToField(string @from)
        {
            switch (@from)
            {
                case "Last Week":
                    return MilestoneTypes.LastWeek;
                case "This Week":
                    return MilestoneTypes.ThisWeek;
                case "Milestone":
                    return MilestoneTypes.Milestone;
                case "Open Item":
                    return MilestoneTypes.OpenItem;
            }
            return MilestoneTypes.Milestone;
        }

        public override string FieldToString(object from)
        {
            var mt = (MilestoneTypes)from;

            switch (mt)
            {
                case MilestoneTypes.LastWeek:
                    return "Last Week";
                case MilestoneTypes.Milestone:
                    return "Milestone";
                case MilestoneTypes.OpenItem:
                    return "Open Item";
                case MilestoneTypes.ThisWeek:
                    return "Last Week";
            }
            return "Milestone";
        }
    }
}
