using System.Collections.Generic;
using System.Threading.Tasks;

using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Round {
    public interface IRoundRepository : IRepository<Round> {
        Task<IEnumerable<Round>> FindById(IEnumerable<long> ids);
        void Create(Round round);
    }
}
