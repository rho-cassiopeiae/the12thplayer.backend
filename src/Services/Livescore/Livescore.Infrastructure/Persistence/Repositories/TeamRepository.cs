using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Team;
using Livescore.Domain.Base;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class TeamRepository : ITeamRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        private IUnitOfWork _unitOfWork;

        public TeamRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _livescoreDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _livescoreDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Team> FindById(long id) => _livescoreDbContext.Teams.SingleOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<Team>> FindById(IEnumerable<long> ids) {
            var teams = await _livescoreDbContext.Teams
                .AsNoTracking()
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            return teams;
        }

        public void Create(Team team) {
            _livescoreDbContext.Teams.Add(team);
        }
    }
}
