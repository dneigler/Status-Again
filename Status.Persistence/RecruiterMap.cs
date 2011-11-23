using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;

namespace Status.Persistence
{
    public class RecruiterMap : SubclassMap<Recruiter>
    {
        public RecruiterMap()
        {
            References(x => x.Company);
        }
    }
}
