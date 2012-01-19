using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Status.Repository;
using NHibernate;
using NHibernate.Linq;
using Status.Model;

namespace Status.Persistence
{
    public class ResourceRepository : RepositoryBase<Resource>, IResourceRepository
    {
        #region Constructors

        public ResourceRepository(ISession session)
            : base(session)
        {
        }

        public ResourceRepository(string connectionString)
            : base(connectionString)
        {
        }

        public ResourceRepository(ITransaction transaction)
            : base(transaction)
        {
        }

        public ResourceRepository(string connectionString, ISession session)
            : base(connectionString, session)
        {
        }

        #endregion
        
        public IList<Model.Resource> GetAllResources()
        {
            var session = this.Session;
            return (from r in session.Query<Resource>()
                    select r).ToList();
        }

        public IList<Model.Resource> GetResourcesByName(string fullName)
        {
            var session = this.Session;
            return (from r in session.Query<Resource>()
                    where r.FullName.Equals(fullName)
                    select r).ToList();
        }

        public IList<Model.Resource> GetResourcesByTeam(int teamId)
        {
            var session = this.Session;
            return (from r in session.Query<Employee>()
                    where r.Team.Id == teamId
                    select r).ToList<Resource>();
        }

        public Resource GetResourceByLogin(string login)
        {
            // Logins are currently only available for employees
            return (from r in this.Session.Query<Employee>()
                    where r.WindowsLogin.Equals(login)
                    select r).SingleOrDefault();
        }

        public Resource GetResourceByExternalId(string externalId)
        {
            // TODO: create external id column for derived types so this can be 
            // run against employees, recruiters, etc - current implementation only 
            // against employees.
            var session = this.Session;
            return (from r in session.Query<Employee>()
                    where r.EdsId.Equals(externalId)
                    select r).SingleOrDefault();
        }

        public Resource GetOrCreateResourceByIIdentity(IIdentity identity)
        {
            var r = this.GetResourceByLogin(identity.Name);
            if (r == null)
            {
                r = new Employee()
                        {
                            WindowsLogin = identity.Name
                        };
                this.Add(r);
            }
            return r;
        }

        public Model.Resource GetResourceByEmail(string emailAddress)
        {
            var session = this.Session;
            return (from r in session.Query<Resource>()
                    where r.EmailAddress.Equals(emailAddress)
                    select r).SingleOrDefault();
        }

        public void AddResource(Resource resource)
        {
            this.Session.Save(resource);
        }
    }
}
