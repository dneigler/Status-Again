using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class JiraIssueTopic : Topic
    {
        public virtual string JiraId { get; set; }
    }
}
