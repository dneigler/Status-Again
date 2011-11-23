using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class NoteMap : ClassMap<Note>
    {
        public NoteMap()
        {
            Id(x => x.Id);
            Map(x => x.Text);
            Component(x => x.AuditInfo);
        }
    }
}
