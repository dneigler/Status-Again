using System.Collections.Generic;
using Status.Model;

namespace Status.Repository
{
    public interface IProjectRepository : IRepository<Project>
    {
        Project GetProjectByName(string projectName);

        IList<Project> GetProjectsByNames(IList<string> projectNames);
        
        IList<Project> GetProjectsByTeam(int teamId);

        IList<Project> GetAllProjects();

        void AddProject(Project project);
    }
}