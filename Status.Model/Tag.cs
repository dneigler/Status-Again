﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Tag : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        // public virtual AuditInfo Creator { get; set; }
    }
}
