using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Status.Model
{
    public class Employee : Resource
    {
        private IList<Project> _projects = null;

        public virtual IList<Project> Projects
        {
            get { return _projects; }
            set { _projects = value; }
        }

        public Employee()
        {
            _projects = new List<Project>();

            WindowsPrincipal principal = Thread.CurrentPrincipal as WindowsPrincipal;
            if (principal != null)
            {
                // or, if you're in Asp.Net with windows authentication you can use:
                // WindowsPrincipal principal = (WindowsPrincipal)User;
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, principal.Identity.Name);
                    // this is the best way to go for defaulting the resource
                    this.EmailAddress = up.EmailAddress;
                    this.FirstName = up.GivenName;
                    this.LastName = up.Surname;
                    this.Description = up.Description;
                    this.DisplayName = up.DisplayName;
                    this.DistinguishedName = up.DistinguishedName;
                    this.Guid = up.Guid;
                    this.MiddleName = up.MiddleName;
                    this.Name = up.Name;
                    this.Sid = up.Sid.Value;
                    this.SamAccountName = up.SamAccountName;
                    this.UserPrincipalName = up.UserPrincipalName;
                    this.VoiceTelephoneNumber = up.VoiceTelephoneNumber;
                    // or return up.GivenName + " " + up.Surname;
                }
            }
            else
            {
                this.FirstName = WindowsIdentity.GetCurrent().Name;
                this.LastName = WindowsIdentity.GetCurrent().Name;
                this.EmailAddress = String.Format("{0}@test.com", this.FirstName);
                //GenericPrincipal gp = Thread.CurrentPrincipal as GenericPrincipal;
                //if (gp != null)
                //{
                //    this.FirstName = gp.Identity.Name;
                //    this.LastName = gp.Identity.Name;
                //    this.EmailAddress = gp.Identity.Name;
                //}
            }

        }

        public virtual string SamAccountName { get; set; }
        public virtual string UserPrincipalName { get; set; }
        public virtual string VoiceTelephoneNumber { get; set; }
        public virtual string Description { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string DistinguishedName { get; set; }
        public virtual Guid? Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string Sid { get; set; }
        public virtual string EdsId { get; set; }
        public virtual Team Team { get; set; }
        public virtual Title Title { get; set; }
        public virtual string WindowsLogin { get; set; }

    }
}
