using System.Collections.Generic;
using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Venue {
    public interface IVenueRepository : IRepository<Venue> {
        Task<Venue> FindById(long id);
        Task<IEnumerable<Venue>> FindById(IEnumerable<long> ids);
        void Create(Venue venue);
    }
}
