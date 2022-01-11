using System.Threading.Tasks;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.League {
    public interface ILeagueRepository : IRepository<League> {
        Task<League> FindById(long id);
        void Create(League league);
    }
}
