using Status.Model;

namespace Status.Repository
{
    public interface ITopicRepository : IRepository
    {
        Topic GetTopicByExternalId(string topicId);
        void AddTopic(JiraIssueTopic jiraIssueTopic);
    }
}