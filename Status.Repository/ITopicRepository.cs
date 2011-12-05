using Status.Model;

namespace Status.Repository
{
    public interface ITopicRepository : IRepository
    {
        Topic GetTopicByExternalId(string topicId);

        Topic GetTopicByCaption(string caption);

        void AddTopic(Topic topic);
    }
}