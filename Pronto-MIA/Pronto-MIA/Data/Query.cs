using System.Linq;
using HotChocolate;
using Microsoft.Extensions.Configuration;

namespace Pronto_MIA.Data
{
    public class Query
    {
        private readonly IConfiguration cfg;

        public Query(IConfiguration cfg)
        {
            this.cfg = cfg;
        }
        
        public IQueryable<Speaker> GetSpeakers([Service] InformbobDbContext context) =>
            context.Speakers;
    }
}