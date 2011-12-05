using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Repository;
using NHibernate.Linq;
using Status.Model;

namespace Status.Persistence
{
    public class TeamRepository : RepositoryBase, ITeamRepository
    {
        public IList<Team> GetAllTeams()
        {
            return (from t in this.Session.Query<Team>()
                   select t).ToList();
        }

        public IList<Model.Team> GetTeamsByLead(string teamLeadEmail)
        {
            return (from t in this.Session.Query<Team>()
                    where t.Lead.EmailAddress.Equals(teamLeadEmail)
                    select t).ToList();
        }

        public Model.Team GetTeamByName(string name)
        {
            return (from t in this.Session.Query<Team>()
                    where t.Name.Equals(name)
                    select t).Single();
        }

        public void AddTeam(Model.Team team)
        {
            this.Session.Save(team);
        }
    }
}
