using System.Threading.Tasks;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Venue {
    public interface IVenueRepository : IRepository<Venue> {
        Task<Venue> FindById(long id);
        void Create(Venue venue);
    }
}
