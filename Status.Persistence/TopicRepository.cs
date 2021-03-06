using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class TopicRepository : RepositoryBase<Topic>, ITopicRepository
    {
        public TopicRepository(string connectionString) : base(connectionString)
        {
        }

        public TopicRepository(ISession session) : base(session)
        {
        }

        public TopicRepository(ITransaction transaction) : base(transaction)
        {
        }

        public TopicRepository(string connectionString, ISession session) : base(connectionString, session)
        {
        }

        public Topic GetTopicByExternalId(string topicId)
        {
            var session = Session;
            {
                var topic = (from t in session.Query<Topic>()
                               where t.ExternalId.Equals(topicId)
                               select t).SingleOrDefault();
                return topic;
            }
        }

        public Topic GetTopicByCaption(string caption)
        {
            var session = Session;
            var topic = (from t in session.Query<Topic>()
                         where t.Caption.Equals(caption)
                         select t).FirstOrDefault();
            return topic;
        }

        public Topic GetOrAddTopicByCaption(string caption)
        {
            Topic topic = this.GetTopicByCaption(caption);
            if (topic == null)
            {
                topic = new Topic() { Caption = caption };
                this.Add(topic);
                // should not need to requery - fix Add method
                topic = this.GetTopicByCaption(caption);
            }
            return topic;
        }

    }
}