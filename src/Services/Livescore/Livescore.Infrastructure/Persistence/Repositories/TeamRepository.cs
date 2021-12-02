using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Team;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class TeamRepository : ITeamRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public TeamRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Team> FindById(long id) {
            var team = await _livescoreDbContext.Teams.SingleOrDefaultAsync(
                t => t.Id == id
            );

            return team;
        }

        public void Create(Team team) {
            _livescoreDbContext.Teams.Add(team);
        }
    }
}
