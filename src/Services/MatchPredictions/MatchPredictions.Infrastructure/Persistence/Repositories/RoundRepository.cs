using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MatchPredictions.Domain.Aggregates.Round;

namespace MatchPredictions.Infrastructure.Persistence.Repositories {
    public class RoundRepository : IRoundRepository {
        private readonly MatchPredictionsDbContext _matchPredictionsDbContext;

        public RoundRepository(MatchPredictionsDbContext matchPredictionsDbContext) {
            _matchPredictionsDbContext = matchPredictionsDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _matchPredictionsDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Round>> FindById(IEnumerable<long> ids) {
            var rounds = await _matchPredictionsDbContext.Rounds
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();

            return rounds;
        }

        public void Create(Round round) {
            _matchPredictionsDbContext.Rounds.Add(round);
        }
    }
}
