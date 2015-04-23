using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class RestaurantRepository: BaseRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(DbContext context)
            : base(context)
        {
        }
    }
}
