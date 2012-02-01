using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Status.Model
{
    public class StatusItem : IIdentityColumn
    {
        private string _caption = null;

        public StatusItem()
        {
            this.AuditInfo =
                new AuditInfo(new Resource()
                {
                    EmailAddress = WindowsIdentity.GetCurrent().Name,
                    FirstName = WindowsIdentity.GetCurrent().Name,
                    LastName = WindowsIdentity.GetCurrent().Name
                });
            this.Milestone = new Milestone() {ConfidenceLevel = MilestoneConfidenceLevels.High, Date = DateTime.Today.AddDays(14), Type = MilestoneTypes.Milestone};
            this.Notes = new List<Note>();
        }

        public StatusItem(Topic statusTopic) : this()
        {
            this.Topic = statusTopic;
        }

        public virtual int Id { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Milestone Milestone { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }

        public virtual StatusReport StatusReport { get; set; }

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
        public virtual IList<Note> Notes { get; set; }

        public virtual IList<Tag> Tags { get; set; }
        public virtual Project Project { get; set; }
    }
}
