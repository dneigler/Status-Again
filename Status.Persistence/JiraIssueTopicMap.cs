using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;

namespace Status.Persistence
{
    public class JiraIssueTopicMap : SubclassMap<JiraIssueTopic>
    {
        public JiraIssueTopicMap()
        {
            Map(x => x.JiraId);
        }
    }
}
