using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(DbContext context)
            : base(context)
        {
        }
    }
}
