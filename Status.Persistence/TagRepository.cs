using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.Persistence
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public IList<StatusItem> GetItemsByTagName(string name)
        {
            throw new NotImplementedException();
        }

        public IList<StatusItem> GetItemsByTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public IList<StatusItem> GetItemsByTagId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
