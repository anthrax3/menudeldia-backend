using System.Data.Entity;
using MenuDelDia.Entities;
using MenuDelDia.Repository.Base;

namespace MenuDelDia.Repository
{
    public class SuggestionRepository : BaseRepository<Suggestion>, ISuggestionRepository
    {
        public SuggestionRepository(DbContext context)
            : base(context)
        {
        }
    }
}
