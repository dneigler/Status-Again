using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class ProjectRepository : RepositoryBase, IProjectRepository
    {
        #region Constructors

        public ProjectRepository(ISession session) : base(session)
        {
        }

        public ProjectRepository(string connectionString) : base(connectionString)
        {
        }

        public ProjectRepository(ITransaction transaction) : base(transaction)
        {
        }

        public ProjectRepository(string connectionString, ISession session) : base(connectionString, session)
        {
        }

        #endregion


        public Project GetProject(string projectName)
        {
            var session = Session;
            {
                var project = (from p in session.Query<Project>()
                               where p.Name.Equals(projectName)
                               select p).SingleOrDefault();
                return project;
            }
        }

        public IList<Project> GetProjectsByNames(IList<string> projectNames)
        {
            var session = Session;
            {
                var project = (from p in session.Query<Project>()
                               where projectNames.Contains(p.Name)
                               select p).ToList();
                return project;
            }
        }

        public IList<Project> GetProjectsByTeam(int teamId)
        {
            var session = Session;
            {
                var projects = (from p in session.Query<Project>()
                               where p.Team.Id.Equals(teamId)
                               select p).ToList();
                return projects;
            }
        }

        public IList<Project> GetAllProjects()
        {
            var session = Session;
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

            var session = Session;
            {
                session.Save(project);
            }
        }
    }
}