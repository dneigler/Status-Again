using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.Persistence
{
    public interface ITagRepository
    {
        IList<StatusItem> GetItemsByTagName(string name);

        IList<StatusItem> GetItemsByTag(Tag tag);

        IList<StatusItem> GetItemsByTagId(int id);
    }
}
