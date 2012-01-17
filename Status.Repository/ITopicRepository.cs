using Status.Model;

namespace Status.Repository
{
    public interface ITopicRepository : IRepository<Topic>
    {
        Topic GetTopicByExternalId(string topicId);

        Topic GetTopicByCaption(string caption);

        Topic GetOrAddTopicByCaption(string caption);
    }
}