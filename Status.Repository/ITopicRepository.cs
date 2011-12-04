using Status.Model;

namespace Status.Repository
{
    public interface ITopicRepository
    {
        Topic GetTopicByExternalId(string topicId);
    }
}