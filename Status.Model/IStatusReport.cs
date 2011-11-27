using System.Collections.Generic;

namespace Status.Model
{
    public interface IStatusReport
    {
        IList<StatusItem> Items { get; }
        void AddStatusItem(Topic statusTopic);
        void AddStatusItem(StatusItem statusItem);
    }
}