using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.Repository
{
    public interface IResourceRepository
    {
        IList<Resource> GetAllResources();

        IList<Resource> GetResourcesByName(string fullName);

        IList<Resource> GetResourcesByTeam(int teamId);

        Resource GetResourceByExternalId(string externalId);

        Resource GetResourceByEmail(string emailAddress);

        void AddResource(Resource resource);
    }
}
