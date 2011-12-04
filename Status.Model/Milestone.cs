using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Milestone
    {
        public virtual MilestoneTypes Type { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual MilestoneConfidenceLevels ConfidenceLevel { get; set; }

        /// <summary>
        /// Defaults values for Milestone in constructor.
        /// </summary>
        public Milestone()
        {
            this.ConfidenceLevel = MilestoneConfidenceLevels.High;
            this.Date = DateTime.Today;
            this.Type = MilestoneTypes.Milestone;
        }
    }
}
