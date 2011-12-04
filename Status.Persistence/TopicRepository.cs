using System.Linq;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class TopicRepository : RepositoryBase, ITopicRepository
    {
        public TopicRepository(string connectionString) : base(connectionString)
        {
        }

        public Topic GetTopicByExternalId(string topicId)
        {
            using (var session = this.CreateSession())
            {
                var topic = (from t in session.Query<Topic>()
                               where t.ExternalId.Equals(topicId)
                               select t).Single();
                return topic;
            }
        }
    }
}