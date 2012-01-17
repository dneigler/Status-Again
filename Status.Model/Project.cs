using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Project : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Department Department { get; set; }
        public virtual ProjectType Type { get; set; }
        public virtual Team Team { get; set; }
        public virtual Employee Lead { get; set; }
        public virtual IList<StatusItem> StatusItems { get; set; }
        public virtual string Caption { get; set; }
        public virtual string Description { get; set; }
        public virtual int Year { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual decimal Budget { get; set; }
        public virtual Uri WikiLocation { get; set; }
        public virtual Uri JiraLocation { get; set; }
        public virtual string JiraProject { get; set; }
        
    }
}
