using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class StatusItem
    {
        private string _caption = null;

        public StatusItem()
        {
            this.Milestone = new Milestone() {ConfidenceLevel = MilestoneConfidenceLevels.High, Date = DateTime.Today.AddDays(14), Type = MilestoneTypes.Milestone};
            this.Notes = new List<Note>();
            this.AuditInfo = AuditInfo.GetAudit();
        }

        public StatusItem(Topic statusTopic) : this()
        {
            this.Topic = statusTopic;
        }

        public virtual int Id { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Milestone Milestone { get; set; }
        public virtual string Caption
        {
            get
            {
                if (_caption == null) return this.Topic.Caption;
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }
        public virtual List<Note> Notes { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }
    }
}
