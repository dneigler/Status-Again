using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Status.Model
{
    public class AuditInfo : IEquatable<AuditInfo>
    {
        public Resource Author { get; private set; }
        public DateTime AuditTime { get; private set; }
        public string MachineName { get; private set; }

        private AuditInfo()
        {
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

        public AuditInfo(Resource author)
            : this(author, DateTime.Now, Environment.MachineName)
        {
        }

        public override int GetHashCode()
        {
            var result = Author.GetHashCode();
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

        public static AuditInfo GetAudit()
        {
            // TODO: Implement the Resource construction logic to pull from repository
            var windowsIdentity = WindowsIdentity.GetCurrent();
            return new AuditInfo
                       {
                           AuditTime = DateTime.Now,
                           Author = new Resource { EmailAddress = (windowsIdentity == null ? windowsIdentity.Name : "unknown@unknown.com") },
                           MachineName = Environment.MachineName
                       };
        }
    }
}
