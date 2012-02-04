using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using NHibernate.Linq;
using Status.Repository;

namespace Status.Persistence
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(string connectionString) : base(connectionString) {}

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

        public IList<Tag> GetAllTags()
        {
            var query = (from t in this.Session.Query<Tag>()
                         select t).ToList();
            return query;
        }
    }
}
