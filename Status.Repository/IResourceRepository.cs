using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Status.Model;

namespace Status.Repository
{
    public interface IResourceRepository : IRepository<Resource>
    {
        IList<Resource> GetAllResources();

        IList<Resource> GetResourcesByName(string fullName);

        IList<Resource> GetResourcesByTeam(int teamId);

        Resource GetResourceByExternalId(string externalId);

        Resource GetResourceByLogin(string login);

        Resource GetResourceByEmail(string emailAddress);

        void AddResource(Resource resource);

        Resource GetOrCreateResourceByIIdentity(IIdentity identity);
    }
}
