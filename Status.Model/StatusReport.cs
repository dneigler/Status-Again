using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class StatusReport : IStatusReport
    {
        private IList<StatusItem> _items = null;
        public virtual int Id { get; set; }
        public virtual DateTime PeriodStart { get; set; }
        public virtual DateTime PeriodEnd { get; set; }
        public virtual string Caption { get; set; }

        public virtual IList<StatusItem> Items
        {
            get { return _items ?? (_items = new List<StatusItem>()); }
            protected internal set { _items = value; }
        }

        public virtual void AddStatusItem(Topic statusTopic)
        {
            // validate that the status topic isn't already assigned to the report?
            // otherwise we have more than one status item being assigned which may cause confusion - but also may be limiting
            var si = new StatusItem(statusTopic);
            this.Items.Add(si);
        }

        public virtual void AddStatusItem(StatusItem statusItem)
        {
            this.Items.Add(statusItem);
        }
    }
}
