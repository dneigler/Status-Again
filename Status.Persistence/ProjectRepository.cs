using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Exceptions;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    class ProjectRepository : RepositoryBase, IProjectRepository
    {
        public ProjectRepository(string connectionString) : base(connectionString)
        {
        }

        public Project GetProject(string projectName)
        {
            using (var session = CreateSession())
            {
                var project = (from p in session.Query<Project>()
                               where p.Name.Equals(projectName)
                               select p).Single();
                return project;
            }
        }

        public IList<Project> GetProjectsByNames(IList<string> projectNames)
        {
            using (var session = CreateSession())
            {
                var project = (from p in session.Query<Project>()
                               where projectNames.Contains(p.Name)
                               select p).ToList();
                return project;
            }
        }

        public IList<Project> GetProjectsByTeam(int teamId)
        {
            using (var session = CreateSession())
            {
                var projects = (from p in session.Query<Project>()
                               where p.Team.Id.Equals(teamId)
                               select p).ToList();
                return projects;
            }
        }

        public IList<Project> GetAllProjects()
        {
            using (var session = CreateSession())
            {
                var projects = (from p in session.Query<Project>()
                                select p).ToList();
                return projects;
            }
        }

        public void AddProject(Project project)
        {
            // double check that project doesn't exist
            Project existingProject = this.GetProject(project.Name);
            if (existingProject != null) throw new Exception(string.Format("Project name {0} already exists with id {1}", existingProject.Name, existingProject.Id));

            using (var session = CreateSession())
            {
                session.Save(project);
            }
        }
    }
}