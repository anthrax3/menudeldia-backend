using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(DbContext context)
            : base(context)
        {
        }
    }
}
