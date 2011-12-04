using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class JiraProjectTopic : Topic
    {
        public virtual string JiraProjectId { get; set; }

        public override string ExternalId
        {
            get
            { return this.JiraProjectId; }
            set { this.JiraProjectId = value; }
        }
    }
}
