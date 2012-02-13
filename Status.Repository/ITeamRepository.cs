using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.Repository
{
    public interface ITeamRepository : IRepository<Team>
    {
        IList<Team> GetAllTeams();

        IList<Team> GetTeamsByLead(string teamLeadEmail);

        Team GetTeamByName(string name);

        void AddTeam(Team team);

        IList<Team> GetAllTeamsDetail();
    }
}
