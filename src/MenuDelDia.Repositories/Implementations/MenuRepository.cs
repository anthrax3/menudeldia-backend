using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(DbContext context)
            : base(context)
        {
        }
    }
}
