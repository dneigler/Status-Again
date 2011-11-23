using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class JiraProjectTopicMap : SubclassMap<JiraProjectTopic>
    {
        public JiraProjectTopicMap()
        {
            Map(x => x.JiraProjectId);
        }
    }
}
