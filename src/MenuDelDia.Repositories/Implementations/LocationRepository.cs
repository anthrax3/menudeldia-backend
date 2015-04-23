using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        public LocationRepository(DbContext context)
            : base(context)
        {
        }
    }
}
