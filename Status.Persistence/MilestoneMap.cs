using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class MilestoneMap : ComponentMap<Milestone>
    {
        public MilestoneMap()
        {
            Map(x => x.Date);
            Map(x => x.ConfidenceLevel).CustomType<MilestoneConfidenceLevels>();
            Map(x => x.Type).CustomType<MilestoneTypes>();
        }
    }
}
