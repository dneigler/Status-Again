using Status.Model;

namespace Status.Repository
{
    public interface ITopicRepository : IRepository<Topic>
    {
        Topic GetTopicByExternalId(string topicId);

        Topic GetTopicByCaption(string caption);

        void AddTopic(Topic topic);
    }
}