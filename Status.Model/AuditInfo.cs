using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Status.Model
{
    public class AuditInfo : IEquatable<AuditInfo>
    {
        private static readonly Employee DefaultEmployee = new Employee();
        public Resource Author { get; private set; }
        public DateTime AuditTime { get; private set; }
        public string MachineName { get; private set; }

        // we shouldn't use this constructor as can result in duplicates in database
        private AuditInfo() : this(DefaultEmployee, DateTime.Now, Environment.MachineName)
        {
            // default to employee
        }

        public AuditInfo(Resource author, DateTime auditTime, string machineName)
        {
            if (author == null)
                throw new ArgumentException("author must be defined.");
            if (string.IsNullOrWhiteSpace(machineName))
                throw new ArgumentException("machineName must be defined.");
            this.Author = author;
            this.AuditTime = auditTime;
            this.MachineName = machineName;
        }

        public AuditInfo(Resource author) : this(author, DateTime.Now, Environment.MachineName)
        {
        }

        public override int GetHashCode()
        {
            var result = Author.Id.GetHashCode();
            result = (result * 397) ^ (MachineName != null ? MachineName.GetHashCode() : 0);
            result = (result * 397) ^ AuditTime.GetHashCode();
            return result;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as AuditInfo);
        }

        public bool Equals(AuditInfo obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.AuditTime, AuditTime) &&
                Equals(obj.Author, Author) &&
                Equals(obj.MachineName, MachineName);
        }
    }
}
