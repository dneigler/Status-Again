using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Recruiter : Resource
    {
        public virtual Company Company { get; set; }
    }
}
